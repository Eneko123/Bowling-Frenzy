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

    internal NormalBulletBehaviour[] listOfHabilities = new NormalBulletBehaviour[3];
    internal int currentPositionHability = 0;
    public static GenerateBullet instance;
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
    internal GameObject SelectTheSpecial(int index)
    {
        switch (index)
        {
            case 0:
                return GetExplosiveBullets();
            case 1:
                return GetPiercingBullets();
            case 2:
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
    internal void ChangeHability(int hability)
    {
        currentPositionHability = hability;
    }
}
