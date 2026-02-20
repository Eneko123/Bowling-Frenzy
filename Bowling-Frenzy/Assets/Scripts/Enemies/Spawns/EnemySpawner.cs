using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    //variables
    public int maxActiveEnemies = 3;
    public GameObject[] enemyPrefab;
    private float spawnCooldown = 2f;
    private float timer;
    public int currentEnemies;
    public int mi;
    //mi solo esta para ser un tipo de "desactivador", cuando se implemente el daño y la "muerte de los enemigos"
    //se cambiara esa logica

    //metodos
    void Start() { currentEnemies = 0; }
    void Update()
    {
        if (currentEnemies < maxActiveEnemies && ShouldSpawn())
        {
            Debug.Log("entra correctamente a sacar enemigos");
            //para empezar la corrutina hay qye usar
            // StartCoroutine(metodo ie);
            StartCoroutine(SpawnCorrutine());
        }

        /*
        if (currentEnemies == enemyPrefab.Length)
        {
            mi = 1;
        }
        if (mi == 1)
        {
            StartCoroutine(UnSpawnCorrutine());
        }
        */
    }

    //
    IEnumerator SpawnCorrutine()
    {
        for (int i = 0; i < enemyPrefab.Length; i++)
        {
            //al inicio todos los prefabs estan desactivados, o deben estar
            if (!enemyPrefab[i].activeSelf)
            {
                enemyPrefab[i].SetActive(true);
                currentEnemies++;
            }
            yield return new WaitForSeconds(spawnCooldown);
            //el wait tiene que estar despues por que sino, espera y aparecen varios enemigos al momento
        }
    }
    IEnumerator UnSpawnCorrutine()
    {
        for (int i = 0; i < enemyPrefab.Length; i++)
        {
            //esto se hace para ir borrandolos
            if (enemyPrefab[i].activeSelf)
            {
                enemyPrefab[i].SetActive(false);
                currentEnemies--;
            }
            yield return new WaitForSeconds(3f);
            //este metodo se desaparecera con la implementacion de la muerte del enemigo

            if (currentEnemies < 0)
            {
                currentEnemies = 0;
                mi = 0;
            }
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