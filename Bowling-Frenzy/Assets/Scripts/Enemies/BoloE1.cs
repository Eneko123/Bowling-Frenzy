using UnityEngine;

public class BoloE1 : EnemyBase
{
    new void Start()
    {
        maxHealth = 20;
        base.Start();
        agent.speed = 5f;
    }

    //protected override void Dead()
    //{
    //    base.Dead();
    //    if (isDead)
    //    {
    //        agent.speed = 0;
    //        animator.SetTrigger("Dead");
    //    }
    //}
}
