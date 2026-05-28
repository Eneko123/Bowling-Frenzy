using UnityEngine;

public class BoloE1 : EnemyBase
{
    new void Awake()
    {
        maxHealth = 15;
        base.Awake();
        agent.speed = 5;
        points = 100;
        damage = 10f;
    }


    protected override void Dead(bool isBarredora)
    {
        base.Dead(isBarredora);
        if (!isBarredora)
        {
            GameManager.Instance.bolo1Score += 1;
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
