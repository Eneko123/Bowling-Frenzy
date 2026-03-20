using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Enemys
{
    public List<GameObject> BoloEPool;
    public int BoloEAmount;
    public GameObject BoloE;

    public Enemys(List<GameObject> boloEPool, int boloEAmount, GameObject boloE)
    {
        BoloEPool = boloEPool;
        BoloEAmount = boloEAmount;
        BoloE = boloE;
    }
}

public class Spawner : MonoBehaviour
{
    public Enemys[] EnemysType;

    public int ActiveEnemies1 => EnemysType[0].BoloEPool.FindAll(e => e.activeSelf).Count;
    public int ActiveEnemies2 => EnemysType[1].BoloEPool.FindAll(e => e.activeSelf).Count;
    public int ActiveEnemies3 => EnemysType[1].BoloEPool.FindAll(e => e.activeSelf).Count;

    //private List<GameObject> BoloBoss;
    public GameObject BoloBoss;

    //void Start()
    //{
    //    BoloBoss.Add(BoloEBoos.instance.gameObject);
    //}

    void Awake()
    {
        AddBoloToPool(EnemysType[0].BoloEAmount, EnemysType[0].BoloE, EnemysType[0].BoloEPool);
        AddBoloToPool(EnemysType[1].BoloEAmount, EnemysType[1].BoloE, EnemysType[1].BoloEPool);
        AddBoloToPool(EnemysType[2].BoloEAmount, EnemysType[2].BoloE, EnemysType[2].BoloEPool);
    }

    void Update()
    {
    }

    void AddBoloToPool(int amount, GameObject bolo, List<GameObject> pool)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject enemy = Instantiate(bolo);
            enemy.SetActive(false);
            enemy.transform.parent = this.transform;
            pool.Add(enemy);
        }
    }

    public void SpawnBolo(GameObject bolo, List<GameObject> pool)
    {
        // Buscar uno inactivo en el pool
        GameObject enemy = pool.Find(e => !e.activeSelf);

        // Si no hay, expandir el pool
        if (enemy == null)
        {
            AddBoloToPool(1, bolo, pool);
            enemy = pool[pool.Count - 1];
        }

        enemy.transform.position = this.transform.position;
        enemy.SetActive(true);
    }

    public void SpawnBoss(Transform Spawn)
    {
        GameObject boss = Instantiate(BoloBoss);
        boss.transform.position = Spawn.transform.position;
        boss.SetActive(true);
    }
}
