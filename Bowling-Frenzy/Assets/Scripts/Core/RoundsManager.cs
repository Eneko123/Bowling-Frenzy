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
        public int E4;
    }

    public GameObject sweeper;
    public State[] states;
    private State currentState;
    private int currentRound;
    private float currentTime;
    private bool isRoundActive;
    private bool isRoundFinished;
    private bool isFinalRound;
    private bool timeBetwineRounds = false;

    void Start()
    {
        currentTime = states[0].timeMax;
        currentRound = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
    }

    void Timer()
    {
        if (timeBetwineRounds)
        {
            return;
        }
        else
        {
            if (currentTime >= 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                sweeper.SetActive(true);
                isRoundActive = false;
                isRoundFinished = true;
                currentRound += 1;
                if (currentRound == states.Length - 1)
                {
                    isFinalRound = true;
                    currentState = states[currentRound];
                    currentTime = 3600;
                    Debug.Log("Final round reached!");
                    return;
                }
                else
                {
                    isRoundActive = true;
                    isRoundFinished = false;
                    currentState = states[currentRound];
                    currentTime = currentState.timeMax;
                    Debug.Log("Round " + (currentRound) + " started! " + currentTime);
                }
            }
        }
    }

    void DesactiveSweeper()
    {
        sweeper.SetActive(false);
    }
}
