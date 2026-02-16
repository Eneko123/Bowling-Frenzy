using UnityEngine;

public class BoloE2 : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 10;
        agent.speed = 3.5f;
    }
}
