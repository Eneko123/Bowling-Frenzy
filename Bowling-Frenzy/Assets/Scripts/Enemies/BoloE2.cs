using UnityEngine;

public class BoloE2 : EnemyBase
{
    new void Awake()
    {
        maxHealth = 5;
        base.Awake();
        agent.speed = 7f;
        points = 250;
        damage = 15f;
    }

    protected override void DeadAnim()
    {
        base.DeadAnim();
        GameManager.Instance.bolo2Score += 1;
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
