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

        int round = RoundsManager.instance.CurrentRound;

        // Evitar errores si el GameManager no esta presente, aunque deberia estarlo siempre. Borrar luego
        if (GameManager.Instance == null) { return; }
        float mult = GameManager.Instance.difficulty switch
        {
            Difficulty.Easy => 1f,                        // sin aumento
            Difficulty.Normal => Mathf.Pow(1.10f, round),  // +10% acumulado por ronda
            Difficulty.Hard => Mathf.Pow(1.30f, round),  // +30% acumulado por ronda
            _ => 1f
        };

        maxHealth *= mult;
        damage *= mult;
        agent.speed *= mult/2;
    }
    protected void Update() 
    { 
        Movemetn();
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
