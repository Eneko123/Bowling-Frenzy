using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
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
    [SerializeField] RoundsManager rounds;
    [SerializeField] PlayerHealth playerHealth;

    public GameObject upgradePanel;
    public Button[] buttons;           // 3 botones
    public TextMeshProUGUI[] btnLabels; // texto de cada boton

    // Cada fila = una ronda. Cada columna = categoria del boton.
    // P = PlayerStat, B = BulletDamage, U = UnlockBullet
    private enum Cat { P, B, U }

    private readonly Cat[][] roundTable = new Cat[][]
    {
        new[]{ Cat.P, Cat.P, Cat.P },          // Ronda 1
        new[]{ Cat.U, Cat.U, Cat.U },          // Ronda 2
        new[]{ Cat.B, Cat.B, Cat.P },          // Ronda 3
        new[]{ Cat.B, Cat.B, Cat.P },          // Ronda 4
        new[]{ Cat.U, Cat.U, Cat.P },          // Ronda 5
        new[]{ Cat.B, Cat.B, Cat.B },          // Ronda 6
        new[]{ Cat.B, Cat.B, Cat.B },          // Ronda 7
        new[]{ Cat.U, Cat.P, Cat.P },          // Ronda 8
        new[]{ Cat.B, Cat.B, Cat.B },          // Ronda 9
    };

    // Cada upgrade es (multiplicador/valor, peso de rareza)
    private struct UpgradeOption
    {
        public string label;
        public System.Action apply;
        public int weight; // mayor = mas comun
    }

    // variables de mejora
    // Player
    float[] healthMults = { 1.1f, 1.2f, 1.5f };
    float[] defenseBons = { 0.05f, 0.1f, 0.2f };
    float[] speedBons = { 1f, 2f, 5f };
    int[] weights = { 60, 30, 10 }; // comun, raro, epico

    // Balas
    float[] normalDmgM = { 1.2f, 1.4f, 1.6f };
    float[] specialDmgM = { 1.1f, 1.2f, 1.3f };
    float[] explScaleM = { 1.2f, 1.4f, 1.6f };
    int[] pierceVals = { 2, 4, 8 };
    float[] slowTimeVals = { 1f, 2f, 3f };

    // Estado interno
    private System.Action[] pendingActions = new System.Action[3];

    void Awake()
    {
        player = MainCharacter.Instance;
    }

    public void ShowUpgradesForRound(int roundIndex)
    {
        Cat[] cats = roundIndex < roundTable.Length
            ? roundTable[roundIndex]
            : new[] { Cat.B, Cat.B, Cat.B };

        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeOption opt = GetRandomOption(cats[i]);
            pendingActions[i] = opt.apply;
            btnLabels[i].text = opt.label;

            int captured = i; // captura para el lambda
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => ApplyAndClose(captured));
        }

        upgradePanel.SetActive(true);
        Time.timeScale = 0f; // pausa el juego
    }

    void ApplyAndClose(int index)
    {
        pendingActions[index]?.Invoke();
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        rounds.StartNextRoundButton();
        playerHealth.UpdateHealth(player.GetCurrentHealth(), player.GetHealthMax());
    }

    UpgradeOption GetRandomOption(Cat cat)
    {
        int tier = WeightedRandom(); // 0 = comun, 1 = raro, 2 = epico

        return cat switch
        {
            Cat.P => GetPlayerOption(tier),
            Cat.B => GetBulletOption(tier),
            Cat.U => GetUnlockOption(),
            _ => GetPlayerOption(tier)
        };
    }

    UpgradeOption GetPlayerOption(int tier)
    {
        // Elige aleatoriamente entre las 3 stats del jugador
        int stat = Random.Range(0, 3);
        return stat switch
        {
            0 => new UpgradeOption
            {
                label = $" Vida +{Mathf.RoundToInt((healthMults[tier] - 1) * 100)}%",
                apply = () => player.SetHealthMax(player.GetHealthMax() * healthMults[tier]),
                weight = weights[tier]
            },
            1 => new UpgradeOption
            {
                label = $" Defensa +{defenseBons[tier]}",
                apply = () => player.SetDefense(player.GetDefense() + defenseBons[tier]),
                weight = weights[tier]
            },
            _ => new UpgradeOption
            {
                label = $" Velocidad +{speedBons[tier]}",
                apply = () => player.SetSpeed(player.GetSpeed() + speedBons[tier]),
                weight = weights[tier]
            }
        };
    }

    UpgradeOption GetBulletOption(int tier)
    {
        // Construye la lista de opciones disponibles
        // La bala normal siempre esta, las especiales solo si están desbloqueadas
        var available = new List<string>();
        available.Add("Normal");

        foreach (SpecialBullets b in GenerateBullet.instance.specialBullets)
            available.Add(b.ToString());

        string chosen = available[Random.Range(0, available.Count)];

        if (chosen == "Normal")
            return new UpgradeOption
            {
                label = $" Danio normal +{Mathf.RoundToInt((normalDmgM[tier] - 1) * 100)}%",
                apply = () => normalBullet.SetDamage(normalBullet.GetDamage() * normalDmgM[tier])
            };

        SpecialBullets chosenSpecial = (SpecialBullets)System.Enum.Parse(typeof(SpecialBullets), chosen);

        return chosenSpecial switch
        {
            SpecialBullets.Explosive => new UpgradeOption
            {
                label = $" Explosivo: danio +{Mathf.RoundToInt((specialDmgM[tier] - 1) * 100)}% / area +{Mathf.RoundToInt((explScaleM[tier] - 1) * 100)}%",
                apply = () =>
                {
                    explosiveBullet.SetDamage(explosiveBullet.GetDamage() * specialDmgM[tier]);
                    explosion.SetExposionScale(explosion.GetExposionScale() * explScaleM[tier]);
                }
            },
            SpecialBullets.Piercing => new UpgradeOption
            {
                label = $" Perforante: danio +{Mathf.RoundToInt((specialDmgM[tier] - 1) * 100)}% / cantidad de perforacion +{pierceVals[tier]}",
                apply = () =>
                {
                    pierceBullet.SetDamage(pierceBullet.GetDamage() * specialDmgM[tier]);
                    pierceBullet.SetMaxPierce(pierceBullet.GetMaxPierce() + pierceVals[tier]);
                }
            },
            SpecialBullets.Slowing => new UpgradeOption
            {
                label = $" Ralentizadora: danio +{Mathf.RoundToInt((specialDmgM[tier] - 1) * 100)}% / duracion +{slowTimeVals[tier]}s",
                apply = () => slowBullet.SetDamage(slowBullet.GetDamage() * specialDmgM[tier])
            },
            _ => GetPlayerOption(0)
        };
    }

    UpgradeOption GetUnlockOption()
    {
        // Busca balas aun no desbloqueadas
        var locked = new List<SpecialBullets>();
        foreach (SpecialBullets b in System.Enum.GetValues(typeof(SpecialBullets)))
            if (!GenerateBullet.instance.specialBullets.Contains(b))
                locked.Add(b);

        if (locked.Count == 0)
            return GetBulletOption(WeightedRandom()); // si ya estan todas, da mejora de daño

        SpecialBullets toUnlock = locked[Random.Range(0, locked.Count)];
        string name = toUnlock switch
        {
            SpecialBullets.Explosive => "Bala Explosiva",
            SpecialBullets.Piercing => "Bala Perforante",
            SpecialBullets.Slowing => "Bala Ralentizadora",
            _ => toUnlock.ToString()
        };

        return new UpgradeOption
        {
            label = $" Desbloquear: {name}",
            apply = () => GenerateBullet.instance.specialBullets.Add(toUnlock)
        };
    }

    int WeightedRandom()
    {
        int roll = Random.Range(1, 101); // 1 a 100
        if (roll <= 10) return 2;        // epico   10%
        if (roll <= 40) return 1;        // raro    30%
        return 0;                        // comun   60%
    }
    //private void Update()
    //{
    //    // OnMouseOver cuando le raton este encima de boton
    //    // up1_1.onClick.RemoveAllListeners(); Borrar las funciones del boton cada ronda para que no den fallos
    //}

    //void ChoseUpsPlayer()
    //{
    //    int up1 = Random.Range(1, 10);
    //    int up2 = Random.Range(1, 10);
    //    int up3 = Random.Range(1, 10);

    //    if (up1 == 1)
    //    {
    //        up1_1.onClick.AddListener(() => UpHealth(healthUp3)); 
    //    }
    //    else if (up1 > 1 && up1 < 4)
    //    {
    //        up1_1.onClick.AddListener(() => UpHealth(healthUp2));
    //    }
    //    else if (up1 >= 4 && up1 <= 10)
    //    {
    //        up1_1.onClick.AddListener(() => UpHealth(healthUp1));
    //    }

    //    if (up2 == 1)
    //    {
    //        up2_1.onClick.AddListener(() => UpDefense(defenseUp3));
    //    }
    //    else if (up2 > 1 && up2 < 4)
    //    {
    //        up2_1.onClick.AddListener(() => UpDefense(defenseUp2));
    //    }
    //    else if (up2 >= 4 && up2 <= 10)
    //    {
    //        up2_1.onClick.AddListener(() => UpDefense(defenseUp1));
    //    }

    //    if (up3 == 1)
    //    {
    //        up3_1.onClick.AddListener(() => UpSpeed(speedUp3));
    //    }
    //    else if (up3 > 1 && up3 < 4)
    //    {
    //        up3_1.onClick.AddListener(() => UpSpeed(speedUp2));
    //    }
    //    else if (up3 >= 4 && up3 <= 10)
    //    {
    //        up3_1.onClick.AddListener(() => UpSpeed(speedUp1));
    //    }
    //}

    //void ChoseUpsBullets()
    //{
    //    int up1 = Random.Range(1, 10);
    //    int up2 = Random.Range(1, 10);
    //    int up3 = Random.Range(1, 10);

    //    if (up1 == 1)
    //    {
    //        up1_1.onClick.AddListener(() => UpNormalDamage(normalDamageUp3));
    //    }
    //    else if (up1 > 1 && up1 < 4)
    //    {
    //        up1_1.onClick.AddListener(() => UpNormalDamage(normalDamageUp2));
    //    }
    //    else if (up1 >= 4 && up1 <= 10)
    //    {
    //        up1_1.onClick.AddListener(() => UpNormalDamage(normalDamageUp1));
    //    }

    //    if (up2 == 1)
    //    {
    //        up2_1.onClick.AddListener(() => UpExplosiveDamage(specialDamageUp2, explosibeScaleUp3));
    //    }
    //    else if (up2 > 1 && up2 < 4)
    //    {
    //        up2_1.onClick.AddListener(() => UpExplosiveDamage(specialDamageUp2, explosibeScaleUp2));
    //    }
    //    else if (up2 >= 4 && up2 <= 10)
    //    {
    //        up2_1.onClick.AddListener(() => UpExplosiveDamage(specialDamageUp1, explosibeScaleUp1));
    //    }

    //    if (up3 == 1)
    //    {
    //        up3_1.onClick.AddListener(() => UpPierceDamage(specialDamageUp3, pierceUp3));
    //    }
    //    else if (up3 > 1 && up3 < 4)
    //    {
    //        up3_1.onClick.AddListener(() => UpPierceDamage(specialDamageUp2, pierceUp2));
    //    }
    //    else if (up3 >= 4 && up3 <= 10)
    //    {
    //        up3_1.onClick.AddListener(() => UpPierceDamage(specialDamageUp1, pierceUp1));
    //    }
    //}



    //void UpHealth(float health)
    //{
    //    float newMaxHealth = player.GetHealthMax() * health;
    //    player.SetHealthMax(newMaxHealth);
    //}

    //void UpDefense(float defense)
    //{
    //    float newDefense = player.GetDefense() + defense;
    //    player.SetDefense(newDefense);
    //}

    //void UpSpeed(float speed)
    //{
    //    float newSpeed = player.GetSpeed() + speed;
    //    player.SetSpeed(newSpeed);
    //}

    //void UnlockExplosiveBullet()
    //{
    //    if (!GenerateBullet.instance.specialBullets.Contains(SpecialBullets.Explosive))
    //    {
    //        GenerateBullet.instance.specialBullets.Add(SpecialBullets.Explosive);
    //    }
    //}

    //void UnlockPiercingBullet()
    //{
    //    if (!GenerateBullet.instance.specialBullets.Contains(SpecialBullets.Piercing))
    //    {
    //        GenerateBullet.instance.specialBullets.Add(SpecialBullets.Piercing);
    //    }
    //}

    //void UnlockSlowingBullet()
    //{
    //    if (!GenerateBullet.instance.specialBullets.Contains(SpecialBullets.Slowing))
    //    {
    //        GenerateBullet.instance.specialBullets.Add(SpecialBullets.Slowing);
    //    }
    //}

    //void UpNormalDamage(float normalDamage)
    //{
    //    float newDamage = normalBullet.GetDamage() * normalDamage;
    //    normalBullet.SetDamage(newDamage);
    //}

    //void UpExplosiveDamage(float explosiveDamage, float explosiveScale)
    //{
    //    float newDamage = explosiveBullet.GetDamage() * explosiveDamage;
    //    float newScale = explosion.GetExposionScale() * explosiveScale;
    //    explosiveBullet.SetDamage(newDamage);
    //    explosion.SetExposionScale(newScale);
    //}

    //void UpPierceDamage(float pierceDamage, int pierce)
    //{
    //    float newDamage = pierceBullet.GetDamage() * pierceDamage;
    //    int newMaxPierce = pierceBullet.GetMaxPierce() + pierce;
    //    pierceBullet.SetDamage(newDamage);
    //    pierceBullet.SetMaxPierce(newMaxPierce);
    //}

    //void UpSlowDamage(float slowDamage, float slowTime)
    //{
    //    float newDamage = slowBullet.GetDamage() * slowDamage;
    //    float newSlowTime = enemis.GetSlowTime() + slowTime;
    //    slowBullet.SetDamage(newDamage);
    //    enemis.SetSlowTime(newSlowTime);
    //}
}
