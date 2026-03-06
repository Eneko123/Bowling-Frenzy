using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBullet : MonoBehaviour
{
    [SerializeField] int numberOfBullets = 10;
    [SerializeField] int numberOfExplosiveBullets = 5;
    [SerializeField] int numberOfPiercingBullets = 5;
    [SerializeField] int numberOfSlowingBullets = 5;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject heavyBullet;
    [SerializeField] GameObject piercingBullet;
    [SerializeField] GameObject slowingBullet;
    List<GameObject> listBullets = new List<GameObject>() { };
    List<GameObject> listHeavyBullets = new List<GameObject>() { };
    List<GameObject> listPiercingBullets = new List<GameObject>() { };
    List<GameObject> listSlowingBullets = new List<GameObject>() { };

    internal int currentPositionHability = 0;
    public static GenerateBullet instance;

    public List<SpecialBullets> specialBullets = new List<SpecialBullets>();
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
        for (int i = 0; i < numberOfBullets; i++)
        {
            tmpBullet = Instantiate(bullet);
            tmpBullet.gameObject.SetActive(false);
            listBullets.Add(tmpBullet);
        }
        for (int i = 0; i < numberOfExplosiveBullets; i++)
        {
            tmpExplosive = Instantiate(heavyBullet);
            tmpExplosive.gameObject.SetActive(false);
            listHeavyBullets.Add(tmpExplosive);
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
            specialBullets.Add(SpecialBullets.Piercing);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            specialBullets.Add(SpecialBullets.Explosive);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            specialBullets.Add(SpecialBullets.Slowing);
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
        foreach (GameObject e in listHeavyBullets)
        {
            if (!e.gameObject.activeInHierarchy)
            {
                e.gameObject.SetActive(true);
                Debug.Log(5);
                return e;
            }
        }
        GameObject tmpExplosive;
        tmpExplosive = Instantiate(heavyBullet);
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
}
