using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player & AI Management")]
    public GameObject playerPrefab;
    public List<Transform> playerSpawnPoints;
    public GameObject[] aiTankPrefabs;
    public List<Transform> aiSpawnPoints;

    private List<GameObject> spawnedAI = new List<GameObject>();

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
        if (GameObject.FindWithTag("Player") == null)  // ✅ Prevents double player spawning
        {
            SpawnPlayer();
        }

        if (GameObject.FindWithTag("AI") == null)  // ✅ Prevents double AI spawning
        {
            SpawnAITanks();
        }

        StartCoroutine(SpawnPowerupsWithDelay());
    }

    // ✅ Spawns powerups with a random delay to avoid all appearing at once
    private IEnumerator SpawnPowerupsWithDelay()
    {
        if (powerupPrefabs.Length == 0 || powerupSpawnPoints.Count == 0)
        {
            Debug.LogError("❌ No powerup prefabs or spawn points assigned!");
            yield break;
        }

        foreach (Transform spawnPoint in powerupSpawnPoints)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f)); // ✅ Random delay between 1-5 seconds
            SpawnPowerup(spawnPoint);
        }
    }

    // ✅ Spawns a powerup at a given location
    private void SpawnPowerup(Transform spawnPoint)
    {
        if (powerupPrefabs.Length == 0) return;

        GameObject powerup = Instantiate(
            powerupPrefabs[Random.Range(0, powerupPrefabs.Length)],
            spawnPoint.position,
            spawnPoint.rotation
        );

        Powerup powerupScript = powerup.GetComponent<Powerup>();
        if (powerupScript != null)
        {
            powerupScript.SetRespawn(spawnPoint, powerupRespawnTime);
        }
    }

    public void StartPowerupRespawn(Powerup powerup)
    {
        StartCoroutine(RespawnPowerup(powerup));
    }

    private IEnumerator RespawnPowerup(Powerup powerup)
    {
        yield return new WaitForSeconds(powerupRespawnTime);
        powerup.gameObject.SetActive(true);
    }


    // ✅ Player Spawning
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

    public void RespawnPlayer(GameObject player)
    {
        Transform randomSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
        player.transform.position = randomSpawn.position;
        player.transform.rotation = randomSpawn.rotation;
    }

    // ✅ AI Spawning
    private void SpawnAITanks()
    {
        if (aiTankPrefabs.Length < 4 || aiSpawnPoints.Count < 4)
        {
            Debug.LogError("❌ Not enough AI tank prefabs or spawn points!");
            return;
        }

        List<Transform> availableSpawns = new List<Transform>(aiSpawnPoints);
        ShuffleList(availableSpawns);

        for (int i = 0; i < 4; i++)
        {
            Transform spawnPoint = availableSpawns[i];
            GameObject aiTank = Instantiate(aiTankPrefabs[i], spawnPoint.position, spawnPoint.rotation);
            spawnedAI.Add(aiTank);
            RegisterAI(aiTank.GetComponent<AIController>());
        }
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

    // ✅ Powerup Respawn System
    public IEnumerator RespawnPowerup(Transform spawnPoint)
    {
        yield return new WaitForSeconds(powerupRespawnTime);
        SpawnPowerup(spawnPoint);
    }

    // ✅ Player, AI, and Tank Registration
    public void RegisterTank(TankPawn tank)
    {
        if (tank != null && !tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    public void UnregisterTank(TankPawn tank)
    {
        if (tank != null && tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }

    public void RegisterAI(AIController ai)
    {
        if (ai != null && !aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
        }
    }

    public void UnregisterAI(AIController ai)
    {
        if (ai != null && aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
        }
    }

    public PlayerController GetPlayer()
    {
        if (playerControllers.Count > 0 && playerControllers[0] != null)
        {
            return playerControllers[0];
        }
        return null;
    }

    public void RegisterPlayer(PlayerController player)
    {
        if (player != null && !playerControllers.Contains(player))
        {
            playerControllers.Add(player);
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (player != null && playerControllers.Contains(player))
        {
            playerControllers.Remove(player);
        }
    }

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






