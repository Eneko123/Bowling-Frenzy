using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Instancia estática para el Singleton
    public static AudioManager Instance { get; private set; }

    [Header("Canales de Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Lista de Sonidos")]
    [SerializeField] private Sound[] musicSounds;
    [SerializeField] private Sound[] sfxSounds;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para reproducir Música (reemplaza la actual y entra en bucle)
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning("Música no encontrada: " + name);
            return;
        }

        musicSource.clip = s.clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Método para reproducir Efectos de Sonido (SFX)
    public void PlaySFX(string name, float volume = 1f)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning("Efecto de sonido no encontrado: " + name);
            return;
        }

        // PlayOneShot permite superponer sonidos sin interrumpir el anterior
        sfxSource.PlayOneShot(s.clip, volume);
        sfxSource.loop = false; // Asegura que los SFX no entren en bucle
    }
}

// Estructura de datos personalizada para organizar los sonidos en el Inspector
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}