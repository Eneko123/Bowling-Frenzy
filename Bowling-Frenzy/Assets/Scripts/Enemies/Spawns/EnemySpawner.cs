using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;

public class EnemySpawner : MonoBehaviour
{
    //variables
    public GameObject[] enemyPrefab;
    private float spawnCooldown = 2f;
    private float timer;
    public int idx;
    public int maxActiveEnemies = 3;
    public int currentEnemies;


    //metodos
    void Start() { idx = -1; }
    void Update()
    {
        if (currentEnemies < maxActiveEnemies)
        {
            if (ShouldSpawn())
            {
                idx++;
                if (!enemyPrefab[idx].activeSelf)
                {
                    enemyPrefab[idx].SetActive(true);
                    currentEnemies++;

                    Debug.Log("entra correctamente a sacar enemigos");
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            enemyPrefab[idx].SetActive(false);
            currentEnemies--;
            idx--;
        }

        if(idx < -1)
        {
            idx = -1;
        }
    }
    private bool ShouldSpawn()
    {
        timer += Time.deltaTime;
        if (timer >= spawnCooldown)
        {
            timer = 0f;
            return true;
        }
        return false;
    }
}