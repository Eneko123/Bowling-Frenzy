using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;

public class EnemySpawner : MonoBehaviour
{
    //variables
    public GameObject[] enemyPrefab;
    private float spawnCooldown = 2f;
    private float timer;
    public int idx = 0;
    public int maxActiveEnemies = 3;
    public int currentEnemies;


    //metodos
    void Start() { currentEnemies = 0; }
    void Update()
    {
        if (currentEnemies < maxActiveEnemies)
        {
            if (ShouldSpawn())
            {
                if (!enemyPrefab[idx].activeSelf)
                {
                    enemyPrefab[idx].SetActive(true);
                    currentEnemies++;
                    idx++;

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

        if(idx < 0)
        {
            idx = 0;
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