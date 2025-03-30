using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance for global access
    public static ScoreManager Instance { get; private set; }

    // Stores scores for each player by their player number
    private Dictionary<int, int> playerScores = new Dictionary<int, int>();

    // Reference to the in-game UI manager
    private UIManager_GameScene gameUIManager;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }

        FindGameUIManager(); // Find the UI manager for updating score displays
    }

    // Attempts to locate the UI Manager in the current scene
    private void FindGameUIManager()
    {
        gameUIManager = FindObjectOfType<UIManager_GameScene>();
    }

    // Adds score to the specified player and updates the UI
    public int AddScore(int playerNumber, int score)
    {
        // Initialize score if player doesn't have one yet
        if (!playerScores.ContainsKey(playerNumber))
        {
            playerScores[playerNumber] = 0;
        }

        // Add the new score
        playerScores[playerNumber] += score;

        // Update UI if manager is found
        if (gameUIManager != null)
        {
            gameUIManager.UpdateScore(playerNumber, playerScores[playerNumber]);
        }

        if (playerScores[playerNumber] >= 2000)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
        }

        return playerScores[playerNumber]; // Return updated score
    }

    // Returns the current score of a player
    public int GetScore(int playerNumber)
    {
        if (playerScores.ContainsKey(playerNumber))
        {
            return playerScores[playerNumber];
        }
        return 0; // Return 0 if player not found
    }
}







