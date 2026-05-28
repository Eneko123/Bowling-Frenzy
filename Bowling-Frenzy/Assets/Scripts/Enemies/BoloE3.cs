using UnityEngine;

public class BoloE3 : EnemyBase
{
    new void Awake()
    {
        maxHealth = 40;
        base.Awake();
        agent.speed = 4f;
        points = 50;
        damage = 5f;
    }

    protected override void Dead(bool isBarredora)
    {
        base.Dead(isBarredora);
        if (!isBarredora)
        {
            GameManager.Instance.bolo3Score += 1;
        }
    }

    public void DificultySystem()
    {
        // Evitar errores si el GameManager no esta presente
        if (GameManager.Instance == null) { return; }

        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.Easy:
                // Sin cambios, los valores quedan como estan
                break;

            case Difficulty.Normal:
                // Incremento ADITIVO por ronda (SUMA)
                // Usa los valores BASE y suma el incremento por ronda
                maxHealth += 10f;      // +2 de vida por ronda
                damage += 2f;             // +0.5 de danio por ronda
                agent.speed += 0.1f;      // +0.2 de velocidad por ronda
                break;

            case Difficulty.Hard:
                maxHealth += 30f;
                damage += 3f;
                agent.speed += 0.3f;
                break;
        }

        // Actualizar la vida actual y el danio actual
        health = maxHealth;
        currentDamage = damage;
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
