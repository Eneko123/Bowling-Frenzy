using UnityEngine;
using UnityEngine.AI;

public class BoloEBoos : EnemyBase
{
    public GameObject jump;
    public GameObject atack;
    private readonly float playerDistance = 15;

    private readonly float cooldown1 = 5f;
    private readonly float cooldownMax1 = 5f;
    private readonly float velocityMin1 = 1;
    private readonly float velocityMax1 = 1;
            
    private readonly float cooldown2 = 5f;
    private readonly float cooldownMax2 = 5f;
    private readonly float velocityMin2 = 1;
    private readonly float velocityMax2 = 1;
            
    private readonly float cooldown3 = 5f;
    private readonly float cooldownMax3 = 5f;
    private readonly float velocityMin3 = 1;
    private readonly float velocityMax3 = 1;
             
    private readonly float cooldown4 = 5f;
    private readonly float cooldownMax4 = 5f;
    private readonly float velocityMin4 = 1;
    private readonly float velocityMax4 = 1;

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
        if (health <= maxHealth && health > maxHealth * 0.75f)
        {
            Atack(cooldown1, cooldownMax1);
            ApproachPlayer(velocityMin1, velocityMax1);
        }
        else if (health <= maxHealth * 0.75f && health > maxHealth * 0.5f)
        {
            Atack(cooldown2, cooldownMax2);
            ApproachPlayer(velocityMin2, velocityMax2);
        }
        else if (health <= maxHealth * 0.5f && health > maxHealth * 0.25f)
        {
            Atack(cooldown3, cooldownMax3);
            ApproachPlayer(velocityMin3, velocityMax3);
        }
        else if (health <= maxHealth * 0.25f && health > 0)
        {
            Atack(cooldown4, cooldownMax4);
            ApproachPlayer(velocityMin4, velocityMax4);
        }
    }

    void Atack(float cd, float cdMax)
    {
        // Patron de ataque: Si le jugador esta a playerDistance unidades, el cooldown ha llegado a zero y dependiendo del bool. El enemigo ataca o salta
        if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && cd <= 0 && !atacked)
        {
            // Activamos animacion, reseteamos el cooldown y cambiamos el bool para que el siguiente ataque sea el salto
            animator.SetBool("Atack", true);
            cd = cdMax;
            atacked = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && cd <= 0 && atacked)
        {
            // Activamos animacion, reseteamos el cooldown y cambiamos el bool para que el siguiente ataque sea el ataque
            animator.SetBool("Jump", true);
            cd = cdMax;
            atacked = false;
        }
        else
        {
            // Debug.Log(Vector3.Distance(transform.position, player.transform.position));
            cd -= Time.deltaTime;
        }
    }

    void ApproachPlayer(float min, float max)
    {
        // Si el jugador esta a mas de playerDistance unidades, el enemigo alcelera, si no, vuelve a su velocidad normal 
        if (Vector3.Distance(transform.position, player.transform.position) > playerDistance)
        {
            agent.speed = max;
        }
        else
        {
            agent.speed = min;
        }
    }

    new void Dead()
    {
        base.Dead();
        if (isDead)
        {
            // Inmoviliza al enemigo y activa la animación de muerte
            agent.speed = 0;
            animator.SetBool("Dead", true);
        }
    }

    // Funciones de animaciones
    // Desactiva al enemigo
    void DeadAnim()
    {
        this.gameObject.SetActive(false);
    }
    // Desactiva el salto
    void StopJump()
    {
        animator.SetBool("Jump", false);
    }
    // Desactiva el ataque
    void StopAtack()
    {
        animator.SetBool("Atack", false);
    }
    // Activa el ataque
    void AttackAnim()
    {
        atack.SetActive(true);
    }
    // Activa el salto
    void JumpAnim()
    {
        jump.SetActive(true);
    }
}
