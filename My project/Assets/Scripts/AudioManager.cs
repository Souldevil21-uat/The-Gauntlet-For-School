using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip clip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadAudioSettings(); // Ensure saved settings apply on startup
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("TEST: Playing death clip manually");
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    // Set Music Volume with a null check
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("MusicSource is not assigned in AudioManager.");
        }
    }

    // Set SFX Volume with a null check
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("SFXSource is not assigned in AudioManager.");
        }
    }

    // Play a sound effect with a null check
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFXSource or AudioClip is missing in PlaySFX().");
        }
    }

    // Load saved volume settings on startup
    private void LoadAudioSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        }
        if (sfxSource != null)
        {
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        }
    }

    // Toggle Mute for Music
    public void ToggleMusicMute()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicSource.mute;
            PlayerPrefs.SetInt("MusicMuted", musicSource.mute ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // Toggle Mute for SFX
    public void ToggleSFXMute()
    {
        if (sfxSource != null)
        {
            sfxSource.mute = !sfxSource.mute;
            PlayerPrefs.SetInt("SFXMuted", sfxSource.mute ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}


