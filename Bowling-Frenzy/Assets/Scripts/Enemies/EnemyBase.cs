using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    public int damage;
    protected NavMeshAgent agent;
    protected int maxHealth;
    protected int health;
    bool isDead = false;


    //metodos
    void Start() 
    {
        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; } // mala practica, cambiar player controler a ser un instance o que desde el game manager se le asigne al enemigo el player
        health = maxHealth;
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

    void Dead()
    {
        if (health <= 0)
        { 
            isDead = true;
        }
    }
}
