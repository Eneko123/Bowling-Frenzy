using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UILevelSelect : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject levelSelectionPanel;

    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;

    [Header("Level Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI selectedDifficultyText;
    [SerializeField] private Button difButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;

    private Difficulty currentDifficulty = Difficulty.Normal;
    private string selectedLevelName = "";
    private Button[] difficultyButtons;

    void Start()
    {
        InitializePanels();
        SetupButtonListeners();
        SetupDifficultyButtons();
        UpdateLevelAvailability();
    }

    void InitializePanels()
    {
        // Mostrar primero el panel de dificultad
        if (difficultyPanel) difficultyPanel.SetActive(true);
        if (levelSelectionPanel) levelSelectionPanel.SetActive(false);
    }

    void SetupButtonListeners()
    {
        // Difficulty buttons
        if (easyButton) easyButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.Easy));
        if (normalButton) normalButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.Normal));
        if (hardButton) hardButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.Hard));

        // Level buttons
        if (level1Button) level1Button.onClick.AddListener(() => OnLevelSelected("Level_1"));
        if (level2Button) level2Button.onClick.AddListener(() => OnLevelSelected("Level_2"));

        // Navigation buttons
        if (difButton) difButton.onClick.AddListener(DificultyElection);
        if (backButton) backButton.onClick.AddListener(OnBackClicked);
        if (confirmButton)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            confirmButton.interactable = false; // Desactivado hasta que se seleccione un nivel
        }
    }

    void SetupDifficultyButtons()
    {
        difficultyButtons = new Button[] { easyButton, normalButton, hardButton };

        // Establecer dificultad Normal por defecto
        OnDifficultySelected(Difficulty.Normal);
    }

    void UpdateLevelAvailability()
    {
        // Todos los niveles estan disponibles

        if (level1Button) level1Button.interactable = true;
        if (level2Button) level2Button.interactable = true;

    }

    #region Difficulty Selection
    void OnDifficultySelected(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.difficulty = difficulty;
        }

        // Actualizar visualizacion de botones
        UpdateDifficultyButtonVisuals();

        // Actualizar texto de dificultad seleccionada
        if (selectedDifficultyText)
        {
            selectedDifficultyText.text = $"Dificultad: {GetDifficultyName(difficulty)}";
        }

        // Mostrar panel de seleccion de nivel
        if (difficultyPanel) difficultyPanel.SetActive(false);
        if (levelSelectionPanel) levelSelectionPanel.SetActive(true);

        Debug.Log($"Difficulty selected: {difficulty}");
    }

    void UpdateDifficultyButtonVisuals()
    {
        // Resaltar el boton seleccionado
        Button selectedButton = currentDifficulty switch
        {
            Difficulty.Easy => easyButton,
            Difficulty.Hard => hardButton,
            _ => normalButton
        };
    }

    string GetDifficultyName(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => "Facil",
            Difficulty.Hard => "Dificil",
            _ => "Normal"
        };
    }
    #endregion

    #region Level Selection
    void OnLevelSelected(string levelName)
    {
        selectedLevelName = levelName;

        // Activar boton de confirmar
        if (confirmButton) confirmButton.interactable = true;

        Debug.Log($"Level selected: {levelName}");
    }

   
    #endregion

    #region Navigation
    void DificultyElection()
    {
        // Si estamos en el panel de niveles, volver a dificultad
        if (levelSelectionPanel && levelSelectionPanel.activeSelf)
        {
            levelSelectionPanel.SetActive(false);
            difficultyPanel.SetActive(true);
            selectedLevelName = "";
            if (confirmButton) confirmButton.interactable = false;
        }
        // Si estamos en dificultad, volver al menu principal
        else
        {
            Debug.Log("no funciona");
        }
    }
    void OnBackClicked()
    {
        //deberia llevarnos solo a inicio
        if (backButton)
        {
            SceneManager.LoadScene("Menu_Main");
        }
    }

    void OnConfirmClicked()
    {
        if (!string.IsNullOrEmpty(selectedLevelName))
        {
            Debug.Log($"Starting {selectedLevelName} on {currentDifficulty} difficulty");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.selectedLevel = selectedLevelName;
            }
            Time.timeScale = 1f; // Asegurar que el tiempo este normal
            SceneManager.LoadScene(selectedLevelName);
        }
    }
    #endregion
}