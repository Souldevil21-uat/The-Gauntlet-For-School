using UnityEngine;
using TMPro; // Required if using TextMeshPro for UI

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance
    public int score = 0; // Tracks player score
    public TextMeshProUGUI scoreText; // Reference to UI text displaying score

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

    public int GetScore()
    {
        return score;
    }


    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance.UpdateScore(score); // Update UI
    }


    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}

