using UnityEngine;

public class BoloE1 : EnemyBase
{
    new void Awake()
    {
        maxHealth = 20;
        base.Awake();
        agent.speed = 5;
        points = 100;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MainCharacter player))
        {
            player.damageHealthPlayer(damage);
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
