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
    public int playerScore = 0;
    public int bestPlayerScore = 0;

    [Header("Settings")]
    public float masterVolume = 1f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 0.8f;

    void Awake()
    {
        // Singleton pattern - Compatible con tu código original
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
        // Cargar configuración guardada si existe
        LoadSettings();
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
        selectedLevel = "";
    }
    #endregion
}