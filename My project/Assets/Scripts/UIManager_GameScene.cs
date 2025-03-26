using UnityEngine;
using TMPro;

public class UIManager_GameScene : MonoBehaviour
{
    public static UIManager_GameScene Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI player1LivesText;    // UI text for player 1's lives
    public TextMeshProUGUI player2LivesText;    // UI text for player 2's lives
    public TextMeshProUGUI player1ScoreText;    // UI text for player 1's score
    public TextMeshProUGUI player2ScoreText;    // UI text for player 2's score
    public GameObject gameOverScreen;           // Game over screen UI panel
    public TextMeshProUGUI finalScoreText;      // Final score display on game over screen

    [Header("UI Root Panels")]
    public GameObject UIPanel;                  // Root panel for all UI elements

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Enable the UI panel if it was disabled in the editor
        if (UIPanel != null && !UIPanel.activeSelf)
        {
            Debug.Log("UIPanel was disabled. Enabling it.");
            UIPanel.SetActive(true);
        }
    }

    // Updates the life count UI for the specified player
    public void UpdateLives(int playerNumber, int lives)
    {
        if (playerNumber == 1 && player1LivesText != null)
        {
            player1LivesText.text = "P1 Lives: " + lives;
        }
        else if (playerNumber == 2 && player2LivesText != null)
        {
            player2LivesText.text = "P2 Lives: " + lives;
        }
    }

    // Updates the score UI for the specified player
    public void UpdateScore(int playerNumber, int score)
    {
        if (playerNumber == 1 && player1ScoreText != null)
        {
            player1ScoreText.text = "P1 Score: " + score;
        }
        else if (playerNumber == 2 && player2ScoreText != null)
        {
            player2ScoreText.text = "P2 Score: " + score;
        }
    }

    // Displays the Game Over screen with the final score
    public void ShowGameOverScreen(int finalScore)
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + finalScore;
            }
        }
    }
}




