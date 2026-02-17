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


    //metodos
    protected void Start() 
    {
        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (animator == null) { animator = GetComponent<Animator>(); }
        player = MainCharacter.Instance.playerTransform;
        health = maxHealth; 
        animator.SetBool("Move", true);
    }
    void Update() 
    { 
        Movemetn();
        Dead();
    }

    protected void Movemetn()
    {
        // movimiento basico del enemigo, se dirige hacia el jugador gracias al NavMeshAgent
        agent.SetDestination(player.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeDamage(damage);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
    }

    protected void Dead()
    {
        if (health <= 0)
        { 
            isDead = true;
        }
    }
}
