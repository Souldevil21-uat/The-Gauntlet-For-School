using UnityEngine;
using TMPro;

public class UIManager_GameScene : MonoBehaviour
{
    public static UIManager_GameScene Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI player1LivesText;
    public TextMeshProUGUI player2LivesText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public GameObject gameOverScreen;
    public TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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


