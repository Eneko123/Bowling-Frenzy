using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct State
{
    public float CooldownMax;
    public float MinVel;
    public float MaxVel;
}

public class BoloEBoos : EnemyBase
{
    //public static BoloEBoos instance;

    public State[] EnemyStates;
    private State CurrentState;
    private float Cooldown;

    public GameObject jump;
    public GameObject atack;
    private readonly float playerDistance = 15;

    private bool atacked = false;

    //void Awake()
    //{
    //    instance = this;
    //}

    new void Start()
    {
        maxHealth = 1000;
        base.Start();
        agent.speed = 1f;
        CurrentState = EnemyStates[0];
        Cooldown = CurrentState.CooldownMax;
        points = 1000;
    }

    new void Update()
    {
        base.Update();
        Atack();
        ApproachPlayer();
    }

    public override void ReceiveDamage(float damage)
    {
        health -= damage;

        if (health <= maxHealth * 0.75f && health > maxHealth * 0.5f)
        {
            CurrentState = EnemyStates[1];
        }
        else if (health <= maxHealth * 0.5f && health > maxHealth * 0.25f)
        {
            CurrentState = EnemyStates[2];
        }
        else if (health <= maxHealth * 0.25f && health > 0)
        {
            CurrentState = EnemyStates[3];
        }
        else if (health <= 0)
        {
            CurrentState = EnemyStates[4];
        }
    }

    void Atack()
    {
        // Patron de ataque: Si le jugador esta a playerDistance unidades, el cooldown ha llegado a zero y dependiendo del bool. El enemigo ataca o salta
        if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && Cooldown <= 0 && !atacked)
        {
            // Activamos animacion, reseteamos el cooldown y cambiamos el bool para que el siguiente ataque sea el salto
            animator.SetTrigger("Atack");
            Cooldown = CurrentState.CooldownMax;
            atacked = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance && Cooldown <= 0 && atacked)
        {
            // Activamos animacion, reseteamos el cooldown y cambiamos el bool para que el siguiente ataque sea el ataque
            animator.SetTrigger("Jump");
            Cooldown = CurrentState.CooldownMax;
            atacked = false;
        }
        else
        {
            // Debug.Log(Vector3.Distance(transform.position, player.transform.position));
            Cooldown -= Time.deltaTime;
        }
    }

    void ApproachPlayer()
    {
        // Si el jugador esta a mas de playerDistance unidades, el enemigo alcelera, si no, vuelve a su velocidad normal 
        if (Vector3.Distance(transform.position, player.transform.position) > playerDistance)
        {
            agent.speed = CurrentState.MaxVel;
        }
        else
        {
            agent.speed = CurrentState.MinVel;
        }
    }

    //protected override void Dead()
    //{
    //    base.Dead();
    //    if (isDead)
    //    {
    //        // Inmoviliza al enemigo y activa la animación de muerte
    //        agent.speed = 0;
    //        animator.SetTrigger("Dead");
    //    }
    //}

    // Funciones de animaciones
    // Desactiva al enemigo
    void DeadAnim()
    {
        this.gameObject.SetActive(false);
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
