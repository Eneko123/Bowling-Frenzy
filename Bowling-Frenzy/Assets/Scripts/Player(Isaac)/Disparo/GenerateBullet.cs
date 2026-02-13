using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBullet : MonoBehaviour
{
    int numberOfBullets = 10;
    [SerializeField] GameObject bullet;
    List<GameObject> listBullets = new List<GameObject>() { };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject tmpBullet;
        for (int i = 0; i < numberOfBullets; i++)
        {
            tmpBullet = Instantiate(bullet);
            tmpBullet.SetActive(false);
            listBullets.Add(tmpBullet);
        }
    }
    public GameObject GetBullets()
    {
        foreach (GameObject b in listBullets)
        {
            if (!b.activeInHierarchy)
            {
                b.SetActive(true);
                return b;
            }
        }
        GameObject tmpBullet;
        tmpBullet = Instantiate(bullet);
        tmpBullet.SetActive(true);
        return tmpBullet;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            GetBullets();
        }
    }

}
