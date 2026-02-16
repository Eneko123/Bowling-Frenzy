using UnityEngine;

public class BoloE1 : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 20;
        agent.speed = 2.5f;
    }
}
