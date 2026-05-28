using UnityEngine;

public class BoloE3 : EnemyBase
{
    new void Awake()
    {
        maxHealth = 40;
        base.Awake();
        agent.speed = 4f;
        points = 50;
        damage = 5f;
    }

    protected override void Dead(bool isBarredora)
    {
        base.Dead(isBarredora);
        if (!isBarredora)
        {
            GameManager.Instance.bolo3Score += 1;
        }
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
