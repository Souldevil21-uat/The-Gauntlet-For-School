using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager_OptionsScene : MonoBehaviour
{
    public Toggle twoPlayerToggle;             // Toggle for enabling two-player mode
    public Slider sfxVolumeSlider;             // Slider for sound effects volume
    public Slider musicVolumeSlider;           // Slider for music volume
    public AudioClip clickClip;                // Clip played on button click

    private void Start()
    {
        LoadSettings();

        // Apply volume to AudioManager on scene start
        AudioManager.Instance?.SetSFXVolume(sfxVolumeSlider.value);
        AudioManager.Instance?.SetMusicVolume(musicVolumeSlider.value);
    }

    // Called when any button in the options menu is clicked
    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX(clickClip);
        Debug.Log("Button clicked!");
    }

    // Loads saved settings and sets UI elements accordingly
    public void LoadSettings()
    {
        if (twoPlayerToggle != null)
        {
            twoPlayerToggle.isOn = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;
            twoPlayerToggle.onValueChanged.AddListener(SetTwoPlayerMode);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    // Saves the two-player mode preference
    public void SetTwoPlayerMode(bool isTwoPlayer)
    {
        PlayerPrefs.SetInt("TwoPlayerMode", isTwoPlayer ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Saves and applies the SFX volume setting
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        AudioManager.Instance?.SetSFXVolume(volume);
    }

    // Saves and applies the music volume setting
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        AudioManager.Instance?.SetMusicVolume(volume);
    }

    // Returns to the main menu scene
    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
        }

        SceneManager.LoadScene("StartScene");
    }
}


