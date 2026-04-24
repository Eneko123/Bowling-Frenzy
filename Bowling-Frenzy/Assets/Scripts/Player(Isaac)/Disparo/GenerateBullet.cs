using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PowerUps;



public class GenerateBullet : MonoBehaviour
{
    [SerializeField] int numberOfBullets = 10;
    [SerializeField] int numberOfExplosiveBullets = 5;
    [SerializeField] int numberOfPiercingBullets = 5;
    [SerializeField] int numberOfSlowingBullets = 5;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject explosiveBullet;
    [SerializeField] GameObject piercingBullet;
    [SerializeField] GameObject slowingBullet;
    Explosion explosion;
    List<GameObject> listBullets = new List<GameObject>() { };
    List<GameObject> listExplosiveBullets = new List<GameObject>() { };
    List<GameObject> listPiercingBullets = new List<GameObject>() { };
    List<GameObject> listSlowingBullets = new List<GameObject>() { };
    [SerializeField] PowerUps powerUps;

    internal int currentPositionHability = 0;
    public static GenerateBullet instance;
    public Sprite ExploIcon;
    public Sprite PiercingIcon;
    public Sprite SlowingIcon;
    //Una vez tengamos los iconos: descomentar los public sprites, bajar hasta AddSpecial y sustituir el ".color = Color.red/green/blue" por ".sprite = ExploIcon/PiercingIcon/SlowingIcon"
    public List<SpecialBullets> specialBullets = new List<SpecialBullets>();
    public List<Image> WeaponSlots = new List<Image>();
    // Primer elemento: imagen del slot de bala explosiva
    // Segundo elemento: imagen del slot de bala perforante
    // Tercer elemento: imagen del slot de bala ralentizadora
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject tmpBullet;
        GameObject tmpExplosive;
        GameObject tmpPiercing;
        GameObject tmpSlowing;
        explosion = explosiveBullet.GetComponent<Explosion>();
        for (int i = 0; i < numberOfBullets; i++)
        {
            tmpBullet = Instantiate(bullet);
            tmpBullet.gameObject.SetActive(false);
            listBullets.Add(tmpBullet);
        }
        for (int i = 0; i < numberOfExplosiveBullets; i++)
        {
            tmpExplosive = Instantiate(explosiveBullet);
            tmpExplosive.gameObject.SetActive(false);
            listExplosiveBullets.Add(tmpExplosive);
        }
        for (int i = 0; i < numberOfPiercingBullets; i++)
        {
            tmpPiercing = Instantiate(piercingBullet);
            tmpPiercing.gameObject.SetActive(false);
            listPiercingBullets.Add(tmpPiercing);
        }
        for (int i = 0; i < numberOfSlowingBullets; i++)
        {
            tmpSlowing = Instantiate(slowingBullet);
            tmpSlowing.gameObject.SetActive(false);
            listSlowingBullets.Add(tmpSlowing);
        }
    }
    private void Update()
    {
        //(Esto es temporal, cuando se implementen las mejoras se debe de hacer que el primer elemento
        // del array se meta el prefab del tipo de bala que haya conseguido
        if (Input.GetKeyDown(KeyCode.Z))
        {
        
            AddSpecial(SpecialBullets.Piercing);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
        
            AddSpecial(SpecialBullets.Explosive);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
        
            AddSpecial(SpecialBullets.Slowing);
        }
    }
    public GameObject GetBullets()
    {
        foreach (GameObject b in listBullets)
        {
            if (!b.gameObject.activeInHierarchy)
            {
                b.gameObject.SetActive(true);
                return b;
            }
        }
        GameObject tmpBullet;
        tmpBullet = Instantiate(bullet);
        tmpBullet.gameObject.SetActive(true);
        return tmpBullet;

    }
    internal GameObject SelectTheSpecial(SpecialBullets selectedSpecial)
    {


        if (selectedSpecial == SpecialBullets.Explosive)
        {
            return GetExplosiveBullets();
        }
        else if (selectedSpecial == SpecialBullets.Piercing)
        {
            return GetPiercingBullets();
        }
        else if (selectedSpecial == SpecialBullets.Slowing)
        {
            return GetSlowingBullets();
        }
        return null;
    }
    public GameObject GetExplosiveBullets()
    {
        foreach (GameObject e in listExplosiveBullets)
        {
            if (!e.gameObject.activeInHierarchy)
            {
                e.gameObject.SetActive(true);
                Debug.Log(5);
                return e;
            }
        }
        GameObject tmpExplosive;
        tmpExplosive = Instantiate(explosiveBullet);
        tmpExplosive.gameObject.SetActive(true);
        return tmpExplosive;
    }
    public GameObject GetPiercingBullets()
    {
        foreach (GameObject e in listPiercingBullets)
        {
            if (!e.gameObject.activeInHierarchy)
            {
                e.gameObject.SetActive(true);
                Debug.Log(5);
                return e;
            }
        }
        GameObject tmpPiercing;
        tmpPiercing = Instantiate(piercingBullet);
        tmpPiercing.gameObject.SetActive(false);
        return tmpPiercing;
    }
    public GameObject GetSlowingBullets()
    {
        foreach (GameObject e in listSlowingBullets)
        {
            if (!e.gameObject.activeInHierarchy)
            {
                e.gameObject.SetActive(true);
                Debug.Log(5);
                return e;
            }
        }
        GameObject tmpSlowing;
        tmpSlowing = Instantiate(slowingBullet);
        tmpSlowing.gameObject.SetActive(false);
        return tmpSlowing;
    }
    internal SpecialBullets ChangeHability(int hability)
    {
        if (specialBullets.Count == 0)
        {
            return SpecialBullets.None;
        }
        SpecialBullets sP = specialBullets[hability];
        switch (sP)
        {
            case SpecialBullets.Explosive:
                sP = SpecialBullets.Explosive;
                break;
            case SpecialBullets.Piercing:
                sP = SpecialBullets.Piercing;
                break;
            case SpecialBullets.Slowing:
                sP = SpecialBullets.Slowing;
                break;

        }
        return sP;
    }
    public void AddSpecial(SpecialBullets SB)
    {
        specialBullets.Add(SB);
        Debug.Log(SB);

        // Verifica que hay suficientes slots antes de acceder
        int slotIndex = specialBullets.Count - 1;

        //if (slotIndex >= WeaponSlots.Count)
        //{
        //    Debug.LogError($"No hay suficientes WeaponSlots! Necesitas al menos {slotIndex + 1} slots en el Inspector.");
        //    return;
        //}

        switch (SB)
        {
            case SpecialBullets.Explosive:
                WeaponSlots[slotIndex].sprite = ExploIcon;

                break;
            case SpecialBullets.Piercing:
                WeaponSlots[slotIndex].sprite = PiercingIcon;

                break;
            case SpecialBullets.Slowing:
                WeaponSlots[slotIndex].sprite = SlowingIcon;

                break;
        }
    }

    #region Update Mejoras balas

    private UpgradeOption ConvertToUpgradeOption(PowerUps.UpgradeOption option)
    {
        return new UpgradeOption
        {
            label = option.label,
            apply = option.apply,
            weight = option.weight
        };
    }

    public UpgradeOption GetBulletOption(int tier)
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
                label = $" Danio normal +{Mathf.RoundToInt((powerUps.normalDmgM[tier] - 1) * 100)}%",
                apply = () => bullet.GetComponent<NormalBulletBehaviour>().SetDamage(bullet.GetComponent<NormalBulletBehaviour>().GetDamage() * powerUps.normalDmgM[tier])
            };

        SpecialBullets chosenSpecial = (SpecialBullets)System.Enum.Parse(typeof(SpecialBullets), chosen);

        return chosenSpecial switch
        {
            SpecialBullets.Explosive => new UpgradeOption
            {
                label = $" Explosivo: danio +{Mathf.RoundToInt((powerUps.specialDmgM[tier] - 1) * 100)}% / area +{Mathf.RoundToInt((powerUps.explScaleM[tier] - 1) * 100)}%",
                apply = () =>
                {
                    explosiveBullet.GetComponent<ExplosiveBulletBehaviour>().SetDamage(explosiveBullet.GetComponent<ExplosiveBulletBehaviour>().GetDamage() * powerUps.specialDmgM[tier]);
                    explosion.SetExposionScale(explosion.GetExposionScale() * powerUps.explScaleM[tier]);
                }
            },
            SpecialBullets.Piercing => new UpgradeOption
            {
                label = $" Perforante: danio +{Mathf.RoundToInt((powerUps.specialDmgM[tier] - 1) * 100)}% / cantidad de perforacion +{powerUps.pierceVals[tier]}",
                apply = () =>
                {
                    piercingBullet.GetComponent<PierceBullet>().SetDamage(piercingBullet.GetComponent<PierceBullet>().GetDamage() * powerUps.specialDmgM[tier]);
                    piercingBullet.GetComponent<PierceBullet>().SetMaxPierce(piercingBullet.GetComponent<PierceBullet>().GetMaxPierce() + powerUps.pierceVals[tier]);
                }
            },
            SpecialBullets.Slowing => new UpgradeOption
            {
                label = $" Ralentizadora: danio +{Mathf.RoundToInt((powerUps.specialDmgM[tier] - 1) * 100)}% / duracion +{powerUps.slowTimeVals[tier]}s",
                apply = () => slowingBullet.GetComponent<SlowBullet>().SetDamage(slowingBullet.GetComponent<SlowBullet>().GetDamage() * powerUps.specialDmgM[tier])
            },
            _ => ConvertToUpgradeOption(powerUps.GetPlayerOption(0))
        };
    }

    #endregion
}
