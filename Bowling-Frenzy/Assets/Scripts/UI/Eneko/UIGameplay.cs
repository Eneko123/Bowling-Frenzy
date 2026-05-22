using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private Image bigJumpFillBar;          // Imagen que se rellena
    [SerializeField] private TextMeshProUGUI bigJumpText;  // Texto que muestra "X.Xs" o "¡LISTO!"

    [SerializeField] private GameObject bossHealthIndicator;     // Almacena todo lo visualmente relacionado con la vida del boss
    [SerializeField] private Image bigBowlingBowlBossHealthBar;     // Imagen que muestra la vida del boss
    [SerializeField] private BoloEBoos boos;

    [System.Serializable]
    public class SpecialCooldownUI
    {
        public SpecialBullets bulletType;
        public Image fillImage;        // Circulo con Fill Type: Radial 360
        public TextMeshProUGUI cooldownText; // Tiempo restante

        // Estado interno
        [HideInInspector] public bool isOnCooldown;
        [HideInInspector] public float remainingTime;
        [HideInInspector] public float totalTime;
    }

    // Array para asignar en Inspector (orden libre)
    [SerializeField] private SpecialCooldownUI[] specialCooldownUIsArray;
    // Diccionario para búsqueda rápida por tipo de bala
    private Dictionary<SpecialBullets, SpecialCooldownUI> cooldownUIs = new Dictionary<SpecialBullets, SpecialCooldownUI>();

    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject upgradesPanel; // Este deberia ser el mismo que PowerUps.upgradePanel

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Iconospausa")]
    [SerializeField] private TextMeshProUGUI ComonBall;
    [SerializeField] private TextMeshProUGUI DrillBall;
    [SerializeField] private GameObject BlackDB;
    [SerializeField] private TextMeshProUGUI FreezeBall;
    [SerializeField] private GameObject BlackFB;
    [SerializeField] private TextMeshProUGUI BurnBall;
    [SerializeField] private GameObject BlackBB;
    [SerializeField] private TextMeshProUGUI Speed;
    [SerializeField] private TextMeshProUGUI Defense;

    GameObject[] bolas;
    NormalBulletBehaviour Balanormal;
    PierceBullet Bolataladro;
    SlowBullet Bolahielo;
    ExplosiveBulletBehaviour Bolaexplosion;
    PowerUps MejBolas;

    internal bool isPaused = false;
    internal bool isUpgradeMenuOpen = false;
    internal bool isTutorialOpen = false;

    private MainCharacter player;
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
        player = MainCharacter.Instance;
        InitializePanels();
        SetupButtonListeners();
        UpdateRoundText();
        UpdateScoreText();
        comboManager = GetComponentInChildren<Combos>();
        InitializeSpecialCooldowns();



        DrillBall.text = "......??"; BlackDB.SetActive(true);
        FreezeBall.text = "......??"; BlackFB.SetActive(true);
        BurnBall.text = "......??"; BlackBB.SetActive(true);
        AudioManager.Instance.PlayMusic("Batalla");
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
        UpdateBigJumpCooldown();
        UpdateSpecialCooldowns();

        ShowPlayerSatats();

        if (RoundsManager.instance.GetFinalRound())
        {
            bossHealthIndicator.SetActive(true);
            UpdateBossHealthBar();
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
            GameManager.Instance.playerScore += (points * comboManager.GetCurrentCombo());
            UpdateScoreText();
        }
    }

    void UpdateBigJumpCooldown()
    {
        bool isReady = player.CanUseBigJump();
        float cooldownRemaining = player.GetBigJumpCooldownRemaining();
        float cooldownTotal = player.GetBigJumpCooldownTotal();

        if (bigJumpFillBar != null && cooldownTotal > 0)
        {
            if (isReady)
            {
                // Completamente listo
                bigJumpFillBar.fillAmount = 1f;
            }
            else
            {
                // En cooldown: el timer va de cooldownTotal → 0
                // El fill va de 0 → 1
                // Invertimos: cuando cooldownRemaining es alto, fill es bajo
                float progress = 1f - (cooldownRemaining / cooldownTotal);
                bigJumpFillBar.fillAmount = Mathf.Clamp01(progress);
            }
        }

        if (bigJumpText != null)
        {
            if (isReady)
            {
                bigJumpText.text = "LISTO";
            }
            else
            {
                bigJumpText.text = cooldownRemaining.ToString("F1") + "s";
            }
        }
    }

    void UpdateBossHealthBar()
    {
        float bossHeath = boos.GetBossHealth();
        float bossHealthMax = 0;
        if (GameManager.Instance.difficulty == Difficulty.Easy)
        {
            bossHealthMax = 1000f;
        } 
        else if (GameManager.Instance.difficulty == Difficulty.Normal)
        {
            bossHealthMax = 1250f;
        }
        else if (GameManager.Instance.difficulty == Difficulty.Hard)
        {
            bossHealthMax = 1500f;
        }

        float progres = (bossHeath / bossHealthMax);
        bigBowlingBowlBossHealthBar.fillAmount = progres;
    }

    private void InitializeSpecialCooldowns()
    {
        foreach (var ui in specialCooldownUIsArray)
        {
            if (ui != null && !cooldownUIs.ContainsKey(ui.bulletType))
            {
                cooldownUIs[ui.bulletType] = ui;

                // Ocultar todo por defecto (no se ven hasta que se use la bala)
                ui.fillImage.gameObject.SetActive(false);
                if (ui.cooldownText != null) ui.cooldownText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateSpecialCooldowns()
    {
        foreach (var ui in cooldownUIs.Values)
        {
            if (!ui.isOnCooldown) continue;

            ui.remainingTime -= Time.deltaTime;

            if (ui.remainingTime <= 0f)
            {
                // Cooldown terminado: ocultar UI y resetear estado
                ui.remainingTime = 0f;
                ui.isOnCooldown = false;
                ui.fillImage.gameObject.SetActive(false);
                if (ui.cooldownText != null) ui.cooldownText.gameObject.SetActive(false);
            }
            else
            {
                // Progreso: 1 (lleno) → 0 (vacío)
                ui.fillImage.fillAmount = ui.remainingTime / ui.totalTime;
                if (ui.cooldownText != null) ui.cooldownText.text = $"{ui.remainingTime:F1}s";
            }
        }
    }

    public void StartSpecialCooldown(SpecialBullets type, float duration)
    {
        if (cooldownUIs.TryGetValue(type, out var ui))
        {
            ui.totalTime = duration;
            ui.remainingTime = duration;
            ui.isOnCooldown = true;

            // Mostrar UI al iniciar cooldown
            ui.fillImage.gameObject.SetActive(true);
            ui.fillImage.fillAmount = 1f; // Empieza completamente visible
            if (ui.cooldownText != null)
            {
                ui.cooldownText.gameObject.SetActive(true);
                ui.cooldownText.text = $"{duration:F1}s";
            }
        }
    }

    public bool IsSpecialReady(SpecialBullets type)
    {
        if (cooldownUIs.TryGetValue(type, out var ui)) return !ui.isOnCooldown;
        return true; // Fallback seguro si no hay UI asignada
    }

    // Marca visualmente la bala como lista
    private void SetSpecialReady(SpecialCooldownUI ui)
    {
        ui.isOnCooldown = false;
        ui.remainingTime = 0f;
        ui.fillImage.fillAmount = 1f;
        ui.cooldownText.text = "LISTO";
    }
    #endregion

    #region Pause Menu
    public void PauseGame()
    {
        if (isUpgradeMenuOpen || BotonTutorial.instance.GetIfTutorialIsOpen()) return; // No pausar si esta abierto el menu de mejoras

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

    void ShowPlayerSatats()
    {
        ComonBall.text = "..." + GenerateBullet.instance.ListBullets[0].GetComponentInChildren<NormalBulletBehaviour>().GetDamage().ToString() + " Dañ.";
        Defense.text = "..." + MainCharacter.Instance.GetDefense().ToString() + " Def.";
        Speed.text = "..." + MainCharacter.Instance.GetSpeed().ToString() + " Vel.";

        // Tienes que hacer un sistema para acceder a los datan como en el power ups o inventarte otra coasa
        //ComonBall.text = Balanormal.GetDamage().ToString();  WeaponSlots

        var unLocked = new List<SpecialBullets>();
        foreach (SpecialBullets b in System.Enum.GetValues(typeof(SpecialBullets)))
        {
            if (GenerateBullet.instance.specialBullets.Contains(b))
                unLocked.Add(b);
        
            //{ 
            //    if (b != SpecialBullets.Piercing)
            //    {
            //        BlackDB.SetActive(false);
            //        DrillBall.text = "......" + GenerateBullet.instance.ListPiercingBullets[0].GetComponentInChildren<PierceBullet>().GetDamage().ToString();
            //    }
            //    if (b != SpecialBullets.Slowing)
            //    {
            //        BlackFB.SetActive(false);
            //        FreezeBall.text = "......" + GenerateBullet.instance.ListSlowingBullets[0].GetComponentInChildren<SlowBullet>().GetDamage().ToString();
            //    }
            //    if (b != SpecialBullets.Explosive)
            //    {
            //        BlackBB.SetActive(false);
            //        BurnBall.text = "......" + GenerateBullet.instance.ListExplosiveBullets[0].GetComponentInChildren<ExplosiveBulletBehaviour>().GetDamage().ToString();
            //    }

            //}
        }

        for (int i = 0; i < unLocked.Count; i++) {
            if (unLocked[i] == SpecialBullets.Explosive)
            {
                BlackBB.SetActive(false);
                BurnBall.text = "..." + GenerateBullet.instance.ListExplosiveBullets[0].GetComponentInChildren<ExplosiveBulletBehaviour>().GetDamage().ToString() + " Dañ.";
            }
            if (unLocked[i] == SpecialBullets.Piercing)
            {
                BlackDB.SetActive(false);
                DrillBall.text = "..." + GenerateBullet.instance.ListPiercingBullets[0].GetComponentInChildren<PierceBullet>().GetDamage().ToString() + " Dañ.";
            }
            if (unLocked[i] == SpecialBullets.Slowing)
            {
                BlackFB.SetActive(false);
                FreezeBall.text = "..." + GenerateBullet.instance.ListSlowingBullets[0].GetComponentInChildren<SlowBullet>().GetDamage().ToString() + " Dañ.";
            }
        }

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