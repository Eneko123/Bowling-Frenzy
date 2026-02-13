using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    protected NavMeshAgent agent;
    protected int maxHealth;
    protected int health;
    bool isDead = false;


    //metodos
    void Start() 
    {
        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; }
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

    void Dead()
    {
        if (health <= 0)
        { 
            isDead = true;
        }
    }
}
