using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    protected int damage;
    protected NavMeshAgent agent;
    protected int maxHealth;
    protected int health;
    protected bool isDead = false;
    protected Animator animator;
    protected float originalSpeed;
    protected bool isSlowing = false;

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
    internal void ReceiveDamage(int damage)
    {
        Debug.Log(health);
        health -= damage;
        Debug.Log(health);
    }
    protected void Movemetn()
    {
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
            isDead = true;
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
}
