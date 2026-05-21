using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIMainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Buttons - Main Panel")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Buttons - Options Panel")]
    [SerializeField] private Button optionsBackButton;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Buttons - Credits Panel")]
    [SerializeField] private Button creditsBackButton;

    [Header("Buttons - Tutorial Panel")]
    [SerializeField] private Button tutorialBackButton;
    public static UIMainMenu instance; 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        InitializePanels();
        SetupButtonListeners();
        LoadAudioSettings();
        AudioManager.Instance.PlayMusic("MainMenu");
    }

    void InitializePanels()
    {
        // Mostrar solo el panel principal al inicio
        if (mainPanel) mainPanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    void SetupButtonListeners()
    {
        // Main Panel
        if (playButton) playButton.onClick.AddListener(OnPlayClicked);
        if (tutorialButton) tutorialButton.onClick.AddListener(OnTutorialClicked);
        if (optionsButton) optionsButton.onClick.AddListener(OnOptionsClicked);
        if (creditsButton) creditsButton.onClick.AddListener(OnCreditsClicked);
        if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);

        // Options Panel
        if (optionsBackButton) optionsBackButton.onClick.AddListener(OnOptionsBackClicked);
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Credits Panel
        if (creditsBackButton) creditsBackButton.onClick.AddListener(OnCreditsBackClicked);

        // Tutorial Panel
        if (tutorialBackButton) tutorialBackButton.onClick.AddListener(OnCreditsBackClicked);
    }

    void LoadAudioSettings()
    {
        if (GameManager.Instance != null)
        {
            if (masterVolumeSlider)
            {
                masterVolumeSlider.value = GameManager.Instance.masterVolume;
                UpdateVolumeText(masterVolumeText, GameManager.Instance.masterVolume);
            }
            if (musicVolumeSlider)
            {
                musicVolumeSlider.value = GameManager.Instance.musicVolume;
                UpdateVolumeText(musicVolumeText, GameManager.Instance.musicVolume);
            }
            if (sfxVolumeSlider)
            {
                sfxVolumeSlider.value = GameManager.Instance.sfxVolume;
                UpdateVolumeText(sfxVolumeText, GameManager.Instance.sfxVolume);
            }
        }
    }

    #region Button Callbacks - Main Panel
    void OnPlayClicked()
    {
        Debug.Log("Play clicked - Loading level selection");
        if (GameManager.Instance != null)
        {
            SceneManager.LoadScene("Menu_Levels");
        }
    }

    void OnTutorialClicked()
    {
        Debug.Log("Tutorial clicked");
        ShowPanel(tutorialPanel);
    }

    void OnOptionsClicked()
    {
        Debug.Log("Options clicked");
        ShowPanel(optionsPanel);
    }

    void OnCreditsClicked()
    {
        Debug.Log("Credits clicked");
        ShowPanel(creditsPanel);
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

    #region Button Callbacks - Options Panel
    void OnOptionsBackClicked()
    {
        Debug.Log("Options back clicked");
        ShowPanel(mainPanel);
    }

    void OnMasterVolumeChanged(float value)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.masterVolume = Mathf.Clamp01(value);
            AudioListener.volume = GameManager.Instance.masterVolume;
            PlayerPrefs.SetFloat("MasterVolume", GameManager.Instance.masterVolume);
            PlayerPrefs.Save();
        }
        UpdateVolumeText(masterVolumeText, value);
    }

    void OnMusicVolumeChanged(float value)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("MusicVolume", GameManager.Instance.musicVolume);
            PlayerPrefs.Save();
        }
        UpdateVolumeText(musicVolumeText, value);
    }

    void OnSFXVolumeChanged(float value)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("SFXVolume", GameManager.Instance.sfxVolume);
            PlayerPrefs.Save();
        }
        UpdateVolumeText(sfxVolumeText, value);
    }

    void UpdateVolumeText(TextMeshProUGUI textComponent, float value)
    {
        if (textComponent)
        {
            textComponent.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
    #endregion

    #region Button Callbacks - Credits Panel
    void OnCreditsBackClicked()
    {
        Debug.Log("Credits back clicked");
        ShowPanel(mainPanel);
    }
    #endregion

    #region Button Callbacks - Tutorial Panel
    void OnTutorialBackClicked()
    {
        Debug.Log("Tutorial back clicked");
        ShowPanel(mainPanel);
    }
    #endregion

    #region Helper Methods
    void ShowPanel(GameObject panelToShow)
    {
        // Desactivar todos los paneles
        if (mainPanel) mainPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);

        // Activar el panel solicitado
        if (panelToShow) panelToShow.SetActive(true);
    }
    #endregion
}