using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    // Starts the game by changing the GameManager state and loading the main game scene
    public void StartGame()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("ERROR: GameManager is missing!");
            return;
        }

        if (SceneExists("Main Game"))
        {
            Debug.Log("Starting game...");
            GameManager.Instance.ChangeState(GameManager.GameState.InitGame);
        }
        else
        {
            Debug.LogError("ERROR: 'Main Game' scene is missing from Build Settings!");
        }

        if (AudioManager.Instance != null && !AudioManager.Instance.musicSource.isPlaying)
        {
            AudioManager.Instance.musicSource.loop = true;
            AudioManager.Instance.musicSource.Play();
        }
    }

    // Opens the options menu by changing state and loading the options scene
    public void OpenOptions()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("ERROR: GameManager is missing!");
            return;
        }

        if (SceneExists("OptionScene"))
        {
            Debug.Log("Opening Options Menu...");
            GameManager.Instance.ChangeState(GameManager.GameState.OptionsMenu);
            SceneManager.LoadScene("OptionScene");
        }
        else
        {
            Debug.LogError("ERROR: 'OptionScene' is missing from Build Settings!");
        }
    }

    // Returns to the main menu scene from the options menu
    public void BackToMainMenu()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("ERROR: GameManager is missing!");
            return;
        }

        if (SceneExists("StartScene"))
        {
            Debug.Log("Returning to Main Menu...");
            GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            Debug.LogError("ERROR: 'StartScene' is missing from Build Settings!");
        }
    }

    // Quits the application
    public void ExitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        // Stop play mode in editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Helper function that checks if a scene exists in the build settings
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (scenePath.Contains(sceneName))
            {
                return true;
            }
        }
        return false;
    }
}




