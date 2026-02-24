using UnityEngine;

public class BoloEBoos : EnemyBase
{
    public GameObject jump;
    public GameObject atack;
    public float playerDistance = 5;
    private float cooldown = 5f;
    private float cooldownMax = 5f;
    private bool atacked = false;

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
        Atack();
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

    void Atack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && cooldown <= 0 && !atacked)
        {
            animator.SetBool("Atack", true);
            cooldown = cooldownMax;
            atacked = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && cooldown <= 0 && atacked)
        {
            animator.SetBool("Jump", true);
            cooldown = cooldownMax;
            atacked = false;
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }




    void StopJump()
    {
        animator.SetBool("Jump", false);
    }

    void StopAtack()
    {
        animator.SetBool("Atack", false);
    }

    void AttackAnim()
    {
        atack.SetActive(true);
    }

    void JumpAnim()
    {
        jump.SetActive(true);
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
