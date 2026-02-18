using UnityEngine;

public class BoloEBoos : EnemyBase
{
    public GameObject jump;
    public GameObject atack;

    new void Start()
    {
        base.Start();
        maxHealth = 1000;
        agent.speed = 1f;
    }

    new void Update()
    {
        base.Update();
        States();
    }

    void States()
    {
        jump.SetActive(true);
    if (health <= maxHealth * 0.75f && health > maxHealth * 0.5f)
        {
            agent.speed = 1.2f;
        }
    else if (health <= maxHealth * 0.5f && health > maxHealth * 0.25f)
        {
            agent.speed = 1.5f;
        }
    else if (health <= maxHealth * 0.25f && health > 0)
        {
            agent.speed = 1.8f;
        }
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
