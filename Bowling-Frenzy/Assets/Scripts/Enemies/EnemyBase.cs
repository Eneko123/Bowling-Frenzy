using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    protected float damage;
    protected NavMeshAgent agent;
    protected float maxHealth;
    protected float health;
    protected Animator animator;
    protected float originalSpeed;
    protected bool isSlowing = false;

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
    }
    protected void Update() 
    { 
        Movemetn();
        Dead();
    }
    public virtual void ReceiveDamage(float damage)
    {
        //Debug.Log(health);
        health -= damage;
        if (Combos.Instance != null)
            Combos.Instance.IncrementCombo();
        //Debug.Log(health);
    }
    protected void Movemetn()
    {
        // para evitar errores, si el enemigo esta muerto o desactivado, no se mueve ni hace nada
        if (!this.gameObject.activeSelf) { return; }
        if (player == null) { return; }
        // movimiento basico del enemigo, se dirige hacia el jugador gracias al NavMeshAgent
        agent.SetDestination(player.position);

    }
    internal float GetEnemySpeed()
    {
        return agent.speed;
    }
    protected void Dead()
    {
        if (health <= 0)
        {
            damage = 0;
            agent.speed = 0;
            animator.SetTrigger("Dead");
            health = maxHealth;
        }
    }
    internal void SetEnemySpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }
    IEnumerator TimerSlow()
    {
        yield return new WaitForSeconds(2f);
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
        this.gameObject.SetActive(false);
    }
}
