using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIGameOver : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalRoundText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Panels")]
    [SerializeField] private GameObject panelWin;
    [SerializeField] private GameObject panelLoose;

    [Header("Buttons Win")]
    [SerializeField] private Button retryButtonW;
    [SerializeField] private Button mainMenuButtonW;
    [SerializeField] private Button quitButtonW;

    [Header("Buttons Loose")]
    [SerializeField] private Button retryButtonL;
    [SerializeField] private Button mainMenuButtonL;
    [SerializeField] private Button quitButtonL;

    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI timePlayedText;

    void Start()
    {
        if(GameManager.Instance.winornot)
        {
            AudioManager.Instance.PlayMusic("Victoria");
        }
        else
        {
            AudioManager.Instance.PlayMusic("Derrota");
        }
            SetupButtonListeners();
        DisplayGameOverStats();
        Time.timeScale = 1f; // Asegurar que el tiempo esta normal
    }

    public void Update()
    {
        WinOrNot();
    }

    public void WinOrNot()
    {
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.Instance.winornot)
        {
            panelWin.SetActive(true);
            panelLoose.SetActive(false);
        }
        else 
        {
            panelWin.SetActive(false);
            panelLoose.SetActive(true);
        }
    }

    void SetupButtonListeners()
    {
        if (retryButtonW) retryButtonW.onClick.AddListener(OnRetryClicked);
        if (mainMenuButtonW) mainMenuButtonW.onClick.AddListener(OnMainMenuClicked);
        if (quitButtonW) quitButtonW.onClick.AddListener(OnQuitClicked);

        if (retryButtonL) retryButtonL.onClick.AddListener(OnRetryClicked);
        if (mainMenuButtonL) mainMenuButtonL.onClick.AddListener(OnMainMenuClicked);
        if (quitButtonL) quitButtonL.onClick.AddListener(OnQuitClicked);
    }

    void DisplayGameOverStats()
    {
        if (GameManager.Instance == null) return;

        // Titulo
        if (gameOverText)
        {
            gameOverText.text = "GAME OVER";
        }

        // Puntuacion final
        if (finalScoreText)
        {
            finalScoreText.text = $"Puntuación Final: {GameManager.Instance.playerScore}";
        }

        // Ronda alcanzada
        if (finalRoundText && RoundsManager.instance != null)
        {
            finalRoundText.text = $"Ronda Alcanzada: {RoundsManager.instance.CurrentRound + 1}";
        }

        // Mensaje personalizado basado en el rendimiento
        if (messageText)
        {
            messageText.text = GetPerformanceMessage();
        }

        // Requieren un sistema de tracking
        if (enemiesKilledText)
        {
            // enemiesKilledText.text = $"Enemigos eliminados: {GameManager.Instance.totalEnemiesKilled}";
            enemiesKilledText.text = "Bolos normales eliminados: " + GameManager.Instance.bolo1Score + 
                "\nBolos bebes eliminados: " + GameManager.Instance.bolo2Score + 
                "\nBolos macarras eliminados: " + GameManager.Instance.bolo3Score;
        }

        // if (accuracyText)
        // {
        //     // accuracyText.text = $"Precisión: {GameManager.Instance.accuracy}%";
        //     accuracyText.text = "Precisión: --%";
        // }

        if (timePlayedText)
        {
            // timePlayedText.text = $"Tiempo jugado: {FormatTime(GameManager.Instance.timePlayed)}";
            timePlayedText.text = "Tiempo jugado: " + FormatTime(GameManager.Instance.playedTime);
        }
    }

    string GetPerformanceMessage()
    {
        int score = GameManager.Instance.playerScore;
        int round = RoundsManager.instance?.CurrentRound ?? 0;

        // Mensajes basados en rendimiento
        if (round >= 9 || score >= 10000)
        {
            return "¡Increíble! ¡Eres un maestro!";
        }
        else if (round >= 6 || score >= 5000)
        {
            return "¡Excelente trabajo! Muy impresionante.";
        }
        else if (round >= 4 || score >= 2000)
        {
            return "¡Buen intento! Sigue así.";
        }
        else if (round >= 2 || score >= 500)
        {
            return "No está mal. ¡Puedes hacerlo mejor!";
        }
        else
        {
            return "¡No te rindas! La práctica hace al maestro.";
        }
    }

    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }

    #region Button Callbacks
    void OnRetryClicked()
    {
        Debug.Log("Retry clicked");

        // Recargar el nivel actual
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.selectedLevel))
        {
            GameManager.Instance.playerScore = 0;
            GameManager.Instance.bolo1Score = 0;
            GameManager.Instance.bolo2Score = 0;
            GameManager.Instance.bolo3Score = 0;
            SceneManager.LoadScene(GameManager.Instance.selectedLevel);
        }
        else
        {
            // Si no hay nivel guardado, volver a seleccion de niveles
            SceneManager.LoadScene("Menu_Levels");
        }
    }

    void OnMainMenuClicked()
    {
        Debug.Log("Main menu clicked");

        // Resetear estado del juego
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameState();
        }

        SceneManager.LoadScene("Menu_Main");
    }

    void OnQuitClicked()
    {
        Debug.Log("Quit clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}