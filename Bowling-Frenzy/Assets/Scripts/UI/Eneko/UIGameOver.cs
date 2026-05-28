using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIGameOver : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalRoundText;
    //[SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI finalScoreTextLose;
    [SerializeField] private TextMeshProUGUI finalRoundTextLose;
    [SerializeField] private TextMeshProUGUI messageTextLose;

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
    [SerializeField] private TextMeshProUGUI enemiesKilledTextLose;
    [SerializeField] private TextMeshProUGUI timePlayedTextLose;

    [Header("Nuevo Record - Textos en escena")]
    // Deben empezar DESACTIVADOS en el Inspector.
    [SerializeField] private GameObject recordScore;   // encima de la puntuación final
    [SerializeField] private GameObject recordBolo1;   // encima de bolos normales
    [SerializeField] private GameObject recordBolo2;   // encima de bolos bebés
    [SerializeField] private GameObject recordBolo3;   // encima de bolos macarras
    [SerializeField] private GameObject recordScoreLose;   // encima de la puntuación final
    [SerializeField] private GameObject recordBolo1Lose;   // encima de bolos normales
    [SerializeField] private GameObject recordBolo2Lose;   // encima de bolos bebés
    [SerializeField] private GameObject recordBolo3Lose;   // encima de bolos macarras

    void Start()
    {
        if (GameManager.Instance == null) return;

        RecordResult records = GameManager.Instance.CheckAndSaveRecords();

        // Activar cada texto de record solo si se ha superado
        if (recordScore) recordScore.SetActive(records.newBestScore);
        if (recordBolo1) recordBolo1.SetActive(records.newBestBolo1);
        if (recordBolo2) recordBolo2.SetActive(records.newBestBolo2);
        if (recordBolo3) recordBolo3.SetActive(records.newBestBolo3);

        if (recordScoreLose) recordScoreLose.SetActive(records.newBestScore);
        if (recordBolo1Lose) recordBolo1Lose.SetActive(records.newBestBolo1);
        if (recordBolo2Lose) recordBolo2Lose.SetActive(records.newBestBolo2);
        if (recordBolo3Lose) recordBolo3Lose.SetActive(records.newBestBolo3);

        if (GameManager.Instance.winornot)
            AudioManager.Instance.PlayMusic("Victoria");
        else
            AudioManager.Instance.PlayMusic("Derrota");

        SetupButtonListeners();
        DisplayGameOverStats();
        WinOrNot();

        Time.timeScale = 1f;
    }

    public void WinOrNot()
    {
        Cursor.lockState = CursorLockMode.None;

        bool win = GameManager.Instance != null && GameManager.Instance.winornot;
        if (panelWin) panelWin.SetActive(win);
        if (panelLoose) panelLoose.SetActive(!win);
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

        if (finalScoreText)
            finalScoreText.text = $"Puntuación Final: {GameManager.Instance.playerScore}";

        if (finalRoundText && RoundsManager.instance != null)
            finalRoundText.text = $"Ronda Alcanzada: {RoundsManager.instance.CurrentRound + 1}";

        //if (messageText)
        //    messageText.text = GetPerformanceMessage();

        if (enemiesKilledText)
        {
            enemiesKilledText.text =
                "Bolos normales eliminados: " + GameManager.Instance.bolo1Score +
                "\nBolos bebés eliminados: " + GameManager.Instance.bolo2Score +
                "\nBolos macarras eliminados: " + GameManager.Instance.bolo3Score;
        }

        if (timePlayedText)
            timePlayedText.text = "Tiempo jugado: " + FormatTime(GameManager.Instance.playedTime);

        if (finalScoreTextLose)
            finalScoreTextLose.text = $"Puntuación Final: {GameManager.Instance.playerScore}";

        if (finalRoundTextLose && RoundsManager.instance != null)
            finalRoundTextLose.text = $"Ronda Alcanzada: {RoundsManager.instance.CurrentRound + 1}";

        if (enemiesKilledTextLose)
        {
            enemiesKilledTextLose.text =
                "Bolos normales eliminados: " + GameManager.Instance.bolo1Score +
                "\nBolos bebés eliminados: " + GameManager.Instance.bolo2Score +
                "\nBolos macarras eliminados: " + GameManager.Instance.bolo3Score;
        }

        if (timePlayedTextLose)
            timePlayedTextLose.text = "Tiempo jugado: " + FormatTime(GameManager.Instance.playedTime);
    }

    //string GetPerformanceMessage()
    //{
    //    int score = GameManager.Instance.playerScore;
    //    int round = RoundsManager.instance?.CurrentRound ?? 0;

    //    if (round >= 9 || score >= 10000) return "¡Increíble! ¡Eres un maestro!";
    //    if (round >= 6 || score >= 5000) return "¡Excelente trabajo! Muy impresionante.";
    //    if (round >= 4 || score >= 2000) return "¡Buen intento! Sigue así.";
    //    if (round >= 2 || score >= 500) return "No está mal. ¡Puedes hacerlo mejor!";
    //    return "¡No te rindas! La práctica hace al maestro.";
    //}

    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }

    #region Button Callbacks

    void OnRetryClicked()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.selectedLevel))
        {
            GameManager.Instance.playerScore = 0;
            GameManager.Instance.bolo1Score = 0;
            GameManager.Instance.bolo2Score = 0;
            GameManager.Instance.bolo3Score = 0;
            GameManager.Instance.playedTime = 0;
            SceneManager.LoadScene(GameManager.Instance.selectedLevel);
        }
        else
        {
            SceneManager.LoadScene("Menu_Levels");
        }
    }

    void OnMainMenuClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGameState();

        SceneManager.LoadScene("Menu_Main");
    }

    void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}
