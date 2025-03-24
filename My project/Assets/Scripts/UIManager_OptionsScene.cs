using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager_OptionsScene : MonoBehaviour
{
    public Toggle twoPlayerToggle;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    private void Start()
    {
        LoadSettings();

        // Manually apply the loaded volume values to AudioManager
        AudioManager.Instance?.SetSFXVolume(sfxVolumeSlider.value);
        AudioManager.Instance?.SetMusicVolume(musicVolumeSlider.value);
    }

    public AudioClip clickClip;

    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX(clickClip);
        Debug.Log("Button clicked!");
    }

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

    public void SetTwoPlayerMode(bool isTwoPlayer)
    {
        PlayerPrefs.SetInt("TwoPlayerMode", isTwoPlayer ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        AudioManager.Instance?.SetSFXVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        AudioManager.Instance?.SetMusicVolume(volume);
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
        }

        SceneManager.LoadScene("StartScene");
    }
}

