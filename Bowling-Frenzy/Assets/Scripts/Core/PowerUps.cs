using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;

public class PowerUps : MonoBehaviour
{
    MainCharacter player;
    EnemyBase enemis;
    NormalBulletBehaviour normalBullet;
    ExplosiveBulletBehaviour explosiveBullet;
    PierceBullet pierceBullet;
    SlowBullet slowBullet;

    private void Awake()
    {
        player = MainCharacter.Instance;
    }

    private void Update()
    {
        
    }

    void ChoseUps()
    {
        int up1 = Random.Range(1, 10);
        int up2 = Random.Range(1, 10);
        int up3 = Random.Range(1, 10);

        if (up1 == 1)
        {
            UpHealth3();
        }
        else if (up1 > 1 && up1 < 4)
        {
            UpHealth2();
        }
        else if (up1 > 4 && up1 <= 10)
        { 
            UpHealth1();
        }

        if (up2 == 1)
        {
            UpDefense3();
        }
        else if (up2 > 1 && up2 < 4)
        {
            UpDefense2();
        }
        else if (up2 > 4 && up2 <= 10)
        {
            UpDefense1();
        }

        if (up3 == 1)
        {
            UpSpeed3();
        }
        else if (up3 > 1 && up3 < 4)
        {
            UpSpeed2();
        }
        else if (up3 > 4 && up3 <= 10)
        {
            UpSpeed1();
        }
    }

    void UpHealth1()
    {
        float newMaxHealth = player.GetHealthMax() * 1.1f;
        player.SetHealthMax(newMaxHealth);
    }

    void UpHealth2()
    {
        float newMaxHealth = player.GetHealthMax() * 1.2f;
        player.SetHealthMax(newMaxHealth);
    }

    void UpHealth3()
    {
        float newMaxHealth = player.GetHealthMax() * 1.5f;
        player.SetHealthMax(newMaxHealth);
    }

    void UpDefense1()
    {
        float newDefense = player.GetDefense() + 0.05f;
        player.SetDefense(newDefense);
    }

    void UpDefense2()
    {
        float newDefense = player.GetDefense() + 0.1f;
        player.SetDefense(newDefense);
    }

    void UpDefense3()
    {
        float newDefense = player.GetDefense() + 0.2f;
        player.SetDefense(newDefense);
    }

    void UpSpeed1()
    {
        float newSpeed = player.GetSpeed() + 1f;
        player.SetSpeed(newSpeed);
    }

    void UpSpeed2()
    {
        float newSpeed = player.GetSpeed() + 2f;
        player.SetSpeed(newSpeed);
    }

    void UpSpeed3()
    {
        float newSpeed = player.GetSpeed() + 5f;
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

    void UpNormalDamage()
    {
        float newDamage = normalBullet.GetDamage() * 1.1f;
        normalBullet.SetDamage(newDamage);
    }

    void UpExplosiveDamage()
    {
        float newDamage = explosiveBullet.GetDamage() * 1.1f;
        explosiveBullet.SetDamage(newDamage);
    }

    void UpPierceDamage()
    {
        float newDamage = pierceBullet.GetDamage() * 1.1f;
        pierceBullet.SetDamage(newDamage);
    }

    void UpSlowDamage()
    {
        float newDamage = slowBullet.GetDamage() * 1.1f;
        slowBullet.SetDamage(newDamage);
    }
}
