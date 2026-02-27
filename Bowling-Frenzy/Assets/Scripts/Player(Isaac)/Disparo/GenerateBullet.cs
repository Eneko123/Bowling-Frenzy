using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBullet : MonoBehaviour
{
    [SerializeField]int numberOfBullets = 10;
    [SerializeField] int numberOfExplosiveBullets = 5;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject heavyBullet;
    List<GameObject> listBullets = new List<GameObject>() { };
    List<GameObject> listHeavyBullets = new List<GameObject>() { };

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
        for (int i = 0; i < numberOfBullets; i++)
        {
            tmpBullet = Instantiate(bullet);
            tmpBullet.gameObject.SetActive(false);
            listBullets.Add(tmpBullet);
        }
        for(int i = 0; i < numberOfExplosiveBullets; i++)
        {
            tmpExplosive = Instantiate(heavyBullet);
            tmpExplosive.gameObject.SetActive(false);
            listHeavyBullets.Add(tmpExplosive);
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
}
