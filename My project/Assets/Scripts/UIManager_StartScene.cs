using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager_StartScene : MonoBehaviour
{
    public GameObject mainMenu;        // Reference to the main menu UI panel
    public AudioClip clickClip;        // Sound clip to play on button click

    // Called when a button is clicked to play a sound and log the click
    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX(clickClip);
        Debug.Log("Button clicked!");
    }

    private void Start()
    {
        // Find and assign the main menu object if not already set
        mainMenu = GameObject.Find("MainMenu");
    }

    // Starts the game by loading the game scene
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Opens the options menu scene
    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    // Exits the application
    public void ExitGame()
    {
        Application.Quit();
    }
}


