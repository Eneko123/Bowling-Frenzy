using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PowerUps : MonoBehaviour
{
    EnemyBase enemis;
    [SerializeField] GenerateBullet generateBullet;
    [SerializeField] RoundsManager rounds;
    [SerializeField] HealthUI playerHealth;

    public GameObject upgradePanel;
    public Button[] buttons;           // 3 botones
    public TextMeshProUGUI[] btnLabels; // texto de cada boton
    public Image[] btnImage; //imagen para cada mejora/texto

    //imagenes
    [SerializeField] public Sprite ImagenIForUpgrate;
    [SerializeField] public Sprite ImagenIIForUpgrate;
    [SerializeField] public Sprite ImagenIIIForUpgrate;

    [SerializeField] public Sprite ImagenVForUpgrate;
    [SerializeField] public Sprite ImagenVIForUpgrate;
    [SerializeField] public Sprite ImagenVIIForUpgrate;

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
    public struct UpgradeOption
    {
        public string label;
        public Action apply;
        public Sprite upgrateimage;
        public int weight; // mayor = mas comun
    }

    // variables de mejora
    // Player
    int[] healthSum = { 10, 25, 50 };
    float[] defenseBons = { 0.05f, 0.1f, 0.2f };
    float[] speedBons = { 1f, 2f, 5f };
    int[] weights = { 60, 30, 10 }; // comun, raro, epico

    // Balas
    internal float[] normalDmgM = { 15f, 25f, 40f };
    internal float[] specialDmgM = { 10f, 20f, 30f };
    internal float[] explScaleM = { 1.2f, 1.4f, 1.6f };
    internal int[] pierceVals = { 2, 4, 8 };
    internal float[] slowTimeVals = { 1f, 2f, 3f };
    // Estado interno
    private Action[] pendingActions = new Action[3];

    public void ShowUpgradesForRound(int roundIndex)
    {
        Cat[] cats = roundIndex < roundTable.Length
            ? roundTable[roundIndex]
            : new[] { Cat.B, Cat.B, Cat.B };

        // Lista para rastrear las opciones ya generadas
        var usedOptions = new HashSet<string>();

        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeOption opt;
            int attempts = 0;

            // Intenta obtener una opcion unica (maximo 20 intentos para evitar bucle infinito)
            do
            {
                opt = GetRandomOption(cats[i]);
                attempts++;
            }
            while (usedOptions.Contains(opt.label) && attempts < 30);

            // Marca esta opcion como usada
            usedOptions.Add(opt.label);

            btnLabels[i].text = opt.label;
            btnImage[i].sprite = opt.upgrateimage;//para que le de la imagen
            Action captured = opt.apply;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => ApplyAndClose(captured));
        }

        upgradePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ApplyAndClose(Action actionToRun)
    {
        MainCharacter.Instance.ResetShootingState();
        actionToRun.Invoke();
        UIGameplay.uI.InitializeSpecialCooldowns();
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        if (rounds != null) rounds.StartNextRoundButton();
        if (playerHealth != null) playerHealth.UpdateHealth(MainCharacter.Instance.GetCurrentHealth(), MainCharacter.Instance.GetHealthMax());
    }

    UpgradeOption GetRandomOption(Cat cat)
    {
        int tier = WeightedRandom(); // 0 = comun, 1 = raro, 2 = epico

        return cat switch
        {
            Cat.P => GetPlayerOption(tier),
            Cat.B => generateBullet.GetBulletOption(tier),
            Cat.U => GetUnlockOption(),
            _ => GetPlayerOption(tier)
        };
    }

    public UpgradeOption GetPlayerOption(int tier)
    {
        // Elige aleatoriamente entre las 3 stats del jugador
        int stat = UnityEngine.Random.Range(0, 3);
        return stat switch
        {
            0 => new UpgradeOption
            {
                label = $" Vida +{healthSum[tier]}",
                upgrateimage = ImagenIForUpgrate,
                apply = () =>
                {
                    MainCharacter.Instance.SetHealthMax(MainCharacter.Instance.GetHealthMax() + healthSum[tier]);
                    // Añade también la vida al current health
                    MainCharacter.Instance.SetCurrentHealth(MainCharacter.Instance.GetCurrentHealth() + healthSum[tier]);
                },
                weight = weights[tier]
            },
            1 => new UpgradeOption
            {
                label = $" Defensa +{defenseBons[tier]}",
                upgrateimage = ImagenIIForUpgrate,
                apply = () => MainCharacter.Instance.SetDefense(MainCharacter.Instance.GetDefense() + defenseBons[tier]),
                weight = weights[tier]
            },
            _ => new UpgradeOption
            {
                label = $" Velocidad +{speedBons[tier]}",
                upgrateimage = ImagenIIIForUpgrate,
                apply = () => MainCharacter.Instance.SetSpeed(MainCharacter.Instance.GetSpeed() + speedBons[tier]),
                weight = weights[tier]
            }
        };
    }



    UpgradeOption GetUnlockOption()
    {
        // Busca balas aun no desbloqueadas
        var locked = new List<SpecialBullets>();
        foreach (SpecialBullets b in System.Enum.GetValues(typeof(SpecialBullets)))
            if (!GenerateBullet.instance.specialBullets.Contains(b))
                locked.Add(b);

        //if (locked.Count == 0)
        //    return GetBulletOption(WeightedRandom()); // si ya estan todas, da mejora de danio

        SpecialBullets toUnlock = SpecialBullets.None;
        while (toUnlock == SpecialBullets.None)
        {
            // evita desbloquear "None" si por alguna razon esta en la lista
            toUnlock = locked[UnityEngine.Random.Range(0, locked.Count)];
        }
        //necesito aca
        Sprite image = toUnlock switch
        {
            SpecialBullets.Explosive => ImagenVForUpgrate,
            SpecialBullets.Piercing => ImagenVIForUpgrate,
            SpecialBullets.Slowing => ImagenVIIForUpgrate,
            _ => null
        };
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
            upgrateimage = image,
            apply = () => GenerateBullet.instance.AddSpecial(toUnlock)
        };
    }

    int WeightedRandom()
    {
        int roll = UnityEngine.Random.Range(1, 101); // 1 a 100
        if (roll <= 10) return 2;        // epico   10%
        if (roll <= 40) return 1;        // raro    30%
        return 0;                        // comun   60%
    }
}
