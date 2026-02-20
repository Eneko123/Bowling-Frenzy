using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBullet : MonoBehaviour
{
    [SerializeField]int numberOfBullets = 10;
    [SerializeField] int numberOfExplosiveBullets = 5;
    [SerializeField] NormalBulletBehaviour bullet;
    [SerializeField] ExplosiveBulletBehaviour heavyBullet;
    List<NormalBulletBehaviour> listBullets = new List<NormalBulletBehaviour>() { };
    List<ExplosiveBulletBehaviour> listHeavyBullets = new List<ExplosiveBulletBehaviour>() { };

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
        NormalBulletBehaviour tmpBullet;
        ExplosiveBulletBehaviour tmpExplosive;
        for (int i = 0; i < numberOfBullets; i++)
        {
            tmpBullet = Instantiate(bullet);
            tmpBullet.gameObject.SetActive(true);
            listBullets.Add(tmpBullet);
        }
        for(int i = 0; i < numberOfExplosiveBullets; i++)
        {
            tmpExplosive = Instantiate(heavyBullet);
            tmpExplosive.gameObject.SetActive(true);
            listHeavyBullets.Add(tmpExplosive);
        }
    }
    public NormalBulletBehaviour GetBullets()
    {
        foreach (NormalBulletBehaviour b in listBullets)
        {
            if (!b.gameObject.activeInHierarchy)
            {
                b.gameObject.SetActive(true);
                return b;
            }
        }
        NormalBulletBehaviour tmpBullet;
        tmpBullet = Instantiate(bullet);
        tmpBullet.gameObject.SetActive(true);
        return tmpBullet;

    }
    public ExplosiveBulletBehaviour GetExplosiveBullets()
    {
        foreach (ExplosiveBulletBehaviour e in listHeavyBullets)
        {
            if (!e.gameObject.activeInHierarchy)
            {
                e.gameObject.SetActive(true);
                return e;
            }
        }
        ExplosiveBulletBehaviour tmpExplosive;
        tmpExplosive = Instantiate(heavyBullet);
        tmpExplosive.gameObject.SetActive(true);
        return tmpExplosive;
    }
}
