using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //variable
    public int hp = 10;
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


}
