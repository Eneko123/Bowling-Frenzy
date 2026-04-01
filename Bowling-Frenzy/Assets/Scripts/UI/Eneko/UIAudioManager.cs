using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip panelOpenSound;
    [SerializeField] private AudioClip panelCloseSound;
    [SerializeField] private AudioClip upgradeSelectSound;
    [SerializeField] private AudioClip errorSound;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool autoSetupButtons = true;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetupAudioSource();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (autoSetupButtons)
        {
            SetupAllButtons();
        }
    }

    void SetupAudioSource()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
    }

    void SetupAllButtons()
    {
        // Buscar todos los botones en la escena y aniadirles eventos de audio
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            // Verificar si ya tiene el componente
            UIButtonSound buttonSound = button.GetComponent<UIButtonSound>();
            if (buttonSound == null)
            {
                buttonSound = button.gameObject.AddComponent<UIButtonSound>();
            }
        }
    }

    #region Sound Playback
    public void PlayButtonHover()
    {
        PlaySound(buttonHoverSound);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayPanelOpen()
    {
        PlaySound(panelOpenSound);
    }

    public void PlayPanelClose()
    {
        PlaySound(panelCloseSound);
    }

    public void PlayUpgradeSelect()
    {
        PlaySound(upgradeSelectSound);
    }

    public void PlayError()
    {
        PlaySound(errorSound);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // Aplicar volumen del GameManager si existe
            float finalVolume = volume;
            if (GameManager.Instance != null)
            {
                finalVolume *= GameManager.Instance.sfxVolume;
            }

            audioSource.PlayOneShot(clip, finalVolume);
        }
    }
    #endregion

    #region Public Methods
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void PlayCustomSound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip != null && audioSource != null)
        {
            float finalVolume = volume * volumeMultiplier;
            if (GameManager.Instance != null)
            {
                finalVolume *= GameManager.Instance.sfxVolume;
            }
            audioSource.PlayOneShot(clip, finalVolume);
        }
    }
    #endregion
}

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Custom Sounds (Opcional)")]
    [SerializeField] private AudioClip customHoverSound;
    [SerializeField] private AudioClip customClickSound;

    [Header("Settings")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Solo reproducir sonido de hover si el boton esta interactuable
        if (playHoverSound && button != null && button.interactable)
        {
            if (customHoverSound != null && UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayCustomSound(customHoverSound);
            }
            else if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayButtonHover();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo reproducir sonido de click si el boton esta interactuable
        if (playClickSound && button != null && button.interactable)
        {
            if (customClickSound != null && UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayCustomSound(customClickSound);
            }
            else if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayButtonClick();
            }
        }
    }

    #region Public Methods
    public void SetHoverSound(AudioClip clip)
    {
        customHoverSound = clip;
    }

    public void SetClickSound(AudioClip clip)
    {
        customClickSound = clip;
    }

    public void EnableHoverSound(bool enable)
    {
        playHoverSound = enable;
    }

    public void EnableClickSound(bool enable)
    {
        playClickSound = enable;
    }
    #endregion
}