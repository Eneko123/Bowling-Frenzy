using UnityEngine;

public class BoloE3 : EnemyBase
{
    new void Start()
    {
        maxHealth = 50;
        base.Start();
        agent.speed = 4f;
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
