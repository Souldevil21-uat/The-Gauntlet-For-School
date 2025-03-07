using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player & AI Management")]
    public GameObject playerPrefab;
    public List<Transform> playerSpawnPoints;
    public GameObject[] aiTankPrefabs; // Array of AI tank prefabs
    public List<Transform> aiSpawnPoints;

    private List<GameObject> spawnedAI = new List<GameObject>(); // Keep track of AI instances

    [Header("Powerup Management")]
    public List<Transform> powerupSpawnPoints;
    public GameObject[] powerupPrefabs;
    public float powerupRespawnTime = 10f;

    public List<PlayerController> playerControllers = new List<PlayerController>();
    public List<TankPawn> tankPawns = new List<TankPawn>();
    public List<AIController> aiControllers = new List<AIController>();

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
    }

    private void Start()
    {
        SpawnPlayer();
        SpawnAITanks();
        SpawnPowerups();
    }

    // ✅ Spawns player at a random location
    public void SpawnPlayer()
    {
        if (playerPrefab == null || playerSpawnPoints.Count == 0)
        {
            Debug.LogError("❌ Player prefab or spawn points missing!");
            return;
        }

        Transform randomSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
        Instantiate(playerPrefab, randomSpawn.position, randomSpawn.rotation);
    }

    // ✅ Respawn player at a random location when they die
    public void RespawnPlayer(GameObject player)
    {
        Transform randomSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
        player.transform.position = randomSpawn.position;
        player.transform.rotation = randomSpawn.rotation;
    }

    // ✅ Spawns 4 AI tanks with different personalities
    private void SpawnAITanks()
    {
        if (aiTankPrefabs.Length < 4 || aiSpawnPoints.Count < 4)
        {
            Debug.LogError("❌ Not enough AI tank prefabs or spawn points!");
            return;
        }

        List<Transform> availableSpawns = new List<Transform>(aiSpawnPoints);
        ShuffleList(availableSpawns); // Randomize spawn points

        for (int i = 0; i < 4; i++)
        {
            Transform spawnPoint = availableSpawns[i];
            GameObject aiTank = Instantiate(aiTankPrefabs[i], spawnPoint.position, spawnPoint.rotation);
            spawnedAI.Add(aiTank);
            RegisterAI(aiTank.GetComponent<AIController>());
        }
    }

    // ✅ Prevents AI from patrolling between sections
    public void RestrictAIToSection(AIController ai, Transform sectionCenter)
    {
        if (ai == null) return;
        float sectionRadius = 20f; // Adjust this value based on section size

        if (Vector3.Distance(ai.transform.position, sectionCenter.position) > sectionRadius)
        {
            ai.transform.position = sectionCenter.position;
        }
    }

    // ✅ Spawns powerups at random locations
    private void SpawnPowerups()
    {
        if (powerupPrefabs.Length == 0 || powerupSpawnPoints.Count == 0)
        {
            Debug.LogError("❌ No powerup prefabs or spawn points assigned!");
            return;
        }

        foreach (Transform spawnPoint in powerupSpawnPoints)
        {
            GameObject powerup = Instantiate(
                powerupPrefabs[Random.Range(0, powerupPrefabs.Length)],
                spawnPoint.position,
                spawnPoint.rotation
            );

            StartCoroutine(RespawnPowerup(powerup, spawnPoint));
        }
    }

    // ✅ Handles powerup respawning after pickup
    private System.Collections.IEnumerator RespawnPowerup(GameObject powerup, Transform spawnPoint)
    {
        yield return new WaitForSeconds(powerupRespawnTime);
        Instantiate(powerup, spawnPoint.position, spawnPoint.rotation);
    }

    // ✅ Utility function to shuffle a list (randomize spawn points)
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    // ✅ Registers a tank in the game
    public void RegisterTank(TankPawn tank)
    {
        if (tank != null && !tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    // ✅ Unregisters a tank
    public void UnregisterTank(TankPawn tank)
    {
        if (tank != null && tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }

    // ✅ Registers an AI controller
    public void RegisterAI(AIController ai)
    {
        if (ai != null && !aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
        }
    }

    // ✅ Unregisters an AI controller
    public void UnregisterAI(AIController ai)
    {
        if (ai != null && aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
        }
    }

    // ✅ Returns the first registered player controller
    public PlayerController GetPlayer()
    {
        if (playerControllers.Count > 0 && playerControllers[0] != null)
        {
            return playerControllers[0];
        }
        return null;
    }

    // ✅ Registers a player controller
    public void RegisterPlayer(PlayerController player)
    {
        if (player != null && !playerControllers.Contains(player))
        {
            playerControllers.Add(player);
        }
    }

    // ✅ Unregisters a player controller
    public void UnregisterPlayer(PlayerController player)
    {
        if (player != null && playerControllers.Contains(player))
        {
            playerControllers.Remove(player);
        }
    }

    // ✅ Returns the AI controller closest to a given position
    public AIController GetNearestAI(Vector3 position)
    {
        if (aiControllers.Count == 0) return null;

        AIController nearestAI = null;
        float minDistance = Mathf.Infinity;

        foreach (AIController ai in aiControllers)
        {
            if (ai == null) continue;

            float distance = Vector3.Distance(position, ai.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAI = ai;
            }
        }
        return nearestAI;
    }

    // ✅ Returns the AI controller with the highest health
    public AIController GetStrongestAI()
    {
        if (aiControllers.Count == 0) return null;

        AIController strongestAI = null;
        float highestHealth = 0f;

        foreach (AIController ai in aiControllers)
        {
            if (ai == null) continue;

            Health aiHealth = ai.GetComponent<Health>();
            if (aiHealth != null && aiHealth.currentHealth > highestHealth)
            {
                highestHealth = aiHealth.currentHealth;
                strongestAI = ai;
            }
        }
        return strongestAI;
    }

    // ✅ Returns a list of AI controllers that are currently in ChaseState
    public List<AIController> GetAllChasingAI()
    {
        List<AIController> chasingAI = new List<AIController>();

        foreach (AIController ai in aiControllers)
        {
            if (ai != null && ai.currentState is ChaseState)
            {
                chasingAI.Add(ai);
            }
        }
        return chasingAI;
    }
}






