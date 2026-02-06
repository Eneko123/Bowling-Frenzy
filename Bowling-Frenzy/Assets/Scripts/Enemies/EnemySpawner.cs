using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //variables
    public int maxActiveEnemies = 20;
    private int currentEnemies => EnemyRegistry.Instance.ActiveCount;


    //metodos
    void Start()
    {
        
    }
    void Update()
    {
       if (currentEnemies < maxActiveEnemies && ShouldSpawn())
        { SpawnEnemy(); }
    }

}
