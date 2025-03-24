using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    /// <summary>
    /// Loads the main game scene.
    /// </summary>
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
    }

    /// <summary>
    /// Loads the options menu scene.
    /// </summary>
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

    /// <summary>
    /// Returns to the Main Menu from Options.
    /// </summary>
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

            // Force state change before switching the scene
            GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);

            // Load Main Menu Scene
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            Debug.LogError("ERROR: 'StartScene' is missing from Build Settings!");
        }
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Checks if a scene exists in Build Settings before loading.
    /// </summary>
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



