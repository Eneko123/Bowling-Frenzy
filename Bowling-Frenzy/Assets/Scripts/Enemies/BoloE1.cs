using UnityEngine;
using UnityEngine.AI;

public class BoloE1 : EnemyBase
{
    void Start()
    {
     maxHealth = 20;
     agent.speed = 2.5f;
    }
}
