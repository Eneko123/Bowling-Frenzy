using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] CameraPlayer cameraPlayer;

    [Header("Sliders")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider sensitivitySlider;

    const string KEY_MUSIC_VOL = "MusicVolume";
    const string KEY_SFX_VOL = "SFXVolume";
    const string KEY_SENS = "Sens";

    void Start()
    {
        float savedMusicVol = PlayerPrefs.GetFloat(KEY_MUSIC_VOL, 1f);
        float savedSfxVol = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);
        float savedSens = PlayerPrefs.GetFloat(KEY_SENS, cameraPlayer.sensibilityX);

        musicVolumeSlider.SetValueWithoutNotify(savedMusicVol);
        sfxVolumeSlider.SetValueWithoutNotify(savedSfxVol);
        sensitivitySlider.SetValueWithoutNotify(savedSens);

        ApplyMusicVolume(savedMusicVol);
        ApplySFXVolume(savedSfxVol);
        ApplySensitivity(savedSens);

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensChanged);
    }

    void OnMusicVolumeChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(KEY_MUSIC_VOL, value);
    }

    void OnSFXVolumeChanged(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(KEY_SFX_VOL, value);
    }

    void OnSensChanged(float value)
    {
        ApplySensitivity(value);
        PlayerPrefs.SetFloat(KEY_SENS, value);
    }

    void ApplyMusicVolume(float value)
    {
        AudioManager.Instance.musicSource.volume = value;
    }

    void ApplySFXVolume(float value)
    {
        AudioManager.Instance.sfxSource.volume = value;
    }

    void ApplySensitivity(float value)
    {
        // Ambos ejes comparten el mismo valor
        cameraPlayer.sensibilityX = value;
        cameraPlayer.sensibilityY = value;
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }
}
