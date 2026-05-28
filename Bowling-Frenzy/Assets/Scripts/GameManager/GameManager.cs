using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public Difficulty difficulty = Difficulty.Normal;
    public string selectedLevel = "";

    [Header("Player Puntuation")]
    public int playerScore = 0;
    public int bolo1Score = 0;
    public int bolo2Score = 0;
    public int bolo3Score = 0;
    public float playedTime = 0;

    [Header("Records")]
    public int bestPlayerScore = 0;
    public int bestBolo1Score = 0;
    public int bestBolo2Score = 0;
    public int bestBolo3Score = 0;

    [Header("Settings")]
    public float masterVolume = 1f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 0.8f;

    public bool winornot;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        playerScore = 0;
    }

    void Start()
    {
        LoadSettings();
        LoadRecords();
    }

    #region Settings

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            AudioListener.volume = masterVolume;
        }
    }

    public void ResetGameState()
    {
        playerScore = 0;
        bolo1Score = 0;
        bolo2Score = 0;
        bolo3Score = 0;
        playedTime = 0;
        selectedLevel = "";
    }

    #endregion

    #region Records

    void LoadRecords()
    {
        bestPlayerScore = PlayerPrefs.GetInt("Record_Score", 0);
        bestBolo1Score = PlayerPrefs.GetInt("Record_Bolo1", 0);
        bestBolo2Score = PlayerPrefs.GetInt("Record_Bolo2", 0);
        bestBolo3Score = PlayerPrefs.GetInt("Record_Bolo3", 0);
    }

    // Compara las puntuaciones actuales con los records guardados.
    // Guarda los nuevos records en PlayerPrefs si se superan.
    // Devuelve que records se han batido.
    public RecordResult CheckAndSaveRecords()
    {
        RecordResult result = new RecordResult();

        if (playerScore > bestPlayerScore) { bestPlayerScore = playerScore; PlayerPrefs.SetInt("Record_Score", bestPlayerScore); result.newBestScore = true; }
        if (bolo1Score > bestBolo1Score) { bestBolo1Score = bolo1Score; PlayerPrefs.SetInt("Record_Bolo1", bestBolo1Score); result.newBestBolo1 = true; }
        if (bolo2Score > bestBolo2Score) { bestBolo2Score = bolo2Score; PlayerPrefs.SetInt("Record_Bolo2", bestBolo2Score); result.newBestBolo2 = true; }
        if (bolo3Score > bestBolo3Score) { bestBolo3Score = bolo3Score; PlayerPrefs.SetInt("Record_Bolo3", bestBolo3Score); result.newBestBolo3 = true; }

        PlayerPrefs.Save();
        return result;
    }

    #endregion
}

public struct RecordResult
{
    public bool newBestScore;
    public bool newBestBolo1;
    public bool newBestBolo2;
    public bool newBestBolo3;
}
