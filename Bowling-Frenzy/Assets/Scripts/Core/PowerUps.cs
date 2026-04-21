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
    int[] healthSum = { 10, 25, 50 };
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
        playerHealth.UpdateHealth(player.GetCurrentHealth(), player.GetHealthMax()); // al current health no le afecta la mejora, pero al maximo si, asi que hay que actualizar la barra de vida
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
                label = $" Vida +{healthSum[tier]}",
                apply = () => player.SetHealthMax(player.GetHealthMax() + healthSum[tier]),
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
            apply = () => GenerateBullet.instance.AddSpecial(toUnlock)
        };
    }

    int WeightedRandom()
    {
        int roll = Random.Range(1, 101); // 1 a 100
        if (roll <= 10) return 2;        // epico   10%
        if (roll <= 40) return 1;        // raro    30%
        return 0;                        // comun   60%
    }
}
