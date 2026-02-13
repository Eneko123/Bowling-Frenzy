using UnityEngine;

public class BoloEBoos : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 1000;
        agent.speed = 1f;
    }
}
