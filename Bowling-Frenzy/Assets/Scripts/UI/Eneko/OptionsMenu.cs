using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    const string KEY_MUSIC_VOL = "MusicVolume";
    const string KEY_SFX_VOL = "SFXVolume";

    void Start()
    {
        float savedMusicVol = PlayerPrefs.GetFloat(KEY_MUSIC_VOL, 1f);
        float savedSfxVol = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);

        musicVolumeSlider.SetValueWithoutNotify(savedMusicVol);
        sfxVolumeSlider.SetValueWithoutNotify(savedSfxVol);

        ApplyMusicVolume(savedMusicVol);
        ApplySFXVolume(savedSfxVol);

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
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

    void ApplyMusicVolume(float value)
    {
        AudioManager.Instance.musicSource.volume = value;
    }

    void ApplySFXVolume(float value)
    {
        AudioManager.Instance.sfxSource.volume = value;
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
