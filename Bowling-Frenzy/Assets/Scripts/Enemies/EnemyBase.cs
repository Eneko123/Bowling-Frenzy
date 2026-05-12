using System.Collections;
using System.Collections.Generic;
using UnityEditor.SpeedTree.Importer;
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
    protected void Awake()
    {   
        player = MainCharacter.Instance.transform;
        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (animator == null) { animator = GetComponent<Animator>(); }
        if (col == null) { col = GetComponent<Collider>(); }
        player = MainCharacter.Instance.playerTransform;
        health = maxHealth;        
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
                maxHealth += 2f;      // +2 de vida por ronda
                damage += 0.5f;             // +0.5 de danio por ronda
                agent.speed += 0.2f;      // +0.2 de velocidad por ronda
                break;

            case Difficulty.Hard:
                maxHealth += 3.5f;   
                damage += 1.5f;      
                agent.speed += 0.04f;
                break;
        }

        // Actualizar la vida actual y el danio actual
        health = maxHealth;
        currentDamage = damage;
    }

    public virtual void ReceiveDamage(float damage, bool isBarredora)
    {
        if(pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        else
        {
            pulseCoroutine = StartCoroutine(ColorPulse());
        }
        //Debug.Log(health);
        health -= damage;
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
        }
        if (health <= 0 && !isBarredoraOn)
        {
            GivePoints(points);
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
    }

    protected IEnumerator ColorPulse()
    {
        materials[0].color = pulseMaterial.color; // Cambia a rojo para indicar que está ralentizado
        yield return new WaitForSeconds(0.2f);
        materials[0].color = originalColor; // Vuelve al color original
        pulseCoroutine = null; // Reinicia la referencia al coroutine
    }
    internal void SlowEnemy()
    {
        if (!isSlowing)
        {

            originalSpeed = GetEnemySpeed();
            SetEnemySpeed(originalSpeed / 2);
            isSlowing = true;
            StartCoroutine(TimerSlow());
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MainCharacter player))
        {
            // Solo hacer daño si el cooldown ha terminado
            if (damageCooldown <= 0)
            {
                player.damageHealthPlayer(damage);
                damageCooldown = 1.5f; // Reiniciar cooldown
                Debug.Log("Daño aplicado en OnCollisionEnter");
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MainCharacter player))
        {
            // Reducir el cooldown constantemente
            damageCooldown -= Time.deltaTime;

            // Hacer daño solo cuando el cooldown llega a 0 o menos
            if (damageCooldown <= 0)
            {
                player.damageHealthPlayer(damage);
                damageCooldown = 1.5f; // Reiniciar cooldown
                Debug.Log("Daño aplicado en OnCollisionStay");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MainCharacter player))
        {
            // Resetear el cooldown cuando deja de tocar al jugador
            // Esto hace que el próximo contacto haga daño inmediato
            damageCooldown = 0f;
            Debug.Log("Jugador salió de colisión - cooldown reseteado");
        }
    }
}