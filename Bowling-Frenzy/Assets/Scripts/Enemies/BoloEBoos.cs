using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    [SerializeField] RoundsManager roundsManager;

    new void Start()
    {
        CurrentState = EnemyStates[0];
        Cooldown = CurrentState.CooldownMax;
    }

    new void Update()
    {
        base.Update();
        Atack();
        ApproachPlayer();
    }

    private void OnEnable()
    {
        DificultySystem();
    }

    public override void DificultySystem()
    {
        // Solo se llama en la ultima ronda
 
        
            // Evitar errores si el GameManager no esta presente
            if (GameManager.Instance == null) { return; }

            switch (GameManager.Instance.difficulty)
            {
                case Difficulty.Easy:
                    // Sin cambios, los valores quedan como estan
                    break;

                case Difficulty.Normal:
                    maxHealth += 250f;
                    damage += 10;
                    for (int i = 0; i < EnemyStates.Length - 1; i++)
                    {
                        EnemyStates[i].CooldownMax -= 2.5f;
                        EnemyStates[i].MaxVel += 2f;
                        EnemyStates[i].MinVel += 2f;
                    }
                    break;

                case Difficulty.Hard:
                    maxHealth += 500f;
                    damage += 30;
                    for (int i = 0; i < EnemyStates.Length - 1; i++)
                    {
                        EnemyStates[i].CooldownMax -= 4.5f;
                        EnemyStates[i].MaxVel += 3f;
                        EnemyStates[i].MinVel += 3f;
                    }
                    break;
            }

            // Actualizar la vida actual y el danio actual
            health = maxHealth;
            currentDamage = damage;
        
    }

    public override void ReceiveDamage(float damage, bool isBarredora)
    {
        health -= damage;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        else
        {
            pulseCoroutine = StartCoroutine(ColorPulse());
        }

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
            Dead(isBarredora);
        }
        if (Combos.Instance != null && !isBarredora)
            Combos.Instance.IncrementCombo();
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

    protected override void Dead(bool isBarredoraOn)
    {
        if (health <= 0)
        {
            damage = 0;
            // Inmoviliza al enemigo y activa la animacion de muerte
            agent.speed = 0;
            animator.SetTrigger("Dead");
        }
        if (health <= 0 && !isBarredoraOn)
        {
            GivePoints(points);
        }
    }

    // Funciones de animaciones
    // Desactiva al enemigo
    void DeadAnim()
    {
        this.gameObject.SetActive(false);
        UIGameplay.uI.UpdateMaxScore();
        GameManager.Instance.winornot = true;
        SceneManager.LoadScene("Game_Over");
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