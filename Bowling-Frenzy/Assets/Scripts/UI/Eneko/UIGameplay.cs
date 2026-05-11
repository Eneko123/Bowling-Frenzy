using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIGameplay : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject upgradesPanel; // Este deberia ser el mismo que PowerUps.upgradePanel

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    internal bool isPaused = false;
    internal bool isUpgradeMenuOpen = false;

    public static UIGameplay uI;
    Combos comboManager;
    private void Awake()
    {
        if (uI == null)
        {
            uI = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        InitializePanels();
        SetupButtonListeners();
        UpdateRoundText();
        UpdateScoreText();
        comboManager = GetComponentInChildren<Combos>();
    }

    void Update()
    {
        // Manejar pausa con ESC (solo si no hay menu de mejoras abierto)
        if (Input.GetKeyDown(KeyCode.Escape) && !isUpgradeMenuOpen)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void InitializePanels()
    {
        if (hudPanel) hudPanel.SetActive(true);
        if (pausePanel) pausePanel.SetActive(false);

        // El panel de mejoras es manejado por PowerUps.cs
        // Solo nos aseguramos de trackear su estado
        if (upgradesPanel)
        {
            upgradesPanel.SetActive(false);
        }
    }

    void SetupButtonListeners()
    {
        if (resumeButton) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton) restartButton.onClick.AddListener(RestartLevel);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    #region HUD Updates

    internal void UpdateRoundText()
    {
        // Actualizar ronda
        if (roundText && RoundsManager.instance != null)
        {
            roundText.text = $"Round {RoundsManager.instance.CurrentRound + 1}";
        }
    }
    internal void UpdateScoreText()
    {
        // Actualizar score
        if (scoreText && GameManager.Instance != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.playerScore}";
        }
    }
    public void UpdateMaxScore()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.bestPlayerScore = GameManager.Instance.playerScore;
            PlayerPrefs.SetString("BestScore", GameManager.Instance.bestPlayerScore.ToString());
            PlayerPrefs.Save();
        }
    }

    public void AddScore(int points)
    {
        if (GameManager.Instance != null)
        {
            Debug.Log(comboManager.GetCurrentCombo());
            GameManager.Instance.playerScore += (points * comboManager.GetCurrentCombo());
            Debug.Log(GameManager.Instance.playerScore);
            UpdateScoreText();
        }
    }
    #endregion

    #region Pause Menu
    public void PauseGame()
    {
        if (isUpgradeMenuOpen) return; // No pausar si esta abierto el menu de mejoras

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel) pausePanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);

        MainCharacter.Instance.ResetShootingState(); // Evita disparos acomulados

        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel) pausePanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Recargar el nivel actual
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.selectedLevel))
        {
            SceneManager.LoadScene(GameManager.Instance.selectedLevel);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Resetear estado del juego
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameState();
        }

        SceneManager.LoadScene("Menu_Main");
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region Upgrade Menu Integration
    // Estos metodos son llamados por PowerUps.cs
    public void OnUpgradeMenuOpened()
    {
        isUpgradeMenuOpen = true;
        if (hudPanel) hudPanel.SetActive(false);
    }

    public void OnUpgradeMenuClosed()
    {
        isUpgradeMenuOpen = false;
        if (hudPanel) hudPanel.SetActive(true);
    }
    #endregion

    #region Public Helper Methods
    public bool IsPaused()
    {
        return isPaused;
    }

    public bool IsUpgradeMenuOpen()
    {
        return isUpgradeMenuOpen;
    }
    #endregion
}