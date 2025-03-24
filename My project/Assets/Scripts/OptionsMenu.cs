using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Gameplay Settings")]
    public Toggle twoPlayerToggle;
    public Toggle mapOfTheDayToggle;
    public AudioClip clickClip;

    private void Start()
    {
        // Load the saved value first
        int twoPlayerMode = PlayerPrefs.GetInt("TwoPlayerMode", 0);
        Debug.Log("🎮 Loaded Two Player Mode from PlayerPrefs: " + twoPlayerMode);
        

        if (twoPlayerToggle != null)
        {
            twoPlayerToggle.onValueChanged.RemoveAllListeners(); // Prevent event triggering on start
            twoPlayerToggle.isOn = twoPlayerMode == 1; // Set the correct state
            twoPlayerToggle.onValueChanged.AddListener(ToggleTwoPlayerMode); // Re-add event listener
        }
    }

    public void OnMusicVolumeChanged(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
    }

    public void OnSFXVolumeChanged(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }




    private void OnEnable()
    {
        LoadSettings(); // Reload settings every time options menu is opened
    }

    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX(clickClip);
    }

    private void LoadSettings()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (twoPlayerToggle != null)
            twoPlayerToggle.isOn = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;

        if (mapOfTheDayToggle != null)
            mapOfTheDayToggle.isOn = PlayerPrefs.GetInt("MapOfTheDay", 0) == 1;
    }

    public void ApplySettings()
    {
        // Ensure AudioManager is available before applying
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
            AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
        }

        // Save player settings
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetInt("TwoPlayerMode", twoPlayerToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("MapOfTheDay", mapOfTheDayToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("✅ Settings Applied & Saved");
    }

    public void ToggleTwoPlayerMode(bool isTwoPlayer)
    {
        // Prevent duplicate calls by checking if the value actually changed
        int currentState = PlayerPrefs.GetInt("TwoPlayerMode", 0);
        if ((isTwoPlayer ? 1 : 0) == currentState)
        {
            Debug.Log("🔄 Toggle ignored: No change detected.");
            return; // Exit early if the value is already the same
        }

        Debug.Log("🔄 ToggleTwoPlayerMode() Called: " + isTwoPlayer);
        PlayerPrefs.SetInt("TwoPlayerMode", isTwoPlayer ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("✅ Two Player Mode Updated in PlayerPrefs: " + PlayerPrefs.GetInt("TwoPlayerMode"));

        // 🔥 Immediately apply the change if GameManager exists
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyTwoPlayerMode();
        }
    }



    public void SetMapOfTheDay(bool isEnabled)
    {
        PlayerPrefs.SetInt("MapOfTheDay", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}



