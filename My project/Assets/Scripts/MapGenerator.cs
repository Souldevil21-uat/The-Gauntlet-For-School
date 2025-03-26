using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Controls how the random seed is chosen for generation
    public enum SeedType { Preset, Random, LevelOfTheDay }

    [Header("Seed Settings")]
    public SeedType seedMode = SeedType.Preset; // Method to determine random seed
    public int presetSeed = 0; // Seed value used when SeedType is Preset

    [Header("Map Settings")]
    public int rows = 5;
    public int cols = 5;
    public float tileSize = 10f;
    public Transform playerSpawn1; // Used for avoiding obstacle spawning
    public Transform playerSpawn2;

    [Header("Prefabs")]
    public GameObject floorTilePrefab;
    public GameObject wallSegmentPrefab;
    public GameObject doorPrefab;
    public GameObject aiSpawnPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject powerupSpawnPrefab;

    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;
    public int numberOfObstacles = 30;
    public float obstacleSpacing = 1f;
    public float obstacleMinDistanceFromPlayers = 3f;

    [Header("Wall Prefab")]
    public GameObject wallPrefab;

    private GameObject[,] grid; // Stores the floor grid

    [Header("Map Segments")]
    public GameObject[] mapSegments; // List of randomizable floor tiles

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        SetSeed();
        grid = new GameObject[rows, cols];
        GenerateFloor();
        GenerateOuterWalls();
        PlaceDoors();
        PlaceSpawnPoints();
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        int spawned = 0;
        int attempts = 0;
        int maxAttempts = numberOfObstacles * 10;

        while (spawned < numberOfObstacles && attempts < maxAttempts)
        {
            attempts++;

            // Pick a random tile in grid space
            float x = Random.Range(0, cols);
            float z = Random.Range(0, rows);

            float xOffset = (cols - 1) / 2f * tileSize;
            float zOffset = (rows - 1) / 2f * tileSize;

            Vector3 position = new Vector3(x * tileSize - xOffset, 1f, z * tileSize - zOffset);

            // Don't spawn too close to player spawns
            if (Vector3.Distance(position, playerSpawn1.position) < obstacleMinDistanceFromPlayers ||
                Vector3.Distance(position, playerSpawn2.position) < obstacleMinDistanceFromPlayers)
            {
                continue;
            }

            // Pick a random obstacle prefab and instantiate it
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Instantiate(prefab, position, Quaternion.identity, transform);
            spawned++;
        }
    }

    void SetSeed()
    {
        // Choose and apply a seed based on selected method
        switch (seedMode)
        {
            case SeedType.Preset:
                Random.InitState(presetSeed);
                break;
            case SeedType.Random:
                presetSeed = Random.Range(0, int.MaxValue);
                Random.InitState(presetSeed);
                break;
            case SeedType.LevelOfTheDay:
                presetSeed = System.DateTime.UtcNow.DayOfYear;
                Random.InitState(presetSeed);
                break;
        }
    }

    private void GenerateFloor()
    {
        if (mapSegments.Length == 0)
        {
            Debug.LogError("No map segments assigned!");
            return;
        }

        float xOffset = (rows - 1) / 2f * tileSize;
        float zOffset = (cols - 1) / 2f * tileSize;

        // Create floor tiles with random segment prefabs
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 position = new Vector3(x * tileSize - xOffset, 0, y * tileSize - zOffset);
                int randomIndex = Random.Range(0, mapSegments.Length);
                grid[x, y] = Instantiate(mapSegments[randomIndex], position, Quaternion.identity, transform);
            }
        }
    }

    void GenerateOuterWalls()
    {
        if (wallPrefab == null)
        {
            Debug.LogError("No wall prefab assigned!");
            return;
        }

        float halfWidth = (cols * tileSize) / 2f;
        float halfHeight = (rows * tileSize) / 2f;

        Quaternion horizontalRot = Quaternion.Euler(0, 90, 0);
        Quaternion verticalRot = Quaternion.identity;

        // Top and bottom rows
        for (int x = 0; x < cols; x++)
        {
            float xPos = (x * tileSize) - halfWidth + tileSize / 2f;

            Vector3 topPos = new Vector3(xPos, 0, halfHeight);
            Instantiate(wallPrefab, topPos, horizontalRot, transform);

            Vector3 bottomPos = new Vector3(xPos, 0, -halfHeight);
            Instantiate(wallPrefab, bottomPos, horizontalRot, transform);
        }

        // Left and right columns
        for (int y = 0; y < rows; y++)
        {
            float zPos = (y * tileSize) - halfHeight + tileSize / 2f;

            Vector3 leftPos = new Vector3(-halfWidth, 0, zPos);
            Instantiate(wallPrefab, leftPos, verticalRot, transform);

            Vector3 rightPos = new Vector3(halfWidth, 0, zPos);
            Instantiate(wallPrefab, rightPos, verticalRot, transform);
        }
    }

    void PlaceDoors()
    {
        if (doorPrefab == null) return;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 tilePosition = grid[x, y].transform.position;

                // Place horizontal door between tiles (x direction)
                if (x < rows - 1)
                {
                    Vector3 doorPos = tilePosition + new Vector3(tileSize / 2, 0, 0);
                    Instantiate(doorPrefab, doorPos, Quaternion.Euler(0, 90, 0), transform);
                }

                // Place vertical door between tiles (y direction)
                if (y < cols - 1)
                {
                    Vector3 doorPos = tilePosition + new Vector3(0, 0, tileSize / 2);
                    Instantiate(doorPrefab, doorPos, Quaternion.identity, transform);
                }
            }
        }
    }

    void PlaceSpawnPoints()
    {
        List<Vector2Int> availableTiles = new List<Vector2Int>();

        // Track all floor grid positions
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (grid[x, y] != null)
                {
                    availableTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // Place 4 AI spawn points
        for (int i = 0; i < 4 && availableTiles.Count > 0; i++)
        {
            Vector2Int pos = GetRandomTile(availableTiles);
            PlaceSpawn(aiSpawnPrefab, pos);
        }

        // Place one player spawn
        if (availableTiles.Count > 0)
        {
            Vector2Int pos = GetRandomTile(availableTiles);
            PlaceSpawn(playerSpawnPrefab, pos);
        }

        // Place several powerup spawns
        int powerupCount = Mathf.Max(3, rows * cols / 5);
        for (int i = 0; i < powerupCount && availableTiles.Count > 0; i++)
        {
            Vector2Int pos = GetRandomTile(availableTiles);
            PlaceSpawn(powerupSpawnPrefab, pos);
        }
    }

    // Instantiates a spawnable prefab at a grid tile
    void PlaceSpawn(GameObject prefab, Vector2Int gridPos)
    {
        if (prefab == null || grid[gridPos.x, gridPos.y] == null) return;
        Vector3 pos = grid[gridPos.x, gridPos.y].transform.position + Vector3.up * 1.5f;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    // Selects and removes a random tile from the list
    Vector2Int GetRandomTile(List<Vector2Int> tiles)
    {
        int index = Random.Range(0, tiles.Count);
        Vector2Int pos = tiles[index];
        tiles.RemoveAt(index);
        return pos;
    }
}








