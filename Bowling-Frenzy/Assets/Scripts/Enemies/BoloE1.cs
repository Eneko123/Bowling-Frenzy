using UnityEngine;

public class BoloE1 : EnemyBase
{
    new void Start()
    {
        maxHealth = 20;
        base.Start();
        agent.speed = speed;
        currentDamage = damage;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MainCharacter player))
        {
            player.damageHealthPlayer(currentDamage);
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
