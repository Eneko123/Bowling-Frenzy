using System.Linq;
using UnityEngine;

public class RoundsManager : MonoBehaviour
{
    [System.Serializable]
    public struct State
    {
        // Duracion de cada ronda
        public float timeMax;
        // Cantidad de enemigos en cada ronda
        public int E1;
        public int E2;
        public int E3;
        public float spawnRateMax;
        public float currentSpawnRate;
    }

    public GameObject sweeper;
    public Spawner[] spawnPoints;

    public State[] states;
    private State currentState;
    private int currentRound;
    private float currentTime;
    // private bool isRoundActive;
    private bool isRoundFinished;
    private bool isFinalRound;
    private bool timeBetwineRounds = false;

    public int TotalActiveEnemies1
    {
        get
        {
            int total = 0;
            foreach (Spawner s in spawnPoints)
                total += s.ActiveEnemies1;
            return total;
        }
    }

    public int TotalActiveEnemies2
    {
        get
        {
            int total = 0;
            foreach (Spawner s in spawnPoints)
                total += s.ActiveEnemies2;
            return total;
        }
    }
    public int TotalActiveEnemies3
    {
        get
        {
            int total = 0;
            foreach (Spawner s in spawnPoints)
                total += s.ActiveEnemies3;
            return total;
        }
    }

    void Start()
    {
        currentState = states[0];
        currentTime = states[0].timeMax;
        currentRound = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
        SpawnEnemis();
    }

    void Timer()
    {
        if (timeBetwineRounds)
        {
            StartNextRound();
        }
        else
        {
            if (currentTime >= 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                isRoundFinished = true;
                timeBetwineRounds = true;
                if (currentRound == states.Length - 1)
                {
                    isFinalRound = true;
                    spawnPoints[spawnPoints.Length-1].SpawnBoss(spawnPoints[spawnPoints.Length-1].transform);
                }
                else
                {
                    return;
                }
            }
        }
    }

    void StartNextRound()
    {
        if (isRoundFinished && !isFinalRound)
        {
            sweeper.SetActive(true);
            isRoundFinished = false;
        }

        if (Input.GetKeyDown(KeyCode.P) && !isRoundFinished)
        {
            currentRound += 1;
            currentState = states[currentRound];
            currentTime = currentState.timeMax;
            timeBetwineRounds = false;
        }
    }

    void SpawnEnemis()
    {
        currentState.currentSpawnRate -= Time.deltaTime;
        if (currentState.currentSpawnRate <= 0 && !timeBetwineRounds)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (currentState.E1 > TotalActiveEnemies1)
                {
                    spawnPoints[i].SpawnBolo(spawnPoints[i].EnemysType[0].BoloE, spawnPoints[i].EnemysType[0].BoloEPool);
                }
                if (currentState.E2 > TotalActiveEnemies2)
                {
                    spawnPoints[i].SpawnBolo(spawnPoints[i].EnemysType[1].BoloE, spawnPoints[i].EnemysType[1].BoloEPool);
                }
                if (currentState.E3 > TotalActiveEnemies3)
                {
                    spawnPoints[i].SpawnBolo(spawnPoints[i].EnemysType[2].BoloE, spawnPoints[i].EnemysType[2].BoloEPool);
                }
            }
            currentState.currentSpawnRate = currentState.spawnRateMax;
        }
    }

    void DesactiveSweeper()
    {
        sweeper.SetActive(false);
    }
}
