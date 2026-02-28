using UnityEngine;

public class BoloE3 : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 50;
        agent.speed = 2f;
    }
    new void Dead()
    {
        base.Dead();
        if (isDead)
        {
            agent.speed = 0;
            animator.SetTrigger("Dead");
        }
    }
}
