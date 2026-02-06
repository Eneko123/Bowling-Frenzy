using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    //variables
    public static EnemyRegistry Instance { get; private set; }
    private HashSet<EnemyBase> activeEnemies = new();
    //metodos
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    //
}
