using UnityEngine;

public class PowerUps : MonoBehaviour
{
    MainCharacter player;
    EnemyBase enemis;

    private void Awake()
    {
        player = MainCharacter.Instance;
    }

    void UpHealth()
    {
        float newMaxHealth = player.GetHealthMax() * 1.1f;
        player.SetHealthMax(newMaxHealth);
    }

    void UpDefense()
    {
        float newDefense = player.GetDefense() + 0.05f;
        player.SetDefense(newDefense);
    }

    void UpSpeed()
    {
        float newSpeed = player.GetSpeed() + 1.5f;
        player.SetSpeed(newSpeed);
    }

    void UnlockExplosiveBullet()
    {
        if (!GenerateBullet.instance.specialBullets.Contains(SpecialBullets.Explosive))
        {
            GenerateBullet.instance.specialBullets.Add(SpecialBullets.Explosive);
        }
    }

    void UnlockPiercingBullet()
    {
        if (!GenerateBullet.instance.specialBullets.Contains(SpecialBullets.Piercing))
        {
            GenerateBullet.instance.specialBullets.Add(SpecialBullets.Piercing);
        }
    }

    void UnlockSlowingBullet()
    {
        if (!GenerateBullet.instance.specialBullets.Contains(SpecialBullets.Slowing))
        {
            GenerateBullet.instance.specialBullets.Add(SpecialBullets.Slowing);
        }
    }
}
