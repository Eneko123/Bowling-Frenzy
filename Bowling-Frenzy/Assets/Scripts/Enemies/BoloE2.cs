using UnityEngine;

public class BoloE2 : EnemyBase
{
    new void Start()
    {
        maxHealth = 10;
        base.Start();
        agent.speed = 7f;
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
