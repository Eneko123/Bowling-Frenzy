using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    [SerializeField] protected float damage;
    [SerializeField] protected float currentDamage;
    protected NavMeshAgent agent;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float health;
    protected Animator animator;
    protected float originalSpeed;
    protected bool isSlowing = false;
    [SerializeField] protected int points;
    private float slowTime = 2f;
    private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material pulseMaterial;
    private Collider col;
    protected Coroutine pulseCoroutine;
    protected Color originalColor;
    protected Material[] materials;
    float damageCooldown = 0;
    bool isPiercing;
    public NormalBulletBehaviour BulletGetter;
    protected void Awake()
    {
        player = MainCharacter.Instance.transform;

        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (animator == null) { animator = GetComponent<Animator>(); }
        if (col == null) { col = GetComponent<Collider>(); }
        player = MainCharacter.Instance.playerTransform;
        health = maxHealth;
        isPiercing = false;
    }

    protected void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        materials = meshRenderer.materials;
        originalColor = materials[0].color;
    }

    //metodos
    protected void Update()
    {
        Movemetn();
    }

    public virtual void DificultySystem()
    {
        // Evitar errores si el GameManager no esta presente
        if (GameManager.Instance == null) { return; }

        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.Easy:
                // Sin cambios, los valores quedan como estan
                break;

            case Difficulty.Normal:
                // Incremento ADITIVO por ronda (SUMA)
                // Usa los valores BASE y suma el incremento por ronda
                maxHealth += 5f;      // +2 de vida por ronda
                damage += 2.5f;             // +0.5 de danio por ronda
                agent.speed += 0.25f;      // +0.2 de velocidad por ronda
                break;

            case Difficulty.Hard:
                maxHealth += 15f;
                damage += 5f;
                agent.speed += 0.5f;
                break;
        }

        // Actualizar la vida actual y el danio actual
        health = maxHealth;
        currentDamage = damage;
    }

    public virtual void ReceiveDamage(float damage, bool isBarredora)
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        pulseCoroutine = StartCoroutine(ColorPulse());
        
        //Debug.Log(health);
        health -= damage;
        AudioManager.Instance.PlaySFX("BolosRecibeDano");
        if (Combos.Instance != null && !isBarredora)
            Combos.Instance.IncrementCombo();
        //Debug.Log(health);
        Dead(isBarredora);
    }
    protected void Movemetn()
    {
        // para evitar errores, si el enemigo esta muerto o desactivado, no se mueve ni hace nada
        if (!this.gameObject.activeSelf) { return; }
        if (player == null) { return; }
        if (agent.enabled)
        {
            // movimiento basico del enemigo, se dirige hacia el jugador gracias al NavMeshAgent
            agent.SetDestination(player.position);
        }
        if (BoloEBoos.Instance != null && BoloEBoos.Instance.GetBossIsDead())
        {
            ReceiveDamage(99999, true);
        }

        StopSpeedIfIsSweeperActive();
    }
    internal float GetEnemySpeed()
    {
        return agent.speed;
    }
    protected virtual void Dead(bool isBarredoraOn)
    {
        if (health <= 0)
        {
            currentDamage = 0;
            agent.enabled = false;
            col.enabled = false;
            animator.SetTrigger("Dead");
            AudioManager.Instance.PlaySFX("BolosMuerte", 0.2f);
        }
        if (health <= 0 && !isBarredoraOn)
        {
            GivePoints(points);
            AudioManager.Instance.PlaySFX("BolosMuerte");
        }
    }
    internal void SetEnemySpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }
    IEnumerator TimerSlow()
    {
        yield return new WaitForSeconds(slowTime);
        isSlowing = false;
        SetEnemySpeed(originalSpeed);
        // Restaurar el color original cuando termine la ralentizacion
        if (materials != null && materials.Length > 0)
        {
            materials[0].color = originalColor;
        }
    }

    protected IEnumerator ColorPulse()
    {
        materials[0].color = pulseMaterial.color; // Cambia a rojo para indicar que esta ralentizado
        yield return new WaitForSeconds(0.2f);
        // Solo restaura al original si no está ralentizado
        if (!isSlowing)
        {
            materials[0].color = originalColor;
        }
        pulseCoroutine = null; // Reinicia la referencia al coroutine
    }

    // Nuevo metodo para ralentizar con duracion personalizada y cambio de color azul claro
    internal void SlowEnemyWithDuration(float duration)
    {
        if (!isSlowing)
        {
            originalSpeed = GetEnemySpeed();
            SetEnemySpeed(originalSpeed / 2);
            isSlowing = true;
            slowTime = duration;

            // Cambiar el color a azul claro para feedback visual
            if (materials != null && materials.Length > 0)
            {
                materials[0].color = new Color(0.5f, 0.8f, 1f, 1f); // Azul claro (RGB: 128, 204, 255)
            }

            StartCoroutine(TimerSlow());
            Debug.Log("Enemy slowed with color change for: " + duration + " seconds");
        }
    }
    void StopSpeedIfIsSweeperActive()
    {
        float currentSpeed = GetEnemySpeed();

        if (Sweeper.instance == null) { return; } // Para que deje de saltar errores

        if (Sweeper.instance.gameObject.activeSelf && Sweeper.instance != null)
        {
            agent.enabled = false;
        }
        else if (!Sweeper.instance.gameObject.activeSelf && Sweeper.instance != null)
        {
            SetEnemySpeed(currentSpeed);
        }
    }

    // Desactiva al enemigo
    void DeadAnim()
    {
        materials[0].color = originalColor;
        health = maxHealth;
        currentDamage = damage;
        agent.enabled = true;
        col.enabled = true;
        this.gameObject.SetActive(false);
    }
    protected void GivePoints(int points)
    {
        UIGameplay.uI.AddScore(points);
    }
    public float GetSlowTime()
    {
        return slowTime;
    }
    public void SetSlowTime(float newSlowTime)
    {
        slowTime = newSlowTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out MainCharacter player))
        {
            // Solo hacer danio si el cooldown ha terminado
            player.damageHealthPlayer(damage);
            AudioManager.Instance.PlaySFX("AtqNormalBolos");
            damageCooldown = 1.5f; // Reiniciar cooldown
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out MainCharacter player))
        {
            // Reducir el cooldown constantemente
            damageCooldown -= Time.deltaTime;

            // Hacer danio solo cuando el cooldown llega a 0 o menos
            if (damageCooldown <= 0)
            {
                player.damageHealthPlayer(damage);
                AudioManager.Instance.PlaySFX("AtqNormalBolos");
                damageCooldown = 1.5f; // Reiniciar cooldown
            }
        }
    }

    public float GetHealth() => health;

}