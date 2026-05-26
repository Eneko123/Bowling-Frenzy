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

    //imagenesbalas
    [SerializeField] public Sprite ImagenIVForUpgrate;
    [SerializeField] public Sprite ImagenVForUpgrate;
    [SerializeField] public Sprite ImagenVIForUpgrate;
    [SerializeField] public Sprite ImagenVIIForUpgrate;

    internal int currentPositionHability = 0;
    public static GenerateBullet instance;
    public Sprite ExploIcon;
    public Sprite PiercingIcon;
    public Sprite SlowingIcon;
    //Una vez tengamos los iconos: descomentar los public sprites, bajar hasta AddSpecial y sustituir el ".color = Color.red/green/blue" por ".sprite = ExploIcon/PiercingIcon/SlowingIcon"
    public List<SpecialBullets> specialBullets = new List<SpecialBullets>();
    //Si quereis hacer esto privado, buscad la forma de anadirlos por script
    public List<Image> weaponSlots = new List<Image>();

    public List<GameObject> ListBullets { get => listBullets; }
    public List<GameObject> ListExplosiveBullets { get => listExplosiveBullets; }
    public List<GameObject> ListPiercingBullets { get => listPiercingBullets; }
    public List<GameObject> ListSlowingBullets { get => listSlowingBullets; }
    public List<Image> WeaponSlots { get => weaponSlots; }

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
        Debug.Log(weaponSlots.Count);
        //Si quereis anadir los WeaponSlots desde script y no desde el inspector hay que crear un algoritmo para que detecte
        //Las imagenes del Weapon 
        WeaponSlots[0].enabled = false;
        WeaponSlots[1].enabled = false;
        WeaponSlots[2].enabled = false;
        GameObject tmpBullet;
        GameObject tmpExplosive;
        GameObject tmpPiercing;
        GameObject tmpSlowing;
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
    //private void Update()
    //{
    //    //(Esto es temporal, cuando se implementen las mejoras se debe de hacer que el primer elemento
    //    // del array se meta el prefab del tipo de bala que haya conseguido
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {

    //        AddSpecial(SpecialBullets.Piercing);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.X))
    //    {

    //        AddSpecial(SpecialBullets.Explosive);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.C))
    //    {

    //        AddSpecial(SpecialBullets.Slowing);
    //    }
    //}
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
    internal SpecialBullets ChangeHability(SpecialBullets hability)
    {
        SpecialBullets sP = hability;
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
            default:
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
                WeaponSlots[slotIndex].enabled = true;
                WeaponSlots[slotIndex].sprite = ExploIcon;
                break;
            case SpecialBullets.Piercing:
                WeaponSlots[slotIndex].enabled = true;
                WeaponSlots[slotIndex].sprite = PiercingIcon;
                break;
            case SpecialBullets.Slowing:
                WeaponSlots[slotIndex].enabled = true;
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
            upgrateimage = option.upgrateimage,
            apply = option.apply,
            weight = option.weight
        };
    }

    public PowerUps.UpgradeOption GetBulletOption(int tier)
    {
        // Construye la lista de opciones disponibles
        // La bala normal siempre esta, las especiales solo si estan desbloqueadas
        var available = new List<string>();
        available.Add("Normal");

        foreach (SpecialBullets b in GenerateBullet.instance.specialBullets)
            available.Add(b.ToString());

        string chosen = available[Random.Range(0, available.Count)];

        if (chosen == "Normal")
            return new UpgradeOption
            {
                label = $" Danio normal +{powerUps.normalDmgM[tier]}",
                upgrateimage = ImagenIVForUpgrate,
                apply = () =>
                {
                    for (int i = 0; i < listBullets.Count; i++)
                    {
                        int captured = i;
                        listBullets[captured].GetComponentInChildren<NormalBulletBehaviour>().SetDamage(
                            listBullets[captured].GetComponentInChildren<NormalBulletBehaviour>().GetDamage() + powerUps.normalDmgM[tier]);
                    }
                }
            };

        SpecialBullets chosenSpecial = (SpecialBullets)System.Enum.Parse(typeof(SpecialBullets), chosen);

        return chosenSpecial switch
        {
            SpecialBullets.Explosive => new UpgradeOption
            {
                label = $" Explosivo: danio +{powerUps.specialDmgM[tier]} / area +{Mathf.RoundToInt((powerUps.explScaleM[tier] - 1) * 100)}%",
                upgrateimage = ImagenVForUpgrate,
                apply = () =>
                {
                    for (int i = 0; i < listExplosiveBullets.Count; i++)
                    {
                        int captured = i;
                        ExplosiveBulletBehaviour explosiveBehaviour = listExplosiveBullets[captured].GetComponentInChildren<ExplosiveBulletBehaviour>();

                        if (explosiveBehaviour != null)
                        {
                            // Actualiza el daioo
                            explosiveBehaviour.SetDamage(explosiveBehaviour.GetDamage() + powerUps.specialDmgM[tier]);

                            // Actualiza la escala de explosioin
                            explosiveBehaviour.SetExplosionScale(explosiveBehaviour.GetExplosionScale() * powerUps.explScaleM[tier]);
                        }
                    }
                }
            },
            SpecialBullets.Piercing => new UpgradeOption
            {
                label = $" Perforante: danio +{powerUps.specialDmgM[tier]} / cantidad de perforacion +{powerUps.pierceVals[tier]}",
                upgrateimage = ImagenVIForUpgrate,
                apply = () =>
                {
                    for (int i = 0; i < listPiercingBullets.Count; i++)
                    {
                        int captured = i;
                        PierceBullet pierceBullet = listPiercingBullets[captured].GetComponentInChildren<PierceBullet>();

                        if (pierceBullet != null)
                        {
                            // Actualiza el daño
                            pierceBullet.SetDamage(pierceBullet.GetDamage() + powerUps.specialDmgM[tier]);

                            // Actualiza la cantidad de perforación
                            pierceBullet.SetMaxPierce(pierceBullet.GetMaxPierce() + powerUps.pierceVals[tier]);
                            pierceBullet.SetPierce(pierceBullet.GetPierce() + powerUps.pierceVals[tier]);
                        }
                    }
                }
            },
            SpecialBullets.Slowing => new UpgradeOption
            {
                label = $" Ralentizadora: danio +{powerUps.specialDmgM[tier]} / duracion +{powerUps.slowTimeVals[tier]}s",
                upgrateimage = ImagenVIIForUpgrate,
                apply = () =>
                {
                    for (int i = 0; i < listSlowingBullets.Count; i++)
                    {
                        int captured = i;
                        SlowBullet slowBullet = listSlowingBullets[captured].GetComponentInChildren<SlowBullet>();

                        if (slowBullet != null)
                        {
                            // Actualiza el daño
                            slowBullet.SetDamage(slowBullet.GetDamage() + powerUps.specialDmgM[tier]);

                            // Actualiza la duración del slow
                            slowBullet.SetSlowDuration(slowBullet.GetSlowDuration() + powerUps.slowTimeVals[tier]);
                        }
                    }
                }
            },
            _ => ConvertToUpgradeOption(powerUps.GetPlayerOption(0))
        };
    }

    #endregion
}
