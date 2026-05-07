using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    protected float damage;
    protected float currentDamage;
    protected NavMeshAgent agent;
    protected float maxHealth;
    protected float health;
    protected Animator animator;
    protected float originalSpeed;
    protected bool isSlowing = false;
    [SerializeField] protected int points;
    private float slowTime = 2f;

    // Variables para guardar los valores BASE antes de aplicar dificultad
    protected float baseMaxHealth;
    protected float baseDamage;
    protected float baseSpeed;

    private void Awake()
    {
        player = MainCharacter.Instance.transform;
    }

    //metodos
    protected void Start()
    {
        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (animator == null) { animator = GetComponent<Animator>(); }
        player = MainCharacter.Instance.playerTransform;
        health = maxHealth;

        // Guardar valores base ANTES de aplicar modificadores de dificultad
        baseMaxHealth = maxHealth;
        baseDamage = damage;
        baseSpeed = agent.speed;

        // Aplicar el sistema de dificultad
        DificultySystem();
    }
    protected void Update()
    {
        Movemetn();
    }

    public virtual void DificultySystem()
    {
        int round = RoundsManager.instance.CurrentRound;

        // Evitar errores si el GameManager no esta presente
        if (GameManager.Instance == null) { return; }

        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.Easy:
                // Sin cambios - los valores quedan como están
                // No hace falta modificar nada
                break;

            case Difficulty.Normal:
                // Incremento ADITIVO por ronda (SUMA)
                // Usa los valores BASE y suma el incremento por ronda
                maxHealth = baseMaxHealth + (10f * round);      // +10 de vida por ronda
                damage = baseDamage + (5f * round);             // +5 de daño por ronda
                agent.speed = baseSpeed + (0.05f * round);      // +0.05 de velocidad por ronda
                break;

            case Difficulty.Hard:
                // Incremento MULTIPLICATIVO acumulativo por ronda
                // Formula: valorBase * (1 + porcentaje)^ronda
                maxHealth = baseMaxHealth * Mathf.Pow(1.10f, round);   // ×1.10 (10% mas) por ronda acumulado
                damage = baseDamage * Mathf.Pow(1.05f, round);         // ×1.05 (5% mas) por ronda acumulado
                agent.speed = baseSpeed * Mathf.Pow(1.01f, round);     // ×1.01 (1% mas) por ronda acumulado
                break;
        }

        // Actualizar la vida actual y el danio actual
        health = maxHealth;
        currentDamage = damage;
    }

    public virtual void ReceiveDamage(float damage)
    {
        //Debug.Log(health);
        health -= damage;
        if (Combos.Instance != null)
            Combos.Instance.IncrementCombo();
        //Debug.Log(health);
        Dead();
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
    protected virtual void Dead()
    {
        if (health <= 0)
        {
            currentDamage = 0;
            agent.enabled = false;
            animator.SetTrigger("Dead");
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
        health = maxHealth;
        currentDamage = damage;
        agent.enabled = true;
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
}