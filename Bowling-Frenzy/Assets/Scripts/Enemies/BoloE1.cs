using UnityEngine;

public class BoloE1 : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 20;
        agent.speed = 2.5f;
    }

    new void Dead()
    {
        base.Dead();
        if (isDead)
        {
            agent.speed = 0;
            animator.SetBool("Dead", true);
        }
    }
}
