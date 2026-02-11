using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variable
    public Transform player;
    private NavMeshAgent agent;
    //numero random

    //metodos
    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update() 
    { 
        Movemetn();
    }

    void Movemetn()
    { 
        agent.SetDestination(player.position);
    }

    private class Bolo1
    {
        private int hp = 10;
    }

    private class Bolo2
    {
        private int hp = 5;
    }

    private class Bolo3
    {
        private int hp = 20;
    }

    private class BoloBoss
    {
        private int hp = 200;
    }

}
