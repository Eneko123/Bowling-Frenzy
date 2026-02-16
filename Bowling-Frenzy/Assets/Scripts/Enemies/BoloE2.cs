using UnityEngine;

public class BoloE2 : EnemyBase
{
    new void Start()
    {
        base.Start();
        maxHealth = 10;
        agent.speed = 3.5f;
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
