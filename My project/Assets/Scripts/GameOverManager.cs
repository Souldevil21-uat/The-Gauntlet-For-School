using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Game Over UI")]
    public GameObject gameOverScreen; // Assign the Game Over UI Panel in the Inspector
    public TextMeshProUGUI finalScoreText; // Assign the Final Score text

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowGameOverScreen(int finalScore)
    {
        if (isGameOver) return; // Prevent multiple activations

        isGameOver = true;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverManager: Game Over screen is not assigned!");
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }
        else
        {
            Debug.LogError("GameOverManager: Final Score Text is not assigned!");
        }

        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time
        isGameOver = false; // Reset game over state
        SceneManager.LoadScene("GameScene"); // Reloads the game scene
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume time
        isGameOver = false; // Reset game over state

        if (SceneUtility.GetBuildIndexByScenePath("StartScene") == -1)
        {
            Debug.LogError("GameOverManager: StartScene is not in Build Settings!");
        }
        else
        {
            SceneManager.LoadScene("StartScene"); // Ensure StartScene is in Build Settings
        }
    }
}


