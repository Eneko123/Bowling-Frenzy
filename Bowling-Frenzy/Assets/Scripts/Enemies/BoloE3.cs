using UnityEngine;

public class BoloE3 : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 50;
        agent.speed = 2f;
    }
}
