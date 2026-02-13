using UnityEngine;

public class BoloEBoos : EnemyBase
{
    void Start()
    {
        maxHealth = 1000;
        agent.speed = 1f;
    }
}
