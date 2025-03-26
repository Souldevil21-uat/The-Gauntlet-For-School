using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton reference to this instance
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource; // Handles background music
    public AudioSource sfxSource;   // Handles sound effects
    public AudioClip clip;          // Test clip for SFX playback

    private void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        LoadAudioSettings(); // Apply saved volume settings
    }

    private void Update()
    {
        // Test SFX playback with the T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("TEST: Playing death clip manually");
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    private void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }


    // Adjust music volume and save the setting
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

    // Adjust SFX volume and save the setting
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

    // Play a sound effect if possible
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

    // Load saved volume settings for music and SFX
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

    // Toggle mute status for music and save preference
    public void ToggleMusicMute()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicSource.mute;
            PlayerPrefs.SetInt("MusicMuted", musicSource.mute ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // Toggle mute status for SFX and save preference
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



