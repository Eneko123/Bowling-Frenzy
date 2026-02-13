using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variables universales para todas las clases de enemigos
    public Transform player;
    private NavMeshAgent agent;
    //numero random

    //metodos
    void Start() 
    {
        // asignamos los componentes necesarios
        if (agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (player == null) { player = GameObject.FindGameObjectWithTag("Player").transform; }
    }
    void Update() 
    { 
        Movemetn();
    }

    void Movemetn()
    {
        // movimiento basico del enemigo, se dirige hacia el jugador gracias al NavMeshAgent
        agent.SetDestination(player.position);
    }

    // clases de enemigos, cada una con sus propias variables, como hp, velocidad, etc.
    //private class Bolo1
    //{
    //    private int hp = 10;
    //}

    //private class Bolo2
    //{
    //    private int hp = 5;
    //}

    //private class Bolo3
    //{
    //    private int hp = 20;
    //}

    //private class BoloBoss
    //{
    //    private int hp = 200;
    //}

}
