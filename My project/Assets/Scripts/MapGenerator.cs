using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int rows = 5;
    public int cols = 5;
    public float tileSize = 10f;
    public bool levelOfTheDay = false;
    public int randomSeed = 0;
    public GameObject[] mapSegments; // Assign in Inspector

    [Header("Spawn Point Prefabs")]
    public GameObject aiSpawnPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject powerupSpawnPrefab;

    private GameObject[,] grid; // Stores the generated tiles

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (mapSegments.Length == 0)
        {
            Debug.LogError("No map segments assigned!");
            return;
        }

        // Set random seed (fixed if using levelOfTheDay)
        if (levelOfTheDay)
        {
            randomSeed = System.DateTime.UtcNow.DayOfYear;
        }
        Random.InitState(randomSeed);

        grid = new GameObject[rows, cols];

        // Generate map tiles
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                int randomIndex = Random.Range(0, mapSegments.Length);
                grid[x, y] = Instantiate(mapSegments[randomIndex], position, Quaternion.identity, transform);
            }
        }

        // ✅ Spawn points must be placed AFTER map generation
        PlaceSpawnPoints();
    }

    void PlaceSpawnPoints()
    {
        List<Vector2Int> availableTiles = new List<Vector2Int>();

        // Collect all open tiles (non-blocked)
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (IsTileOpen(grid[x, y])) // Only use open tiles
                {
                    availableTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        if (availableTiles.Count == 0)
        {
            Debug.LogError("No valid tiles to place spawn points!");
            return;
        }

        // Randomly place AI spawn points
        for (int i = 0; i < 4; i++) // 4 AI spawns
        {
            if (availableTiles.Count == 0) break;
            Vector2Int aiPos = GetRandomTile(availableTiles);
            PlaceSpawn(aiSpawnPrefab, aiPos);
        }

        // Randomly place Player spawn point
        if (availableTiles.Count > 0)
        {
            Vector2Int playerPos = GetRandomTile(availableTiles);
            PlaceSpawn(playerSpawnPrefab, playerPos);
        }

        // Randomly place Powerup spawn points (at least one per section)
        int numPowerups = Mathf.Max(3, rows * cols / 5); // Adjust based on map size
        for (int i = 0; i < numPowerups; i++)
        {
            if (availableTiles.Count == 0) break;
            Vector2Int powerupPos = GetRandomTile(availableTiles);
            PlaceSpawn(powerupSpawnPrefab, powerupPos);
        }

        Debug.Log("✅ Spawn points placed successfully.");
    }

    void PlaceSpawn(GameObject spawnPrefab, Vector2Int tilePos)
    {
        if (spawnPrefab == null || grid[tilePos.x, tilePos.y] == null) return;

        Vector3 spawnPosition = grid[tilePos.x, tilePos.y].transform.position + Vector3.up * 1.5f;
        Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
    }

    Vector2Int GetRandomTile(List<Vector2Int> tileList)
    {
        int index = Random.Range(0, tileList.Count);
        Vector2Int selectedTile = tileList[index];
        tileList.RemoveAt(index); // Prevent duplicate placements
        return selectedTile;
    }

    bool IsTileOpen(GameObject tile)
    {
        // Add logic to check if tile is valid (not blocked)
        return tile != null; // For now, assume all tiles are valid
    }
}



