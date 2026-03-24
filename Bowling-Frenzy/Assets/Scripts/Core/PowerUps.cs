using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{
    MainCharacter player;
    EnemyBase enemis;
    NormalBulletBehaviour normalBullet;
    ExplosiveBulletBehaviour explosiveBullet;
    PierceBullet pierceBullet;
    SlowBullet slowBullet;
    Explosion explosion;
    RoundsManager rounds;

    public Button up1_1;
    public Button up2_1;
    public Button up3_1;

    public Button up1_2;
    public Button up2_2;

    public Button up1_3;


    // public OnButtonClick

    private void Awake()
    {
        player = MainCharacter.Instance;
    }

    private void Update()
    {
        // OnMouseOver cuando le raton este encima de boton
        // up1_1.onClick.RemoveAllListeners(); Borrar las funciones del boton cada ronda para que no den fallos
    }

    void ChoseUpsPlayer()
    {
        int up1 = Random.Range(1, 10);
        int up2 = Random.Range(1, 10);
        int up3 = Random.Range(1, 10);

        if (up1 == 1)
        {
            up1_1.onClick.AddListener(() => UpHealth3()); 
        }
        else if (up1 > 1 && up1 < 4)
        {
            up1_1.onClick.AddListener(() => UpHealth2());
        }
        else if (up1 > 4 && up1 <= 10)
        {
            up1_1.onClick.AddListener(() => UpHealth1());
        }

        if (up2 == 1)
        {
            up2_1.onClick.AddListener(() => UpHealth3());
        }
        else if (up2 > 1 && up2 < 4)
        {
            up2_1.onClick.AddListener(() => UpHealth2());
        }
        else if (up2 > 4 && up2 <= 10)
        {
            up2_1.onClick.AddListener(() => UpDefense1());
        }

        if (up3 == 1)
        {
            up3_1.onClick.AddListener(() => UpSpeed3());
        }
        else if (up3 > 1 && up3 < 4)
        {
            up3_1.onClick.AddListener(() => UpSpeed2());
        }
        else if (up3 > 4 && up3 <= 10)
        {
            up3_1.onClick.AddListener(() => UpSpeed1());
        }
    }

    void ChoseUpsBullets()
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

    void UpNormalDamage1()
    {
        float newDamage = normalBullet.GetDamage() * 1.2f;
        normalBullet.SetDamage(newDamage);
    }
    void UpNormalDamage2()
    {
        float newDamage = normalBullet.GetDamage() * 1.4f;
        normalBullet.SetDamage(newDamage);
    }
    void UpNormalDamage3()
    {
        float newDamage = normalBullet.GetDamage() * 1.6f;
        normalBullet.SetDamage(newDamage);
    }

    void UpExplosiveDamage1()
    {
        float newDamage = explosiveBullet.GetDamage() * 1.1f;
        float newScale = explosion.GetExposionScale() * 1.2f;
        explosiveBullet.SetDamage(newDamage);
        explosion.SetExposionScale(newScale);
    }
    void UpExplosiveDamage2()
    {
        float newDamage = explosiveBullet.GetDamage() * 1.2f;
        float newScale = explosion.GetExposionScale() * 1.4f;
        explosiveBullet.SetDamage(newDamage);
        explosion.SetExposionScale(newScale);
    }
    void UpExplosiveDamage3()
    {
        float newDamage = explosiveBullet.GetDamage() * 1.3f;
        explosiveBullet.SetDamage(newDamage);
        float newScale = explosion.GetExposionScale() * 1.6f;
        explosion.SetExposionScale(newScale);
    }

    void UpPierceDamage1()
    {
        float newDamage = pierceBullet.GetDamage() * 1.1f;
        int newMaxPierce = pierceBullet.GetMaxPierce() + 2;
        pierceBullet.SetDamage(newDamage);
        pierceBullet.SetMaxPierce(newMaxPierce);
    }
    void UpPierceDamage2()
    {
        float newDamage = pierceBullet.GetDamage() * 1.2f;
        int newMaxPierce = pierceBullet.GetMaxPierce() + 4;
        pierceBullet.SetDamage(newDamage);
        pierceBullet.SetMaxPierce(newMaxPierce);
    }
    void UpPierceDamage3()
    {
        float newDamage = pierceBullet.GetDamage() * 1.3f;
        int newMaxPierce = pierceBullet.GetMaxPierce() + 8;
        pierceBullet.SetDamage(newDamage);
        pierceBullet.SetMaxPierce(newMaxPierce);
    }

    void UpSlowDamage1()
    {
        float newDamage = slowBullet.GetDamage() * 1.1f;
        float newSlowTime = enemis.GetSlowTime() + 1f;
        slowBullet.SetDamage(newDamage);
        enemis.SetSlowTime(newSlowTime);
    }
    void UpSlowDamage2()
    {
        float newDamage = slowBullet.GetDamage() * 1.2f;
        float newSlowTime = enemis.GetSlowTime() + 2f;
        slowBullet.SetDamage(newDamage);
        enemis.SetSlowTime(newSlowTime);
    }
    void UpSlowDamage3()
    {
        float newDamage = slowBullet.GetDamage() * 1.3f;
        float newSlowTime = enemis.GetSlowTime() + 3f;
        slowBullet.SetDamage(newDamage);
        enemis.SetSlowTime(newSlowTime);
    }
}
