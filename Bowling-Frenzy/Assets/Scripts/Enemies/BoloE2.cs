using UnityEngine;

public class BoloE2 : EnemyBase
{
    new void Awake()
    {
        maxHealth = 10;
        base.Awake();
        agent.speed = 7f;
        points = 250;
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
