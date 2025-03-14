using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen; // Assign the Game Over UI Panel in the Inspector
    public TextMeshProUGUI finalScoreText; // Assign the Final Score text

    private bool isGameOver = false;

    public void ShowGameOverScreen(int finalScore)
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverScreen.SetActive(true);
        finalScoreText.text = "Final Score: " + finalScore;
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene("GameScene"); // Reloads the game scene
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene"); // Ensure StartScene is in Build Settings
    }
}

