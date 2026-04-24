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

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI timePlayedText;

    void Start()
    {
        SetupButtonListeners();
        DisplayGameOverStats();
        Time.timeScale = 1f; // Asegurar que el tiempo esta normal
    }

    void SetupButtonListeners()
    {
        if (retryButton) retryButton.onClick.AddListener(OnRetryClicked);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);
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
            enemiesKilledText.text = "Enemigos eliminados: --";
        }

        // if (accuracyText)
        // {
        //     // accuracyText.text = $"Precisión: {GameManager.Instance.accuracy}%";
        //     accuracyText.text = "Precisión: --%";
        // }

        if (timePlayedText)
        {
            // timePlayedText.text = $"Tiempo jugado: {FormatTime(GameManager.Instance.timePlayed)}";
            timePlayedText.text = "Tiempo jugado: --:--";
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