using UnityEngine;

public class BoloEBoos : EnemyBase
{
    public GameObject jump;
    public GameObject atack;
    private float playerDistance = 15;
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
        ApproachPlayer();
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
        // Patron de ataque: Si le jugador esta a playerDistance unidades, el cooldown ha llegado a zero y dependiendo del bool. El enemigo ataca o salta
        if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && cooldown <= 0 && !atacked)
        {
            // Activamos animacion, reseteamos el cooldown y cambiamos el bool para que el siguiente ataque sea el salto
            animator.SetBool("Atack", true);
            cooldown = cooldownMax;
            atacked = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && cooldown <= 0 && atacked)
        {
            // Activamos animacion, reseteamos el cooldown y cambiamos el bool para que el siguiente ataque sea el ataque
            animator.SetBool("Jump", true);
            cooldown = cooldownMax;
            atacked = false;
        }
        else
        {
            // Debug.Log(Vector3.Distance(transform.position, player.transform.position));
            cooldown -= Time.deltaTime;
        }
    }

    void ApproachPlayer()
    {
        // Si el jugador esta a mas de playerDistance unidades, el enemigo alcelera, si no, vuelve a su velocidad normal 
        if (Vector3.Distance(transform.position, player.transform.position) > playerDistance)
        {
            agent.speed = 5f;
        }
        else
        {
            agent.speed = 1f;
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
