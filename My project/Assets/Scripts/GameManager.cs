using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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

    // References to active player objects
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
        Debug.Log("GameManager Started");
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            Debug.LogWarning($"Game is already in {newState} state. Ignoring request.");
            return;
        }

        Debug.Log($"Changing Game State: {currentState} ➡ {newState}");
        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Debug.Log("Loading Main Menu...");
                SceneManager.LoadScene("StartScene");
                break;

            case GameState.OptionsMenu:
                Debug.Log("Loading Options Menu...");
                SceneManager.LoadScene("OptionScene");
                break;

            case GameState.InitGame:
                Debug.Log("Initializing Game...");
                StartCoroutine(InitializeGame());
                break;

            case GameState.PlayGame:
                Debug.Log("Game Started!");
                break;

            case GameState.GameOver:
                Debug.Log("Game Over triggered.");
                ShowGameOverScreen();
                break;
        }
    }


    private IEnumerator InitializeGame()
    {
        Debug.Log("Initializing Game...");

        // ✅ Load scene asynchronously so we can wait for it to finish
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Game");

        // Wait until the scene is done loading
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // ✅ Wait an extra frame just to ensure objects are initialized
        yield return null;

        // ✅ Re-apply saved volume settings after scene is loaded
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(sfxVol);
            AudioManager.Instance.SetMusicVolume(musicVol);
        }
        else
        {
            Debug.LogWarning("AudioManager not found in Game Scene!");
        }

        // ✅ Now everything in the scene is ready
        FindGameUIManager();

        // 🔹 Find Player Spawn Points
        GameObject[] playerSpawnObjects = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        playerSpawnPoints = new List<Transform>();
        foreach (GameObject obj in playerSpawnObjects)
        {
            playerSpawnPoints.Add(obj.transform);
        }

        // 🔹 Find AI Spawn Points
        GameObject[] aiSpawnObjects = GameObject.FindGameObjectsWithTag("AI Spawn Point");
        aiSpawnPoints = new List<Transform>();
        foreach (GameObject obj in aiSpawnObjects)
        {
            aiSpawnPoints.Add(obj.transform);
        }

        // 🔹 Find Powerup Spawn Points
        GameObject[] powerupSpawnObjects = GameObject.FindGameObjectsWithTag("PowerupSpawn");
        powerupSpawnPoints = new List<Transform>();
        foreach (GameObject obj in powerupSpawnObjects)
        {
            powerupSpawnPoints.Add(obj.transform);
        }

        ApplyTwoPlayerMode();
        SpawnPlayers();
        SpawnAI();
        SpawnPowerups();

        CameraManager.Instance.UpdateCameraMode();

        ChangeState(GameState.PlayGame);
    }





    public void SpawnPlayers()
    {
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;

        if (playerPrefab == null || playerSpawnPoints.Count < 2)
        {
            Debug.LogError("Not enough player spawn points or missing player prefab!");
            return;
        }

        // Spawn Player 1
        Transform spawn1 = playerSpawnPoints[0];
        player1 = Instantiate(playerPrefab, spawn1.position, spawn1.rotation);
        PlayerController player1Controller = player1.GetComponent<PlayerController>();
        player1Controller.playerNumber = 1;
        RegisterPlayer(player1Controller);
        Debug.Log("Player 1 Spawned!");

        // Spawn Player 2 if Two Player Mode is Enabled
        if (isTwoPlayer)
        {
            Transform spawn2 = playerSpawnPoints[1];
            player2 = Instantiate(playerPrefab, spawn2.position, spawn2.rotation);
            player2.tag = "Player 2";

            PlayerController player2Controller = player2.GetComponent<PlayerController>();
            player2Controller.playerNumber = 2;
            RegisterPlayer(player2Controller);
            Debug.Log("Player 2 Spawned!");
        }
    }

    public void SpawnAI()
    {
        if (aiTankPrefabs.Count == 0 || aiSpawnPoints.Count == 0)
        {
            Debug.LogWarning("Missing AI tank prefabs or spawn points.");
            return;
        }

        void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[rand];
                list[rand] = temp;
            }
        }

        List<GameObject> guaranteed = new List<GameObject>();
        HashSet<System.Type> addedTypes = new HashSet<System.Type>();

        // 🔹 Guarantee one of each AI type
        foreach (GameObject prefab in aiTankPrefabs)
        {
            AIController controller = prefab.GetComponent<AIController>();
            if (controller == null) continue;

            System.Type type = controller.GetType();
            if (!addedTypes.Contains(type))
            {
                guaranteed.Add(prefab);
                addedTypes.Add(type);
            }
        }

        // 🔹 Shuffle extra prefabs
        List<GameObject> extras = new List<GameObject>(aiTankPrefabs);
        ShuffleList(extras);

        // 🔹 Fill up to max (but not more than spawn points)
        int maxSpawn = Mathf.Min(4, aiSpawnPoints.Count); // Or raise 4 if needed
        while (guaranteed.Count < maxSpawn)
        {
            GameObject extra = extras[Random.Range(0, extras.Count)];
            guaranteed.Add(extra);
        }

        // 🔹 Shuffle spawn points
        List<Transform> shuffledSpawnPoints = new List<Transform>(aiSpawnPoints);
        ShuffleList(shuffledSpawnPoints);

        // 🔹 Spawn each AI at a shuffled spawn point
        for (int i = 0; i < guaranteed.Count; i++)
        {
            GameObject aiTank = Instantiate(guaranteed[i], shuffledSpawnPoints[i].position, shuffledSpawnPoints[i].rotation);
            AIController aiController = aiTank.GetComponent<AIController>();

            if (aiController == null)
            {
                Debug.LogError("Spawned AI tank missing AIController!");
                continue;
            }

            if (aiController is AIPatrolChase patrolAI)
            {
                patrolAI.patrolPoints = new List<Transform>();
                foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
                {
                    patrolAI.patrolPoints.Add(wp.transform);
                }
                ShuffleList(patrolAI.patrolPoints);
            }
            else if (aiController is AIFlee fleeAI)
            {
                fleeAI.patrolPoints = new List<Transform>();
                foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
                {
                    fleeAI.patrolPoints.Add(wp.transform);
                }
                ShuffleList(fleeAI.patrolPoints);
            }

            RegisterAI(aiController);
        }
    }


    // Utility method to shuffle a list
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




    public void SpawnPowerups()
    {
        foreach (Transform spawnPoint in powerupSpawnPoints)
        {
            GameObject powerup = Instantiate(powerupPrefab, spawnPoint.position, spawnPoint.rotation);

            Powerup powerupScript = powerup.GetComponent<Powerup>();
            if (powerupScript != null)
            {
                powerupScript.type = (Powerup.PowerupType)Random.Range(0, System.Enum.GetValues(typeof(Powerup.PowerupType)).Length);
                powerupScript.SetRespawn(spawnPoint, powerupRespawnTime);
            }

            Debug.Log("Powerup Spawned!");
        }
    }


    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        finalScoreText.text = $"Final Score: {playerScores[1]}";
        Debug.Log("Game Over Screen Shown");
    }

    public void RegisterPlayer(PlayerController player)
    {
        if (!playerControllers.Contains(player))
        {
            playerControllers.Add(player);
            Debug.Log($"Registered Player {player.playerNumber}.");
        }
    }

    public void RegisterAI(AIController ai)
    {
        if (!aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
        }
    }

    public void UnregisterAI(AIController ai)
    {
        if (aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
        }
    }

    public void RegisterTank(TankPawn tank)
    {
        if (!tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    public void UnregisterTank(TankPawn tank)
    {
        if (tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }

    public void ApplyTwoPlayerMode()
    {
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;
        Debug.Log($"Two Player Mode: {(isTwoPlayer ? "Enabled" : "Disabled")}");
    }

    public int GetPlayerCount()
    {
        return playerControllers.Count;
    }

    public PlayerController GetPlayer()
    {
        return playerControllers.Count > 0 ? playerControllers[0] : null;
    }

    public void RespawnPlayer(int playerNumber)
    {
        if (playerSpawnPoints.Count < 2)
        {
            Debug.LogError("Not enough spawn points for respawn!");
            return;
        }

        Transform spawnPoint = (playerNumber == 1) ? playerSpawnPoints[0] : playerSpawnPoints[1];

        if (spawnPoint == null)
        {
            Debug.LogError($"Spawn point for Player {playerNumber} is missing!");
            return;
        }

        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.playerNumber = playerNumber;
        RegisterPlayer(playerController);
        Debug.Log($"Player {playerNumber} respawned!");
    }

    public void PlayerDied(int playerNumber)
    {
        if (playerNumber == 1) player1Lives--;
        else if (playerNumber == 2) player2Lives--;

        if (player1Lives <= 0 && player2Lives <= 0)
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

    public void StartPowerupRespawn(Powerup powerup)
    {
        if (powerup != null)
        {
            StartCoroutine(RespawnPowerup(powerup));
        }
    }

    private IEnumerator RespawnPowerup(Powerup powerup)
{
    yield return new WaitForSeconds(powerupRespawnTime);
    if (powerup != null)
    {
        powerup.ResetPowerup();
    }
}


    private void FindGameUIManager()
    {
        gameUIManager = FindObjectOfType<UIManager_GameScene>();
    }
}



























