using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private Dictionary<int, int> playerScores = new Dictionary<int, int>();
    private UIManager_GameScene gameUIManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FindGameUIManager();
    }

    private void FindGameUIManager()
    {
        gameUIManager = FindObjectOfType<UIManager_GameScene>();
    }

    // ✅ Now returns the updated score!
    public int AddScore(int playerNumber, int score)
    {
        if (!playerScores.ContainsKey(playerNumber))
        {
            playerScores[playerNumber] = 0;
        }

        playerScores[playerNumber] += score;

        if (gameUIManager != null)
        {
            gameUIManager.UpdateScore(playerNumber, playerScores[playerNumber]);
        }

        return playerScores[playerNumber];
    }

    // ✅ Optional: Get score manually (used for UI or win checks)
    public int GetScore(int playerNumber)
    {
        if (playerScores.ContainsKey(playerNumber))
        {
            return playerScores[playerNumber];
        }
        return 0;
    }
}






