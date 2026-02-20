using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{ 
    //metodos
    void Start() { }
    void Update() { }
    //
    public static EnemyRegistry Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private HashSet<EnemyBase> activeEnemies = new();
    public int ActiveCount => activeEnemies.Count;
    //public void Register(EnemyBase enemy) => activeEnemies.Add(enemy);
    //public void Unregister(EnemyBase enemy) => activeEnemies.Remove(enemy);
}