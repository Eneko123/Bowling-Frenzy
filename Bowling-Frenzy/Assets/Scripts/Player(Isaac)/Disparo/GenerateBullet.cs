using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBullet : MonoBehaviour
{
    [SerializeField]int numberOfBullets = 10;
    [SerializeField] NormalBulletBehaviour bullet;
    List<NormalBulletBehaviour> listBullets = new List<NormalBulletBehaviour>() { };

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
        for (int i = 0; i < numberOfBullets; i++)
        {
            tmpBullet = Instantiate(bullet);
            tmpBullet.gameObject.SetActive(true);
            listBullets.Add(tmpBullet);
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
}
