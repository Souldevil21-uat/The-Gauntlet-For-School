using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider musicVolumeSlider;        // Slider to control music volume
    public Slider sfxVolumeSlider;          // Slider to control sound effects volume

    [Header("Gameplay Settings")]
    public Toggle twoPlayerToggle;          // Toggle to enable/disable two-player mode
    public Toggle mapOfTheDayToggle;        // Toggle to use "Map of the Day" seed
    public AudioClip clickClip;             // Sound clip for button clicks

    private void Start()
    {
        // Load Two Player preference from PlayerPrefs
        int twoPlayerMode = PlayerPrefs.GetInt("TwoPlayerMode", 0);

        if (twoPlayerToggle != null)
        {
            // Prevent triggering the event while setting the toggle
            twoPlayerToggle.onValueChanged.RemoveAllListeners();
            twoPlayerToggle.isOn = twoPlayerMode == 1;
            twoPlayerToggle.onValueChanged.AddListener(ToggleTwoPlayerMode);
        }
    }

    private void OnEnable()
    {
        // Reload the latest saved settings when the menu is opened
        LoadSettings();
    }

    public void OnMusicVolumeChanged(float volume)
    {
        // Update music volume in the AudioManager
        AudioManager.Instance.SetMusicVolume(volume);
    }

    public void OnSFXVolumeChanged(float volume)
    {
        // Update sound effect volume in the AudioManager
        AudioManager.Instance.SetSFXVolume(volume);
    }

    public void OnButtonClick()
    {
        // Play a click sound when a button is pressed
        AudioManager.Instance.PlaySFX(clickClip);
    }

    private void LoadSettings()
    {
        // Load music volume
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Load sound effects volume
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Load Two Player toggle state
        if (twoPlayerToggle != null)
            twoPlayerToggle.isOn = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;

        // Load Map of the Day toggle state
        if (mapOfTheDayToggle != null)
            mapOfTheDayToggle.isOn = PlayerPrefs.GetInt("MapOfTheDay", 0) == 1;
    }

    public void ApplySettings()
    {
        // Apply and save audio volume settings
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
            AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
        }

        // Save all settings to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetInt("TwoPlayerMode", twoPlayerToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("MapOfTheDay", mapOfTheDayToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleTwoPlayerMode(bool isTwoPlayer)
    {
        // Check if the new value differs from the saved one
        int currentState = PlayerPrefs.GetInt("TwoPlayerMode", 0);
        if ((isTwoPlayer ? 1 : 0) == currentState)
            return;

        // Save the new toggle state
        PlayerPrefs.SetInt("TwoPlayerMode", isTwoPlayer ? 1 : 0);
        PlayerPrefs.Save();

        // Apply the change to the GameManager if it's available
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyTwoPlayerMode();
        }
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMapOfTheDay(bool isEnabled)
    {
        // Save the Map of the Day toggle state
        PlayerPrefs.SetInt("MapOfTheDay", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}




