using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //variables
    public int maxActiveEnemies = 20;
    public GameObject enemyPrefab;
    private float spawnCooldown = 2f;
    private float timer;
    private int currentEnemies => EnemyRegistry.Instance.ActiveCount;


    //metodos
    void Start() { }
    void Update()
    {
        if (EnemyRegistry.Instance == null) return;

        if (currentEnemies < maxActiveEnemies && ShouldSpawn())
        { SpawnEnemy(); }
    }

    //
   
    private void SpawnEnemy()
    {
        // Instanciar el prefab
        GameObject go = Instantiate(enemyPrefab, transform.position, Quaternion.identity); 

        // Obtener el componente EnemyBase del prefab instanciado
        EnemyBase enemy = go.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            EnemyRegistry.Instance.Register(enemy);
        }
        else
        {
            Debug.LogError("El prefab no tiene EnemyBase");
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