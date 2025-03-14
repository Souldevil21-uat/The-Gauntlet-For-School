using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main Game"); // Change "GameScene" to your actual game scene name
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionScene"); // Ensure you have an Options Menu scene
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
