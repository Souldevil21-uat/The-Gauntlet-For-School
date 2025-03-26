using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton reference
    public static GameManager Instance { get; private set; }

    // States the game can be in
    public enum GameState { MainMenu, OptionsMenu, InitGame, PlayGame, GameOver }
    public GameState currentState;

    [Header("UI Elements")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI finalScoreText;

    [Header("Player & AI Management")]
    public GameObject playerPrefab;
    public List<Transform> playerSpawnPoints;
    public List<GameObject> aiTankPrefabs;
    public List<Transform> aiSpawnPoints;
    private List<GameObject> spawnedAI = new List<GameObject>();
    [SerializeField] private int initialAITankCount = 6;

    [Header("Powerup Management")]
    public List<Transform> powerupSpawnPoints;
    public GameObject powerupPrefab;
    public float powerupRespawnTime = 10f;

    [Header("Player Lives & Score Management")]
    public int player1Lives = 3;
    public int player2Lives = 3;
    private Dictionary<int, int> playerScores = new Dictionary<int, int>();

    public List<PlayerController> playerControllers = new List<PlayerController>();
    public List<TankPawn> tankPawns = new List<TankPawn>();
    public List<AIController> aiControllers = new List<AIController>();

    private UIManager_GameScene gameUIManager;

    public GameObject player1 { get; private set; }
    public GameObject player2 { get; private set; }

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
            return;
        }

        playerScores[1] = 0;
        playerScores[2] = 0;
    }

    private void Start()
    {
        FindGameUIManager();
        ChangeState(GameState.MainMenu);
    }

    // Handles transitions between game states
    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            Debug.LogWarning($"Game is already in {newState} state.");
            return;
        }

        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                SceneManager.LoadScene("StartScene");
                break;
            case GameState.OptionsMenu:
                SceneManager.LoadScene("OptionScene");
                break;
            case GameState.InitGame:
                StartCoroutine(InitializeGame());
                break;
            case GameState.PlayGame:
                break;
            case GameState.GameOver:
                ShowGameOverScreen();
                break;
        }
    }

    // Finds the game UI manager and links important UI elements
    private void FindGameUIManager()
    {
        gameUIManager = FindObjectOfType<UIManager_GameScene>();

        GameObject goUI = FindInactiveObject("GameOverScreen");
        if (goUI != null)
        {
            gameOverScreen = goUI;
            gameOverScreen.SetActive(false);
        }

        TextMeshProUGUI finalScore = FindInactiveObject("Final Score")?.GetComponent<TextMeshProUGUI>();
        if (finalScore != null)
        {
            finalScoreText = finalScore;
        }
    }


    private GameObject FindInactiveObject(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform obj in objs)
        {
            if (obj.hideFlags == HideFlags.None && obj.name == name)
            {
                return obj.gameObject;
            }
        }
        return null;
    }


    // Loads scene and sets up player, AI, and powerups
    private IEnumerator InitializeGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Game");
        while (!asyncLoad.isDone) yield return null;
        yield return null;

        // Apply saved audio settings
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(sfxVol);
            AudioManager.Instance.SetMusicVolume(musicVol);
        }

        FindGameUIManager();

        // Collect spawn point references
        playerSpawnPoints = new List<Transform>(GameObject.FindGameObjectsWithTag("PlayerSpawn").Length);
        foreach (var obj in GameObject.FindGameObjectsWithTag("PlayerSpawn"))
            playerSpawnPoints.Add(obj.transform);

        aiSpawnPoints = new List<Transform>(GameObject.FindGameObjectsWithTag("AI Spawn Point").Length);
        foreach (var obj in GameObject.FindGameObjectsWithTag("AI Spawn Point"))
            aiSpawnPoints.Add(obj.transform);

        powerupSpawnPoints = new List<Transform>(GameObject.FindGameObjectsWithTag("PowerupSpawn").Length);
        foreach (var obj in GameObject.FindGameObjectsWithTag("PowerupSpawn"))
            powerupSpawnPoints.Add(obj.transform);

        ApplyTwoPlayerMode();
        SpawnPlayers();
        SpawnAI();
        SpawnPowerups();

        CameraManager.Instance.UpdateCameraMode();
        ChangeState(GameState.PlayGame);
    }


    // Spawns player tanks based on mode
    public void SpawnPlayers()
    {
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;

        if (playerPrefab == null || playerSpawnPoints.Count < 2)
        {
            Debug.LogError("Missing player prefab or spawn points.");
            return;
        }

        // Player 1 spawn
        Transform spawn1 = playerSpawnPoints[0];
        player1 = Instantiate(playerPrefab, spawn1.position, spawn1.rotation);
        PlayerController pc1 = player1.GetComponent<PlayerController>();
        pc1.playerNumber = 1;
        RegisterPlayer(pc1);
        UIManager_GameScene.Instance.UpdateLives(1, player1Lives);
        ScoreManager.Instance.AddScore(1, 0);

        // Player 2 spawn if enabled
        if (isTwoPlayer)
        {
            Transform spawn2 = playerSpawnPoints[1];
            player2 = Instantiate(playerPrefab, spawn2.position, spawn2.rotation);
            player2.tag = "Player 2";

            PlayerController pc2 = player2.GetComponent<PlayerController>();
            pc2.playerNumber = 2;
            RegisterPlayer(pc2);
            UIManager_GameScene.Instance.UpdateLives(2, player2Lives);
            ScoreManager.Instance.AddScore(2, 0);
        }
    }

    // Spawns AI tanks using a variety of types
    public void SpawnAI()
    {
        if (aiTankPrefabs.Count == 0 || aiSpawnPoints.Count == 0)
        {
            Debug.LogWarning("Missing AI prefabs or spawn points.");
            return;
        }

        List<GameObject> guaranteed = new List<GameObject>();
        HashSet<System.Type> addedTypes = new HashSet<System.Type>();

        // Ensure each AI type appears at least once
        foreach (var prefab in aiTankPrefabs)
        {
            var controller = prefab.GetComponent<AIController>();
            if (controller == null) continue;
            var type = controller.GetType();
            if (!addedTypes.Contains(type))
            {
                guaranteed.Add(prefab);
                addedTypes.Add(type);
            }
        }

        // Fill with extras up to max allowed
        List<GameObject> extras = new List<GameObject>(aiTankPrefabs);
        ShuffleList(extras);
        int maxSpawn = Mathf.Min(8, aiSpawnPoints.Count);
        while (guaranteed.Count < maxSpawn)
        {
            GameObject extra = extras[Random.Range(0, extras.Count)];
            guaranteed.Add(extra);
        }

        // Shuffle and spawn
        List<Transform> shuffledSpawns = new List<Transform>(aiSpawnPoints);
        ShuffleList(shuffledSpawns);

        for (int i = 0; i < guaranteed.Count; i++)
        {
            GameObject aiTank = Instantiate(guaranteed[i], shuffledSpawns[i].position, shuffledSpawns[i].rotation);
            AIController aiController = aiTank.GetComponent<AIController>();

            if (aiController is AIPatrolChase patrolAI)
            {
                patrolAI.patrolPoints = new List<Transform>(GameObject.FindGameObjectsWithTag("Waypoint").Length);
                foreach (var wp in GameObject.FindGameObjectsWithTag("Waypoint"))
                    patrolAI.patrolPoints.Add(wp.transform);
                ShuffleList(patrolAI.patrolPoints);
            }
            else if (aiController is AIFlee fleeAI)
            {
                fleeAI.patrolPoints = new List<Transform>(GameObject.FindGameObjectsWithTag("Waypoint").Length);
                foreach (var wp in GameObject.FindGameObjectsWithTag("Waypoint"))
                    fleeAI.patrolPoints.Add(wp.transform);
                ShuffleList(fleeAI.patrolPoints);
            }

            RegisterAI(aiController);
        }
    }

    // Respawn a new AI tank after a delay
    public void RespawnAIWithDelay(float delay)
    {
        StartCoroutine(RespawnAIAfterDelay(delay));
    }

    private IEnumerator RespawnAIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (aiTankPrefabs.Count == 0 || aiSpawnPoints.Count == 0)
            yield break;

        GameObject prefab = aiTankPrefabs[Random.Range(0, aiTankPrefabs.Count)];
        Transform spawnPoint = aiSpawnPoints[Random.Range(0, aiSpawnPoints.Count)];

        GameObject aiTank = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        AIController aiController = aiTank.GetComponent<AIController>();

        if (aiController is AIPatrolChase patrolAI)
        {
            patrolAI.patrolPoints = new List<Transform>();
            foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
                patrolAI.patrolPoints.Add(wp.transform);
        }
        else if (aiController is AIFlee fleeAI)
        {
            fleeAI.patrolPoints = new List<Transform>();
            foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
                fleeAI.patrolPoints.Add(wp.transform);
        }

        RegisterAI(aiController);
    }

    // Spawns powerups at designated spawn points
    public void SpawnPowerups()
    {
        foreach (Transform spawn in powerupSpawnPoints)
        {
            GameObject p = Instantiate(powerupPrefab, spawn.position, spawn.rotation);
            Powerup ps = p.GetComponent<Powerup>();
            if (ps != null)
            {
                ps.type = (Powerup.PowerupType)Random.Range(0, System.Enum.GetValues(typeof(Powerup.PowerupType)).Length);
                ps.SetRespawn(spawn, powerupRespawnTime);
            }
        }
    }

    // Displays game over UI
    public void ShowGameOverScreen()
    {
        Debug.Log($"GameOverScreen: {(gameOverScreen != null ? "Found" : "Missing")}");
        Debug.Log($"FinalScoreText: {(finalScoreText != null ? "Found" : "Missing")}");
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        if (finalScoreText != null)
        {
            int score = ScoreManager.Instance?.GetScore(1) ?? 0;
            finalScoreText.text = $"Final Score: {score}";
        }
        else
        {
            Debug.LogWarning("Final Score Text is missing!");
        }

        Button restartButton = gameOverScreen?.transform.Find("Restart")?.GetComponent<Button>();
        Button mainMenuButton = gameOverScreen?.transform.Find("MainMenu")?.GetComponent<Button>();

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartButton);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenuButton);
        }
    }


    // Utility: shuffle a list
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }


    // Adds a player controller to the list if not already registered
    public void RegisterPlayer(PlayerController player)
    {
        if (!playerControllers.Contains(player))
        {
            playerControllers.Add(player);
            Debug.Log($"Registered Player {player.playerNumber}.");
        }
    }

    // Adds an AI controller to the list if not already registered
    public void RegisterAI(AIController ai)
    {
        if (!aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
        }
    }

    // Removes an AI controller from the list
    public void UnregisterAI(AIController ai)
    {
        if (aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
        }
    }

    // Registers a tank pawn in the tank list
    public void RegisterTank(TankPawn tank)
    {
        if (!tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    // Removes a tank pawn from the tank list
    public void UnregisterTank(TankPawn tank)
    {
        if (tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }

    // Logs and applies the two-player mode setting
    public void ApplyTwoPlayerMode()
    {
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;
        Debug.Log($"Two Player Mode: {(isTwoPlayer ? "Enabled" : "Disabled")}");
    }

    // Returns the number of currently registered players
    public int GetPlayerCount()
    {
        return playerControllers.Count;
    }

    // Returns the first registered player controller
    public PlayerController GetPlayer()
    {
        return playerControllers.Count > 0 ? playerControllers[0] : null;
    }

    // Respawns a player tank by number and sets it up
    public void RespawnPlayer(int playerNumber)
    {
        if (playerSpawnPoints.Count < 2)
        {
            Debug.LogError("Not enough spawn points for respawn!");
            return;
        }

        Transform spawnPoint = (playerNumber == 1) ? playerSpawnPoints[0] : playerSpawnPoints[1];

        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.playerNumber = playerNumber;
        RegisterPlayer(playerController);

        if (playerNumber == 1) player1 = player;
        else if (playerNumber == 2) player2 = player;

        int lives = (playerNumber == 1) ? player1Lives : player2Lives;
        UIManager_GameScene.Instance.UpdateLives(playerNumber, lives);
        ScoreManager.Instance.AddScore(playerNumber, 0);
        CameraManager.Instance.UpdateCameraMode();

        Debug.Log($"Player {playerNumber} respawned!");
    }

    // Handles player death and checks for game over
    public void PlayerDied(int playerNumber)
    {
        if (playerNumber == 1) player1Lives--;
        else if (playerNumber == 2) player2Lives--;

        UIManager_GameScene.Instance.UpdateLives(playerNumber,
            playerNumber == 1 ? player1Lives : player2Lives);

        // One or two player mode?
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;

        if ((!isTwoPlayer && player1Lives <= 0) ||
            (isTwoPlayer && player1Lives <= 0 && player2Lives <= 0))
        {
            ChangeState(GameState.GameOver);
        }
        else if (player1Lives > 0 && playerNumber == 1)
        {
            RespawnPlayer(1);
        }
        else if (player2Lives > 0 && playerNumber == 2)
        {
            RespawnPlayer(2);
        }
    }


    // Starts the coroutine to respawn a powerup
    public void StartPowerupRespawn(Powerup powerup)
    {
        if (powerup != null)
        {
            StartCoroutine(RespawnPowerup(powerup));
        }
    }

    // Coroutine for respawning a powerup after a delay
    private IEnumerator RespawnPowerup(Powerup powerup)
    {
        yield return new WaitForSeconds(powerupRespawnTime);
        if (powerup != null)
        {
            powerup.ResetPowerup();
        }
    }

    // Handles restart button click
    public void OnRestartButton()
    {
        Debug.Log("Restarting Game...");
        ChangeState(GameState.InitGame);
    }

    // Handles main menu button click
    public void OnMainMenuButton()
    {
        Debug.Log("Returning to Main Menu...");
        ChangeState(GameState.MainMenu);
    }
}




























