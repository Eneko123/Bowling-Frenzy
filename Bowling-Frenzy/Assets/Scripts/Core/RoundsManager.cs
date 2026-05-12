using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
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
    public PowerUps powerUps;

    public State[] states;
    private State currentState;
    private int currentRound;
    private float currentTime;
    // private bool isRoundActive;
    private bool isRoundFinished;
    private bool isFinalRound;
    private bool timeBetwineRounds = false;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI RoundManagerText;

    [SerializeField] GameObject timerObject;

    [SerializeField] UIGameplay uiGameplay;

    [SerializeField] GameObject boss; // prefab del boss colocado en la escena pero desactivado

    public static RoundsManager instance;

    void Awake()
    {
        instance = this;
    }

    public int CurrentRound => currentRound;

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
        timerObject.SetActive(true);
        boss.SetActive(false);
        currentState = states[0];
        currentTime = states[0].timeMax;
        currentRound = 0;
        RoundManagerText.text = $"Round {currentRound + 1}";
        timerText.text = "00:" + (currentTime%60).ToString("00");
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
        SpawnEnemis();
    }

    void Timer()
    {
        if(!isFinalRound)
        {
            if (timeBetwineRounds)
            {
                StartNextRound();
            }
            else
            {
                if (currentRound == states.Length - 1)
                {
                    isFinalRound = true;
                    timerObject.SetActive(false);
                    RoundManagerText.gameObject.SetActive(false);
                    boss.GetComponent<BoloEBoos>().DificultySystem(); // Llamamos a esta funcion en la ultima ronda para que funcione
                    boss.SetActive(true);
                }
                else if (currentTime >= 0)
                {
                    currentTime -= Time.deltaTime;
                    timerText.text = "00:" + (currentTime % 60).ToString("00");
                }
                else
                {
                    isRoundFinished = true;
                    timeBetwineRounds = true;
                    uiGameplay.isUpgradeMenuOpen = true;
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
    }

    public void StartNextRoundButton()
    {
        if (!isRoundFinished)
        {
            currentRound += 1;
            AplyDificultySystemToEnemis();
            uiGameplay.UpdateRoundText();
            currentState = states[currentRound];
            currentTime = currentState.timeMax;
            timerText.text = "00 :" + (currentTime % 60).ToString("00");
            timeBetwineRounds = false;
            RoundManagerText.text = $"Round {currentRound + 1}";
            uiGameplay.isUpgradeMenuOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void AplyDificultySystemToEnemis()
    { // Accede a los enemigos dentro de los spawnpoints y les aplica las mejoras
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var enemies = spawnPoints[i].GetComponentsInChildren<EnemyBase>(true);
            Debug.Log(enemies.Length);
            for (int j = 0; j < enemies.Length; j++)
            {
                enemies[j].DificultySystem();
            }
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

    public bool GetFinalRound() => isFinalRound;

    void DesactiveSweeper()
    {
        sweeper.SetActive(false);
    }
}
