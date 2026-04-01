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
    [SerializeField] private TextMeshProUGUI level1Text;
    [SerializeField] private TextMeshProUGUI level2Text;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI selectedDifficultyText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;

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

        // Actualizar textos
        if (level1Text) level1Text.text = "Mapa 1";
        if (level2Text) level2Text.text = "Mapa 2";
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
        // Resetear todos los botones
        foreach (var btn in difficultyButtons)
        {
            if (btn != null)
            {
                var colors = btn.colors;
                colors.normalColor = normalColor;
                btn.colors = colors;
            }
        }

        // Resaltar el boton seleccionado
        Button selectedButton = currentDifficulty switch
        {
            Difficulty.Easy => easyButton,
            Difficulty.Hard => hardButton,
            _ => normalButton
        };

        if (selectedButton != null)
        {
            var colors = selectedButton.colors;
            colors.normalColor = selectedColor;
            selectedButton.colors = colors;
        }
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

        // Actualizar visualizacion de botones de nivel
        UpdateLevelButtonVisuals(levelName);

        Debug.Log($"Level selected: {levelName}");
    }

    void UpdateLevelButtonVisuals(string levelName)
    {
        // Resetear todos los botones de nivel
        SetButtonHighlight(level1Button, levelName == "Level_1");
        SetButtonHighlight(level2Button, levelName == "Level_2");
    }

    void SetButtonHighlight(Button button, bool isSelected)
    {
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = isSelected ? selectedColor : normalColor;
            button.colors = colors;
        }
    }
    #endregion

    #region Navigation
    void OnBackClicked()
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