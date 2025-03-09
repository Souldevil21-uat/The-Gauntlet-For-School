using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public enum SeedType { Preset, Random, LevelOfTheDay } // 🎲 Editor Choice
    [Header("Seed Settings")]
    public SeedType seedMode = SeedType.Preset;
    public int presetSeed = 0;

    [Header("Map Settings")]
    public int rows = 5;
    public int cols = 5;
    public float tileSize = 10f;
    public GameObject[] mapSegments; // Assign in Inspector

    [Header("Spawn Point Prefabs")]
    public GameObject aiSpawnPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject powerupSpawnPrefab;
    public GameObject doorPrefab; // ✅ Assign a door prefab in the inspector

    private GameObject[,] grid;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (mapSegments.Length == 0)
        {
            Debug.LogError("❌ No map segments assigned!");
            return;
        }

        // ✅ Choose the seed mode based on the Editor selection
        switch (seedMode)
        {
            case SeedType.Preset:
                Random.InitState(presetSeed);
                Debug.Log("📌 Using **Preset Seed**: " + presetSeed);
                break;

            case SeedType.Random:
                presetSeed = Random.Range(0, int.MaxValue);
                Random.InitState(presetSeed);
                Debug.Log("🎲 Using **Random Seed**: " + presetSeed);
                break;

            case SeedType.LevelOfTheDay:
                presetSeed = System.DateTime.UtcNow.DayOfYear;
                Random.InitState(presetSeed);
                Debug.Log("🌍 Using **Level of the Day** - Seed: " + presetSeed);
                break;
        }

        grid = new GameObject[rows, cols];

        // ✅ Generate map tiles
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                int randomIndex = Random.Range(0, mapSegments.Length);
                grid[x, y] = Instantiate(mapSegments[randomIndex], position, Quaternion.identity, transform);
            }
        }

        // ✅ Place Doors Between Segments
        PlaceDoors();

        // ✅ Place Spawn Points after map is built
        PlaceSpawnPoints();
    }

    void PlaceDoors()
    {
        if (doorPrefab == null)
        {
            Debug.LogError("❌ No door prefab assigned!");
            return;
        }

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (grid[x, y] == null) continue;

                Vector3 tilePosition = grid[x, y].transform.position;

                // ✅ Check Right Neighbor
                if (x < rows - 1 && grid[x + 1, y] != null)
                {
                    Vector3 doorPos = tilePosition + new Vector3(tileSize / 2, 0, 0);
                    Instantiate(doorPrefab, doorPos, Quaternion.Euler(0, 90, 0), transform);
                }

                // ✅ Check Top Neighbor
                if (y < cols - 1 && grid[x, y + 1] != null)
                {
                    Vector3 doorPos = tilePosition + new Vector3(0, 0, tileSize / 2);
                    Instantiate(doorPrefab, doorPos, Quaternion.identity, transform);
                }
            }
        }

        Debug.Log("🚪 Doors placed successfully!");
    }

    void PlaceSpawnPoints()
    {
        List<Vector2Int> availableTiles = new List<Vector2Int>();

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (IsTileOpen(grid[x, y]))
                {
                    availableTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        if (availableTiles.Count == 0)
        {
            Debug.LogError("❌ No valid tiles to place spawn points!");
            return;
        }

        // ✅ Place AI Spawn Points
        for (int i = 0; i < 4; i++)
        {
            if (availableTiles.Count == 0) break;
            Vector2Int aiPos = GetRandomTile(availableTiles);
            PlaceSpawn(aiSpawnPrefab, aiPos);
        }

        // ✅ Place Player Spawn Point
        if (availableTiles.Count > 0)
        {
            Vector2Int playerPos = GetRandomTile(availableTiles);
            PlaceSpawn(playerSpawnPrefab, playerPos);
        }

        // ✅ Place Powerup Spawn Points
        int numPowerups = Mathf.Max(3, rows * cols / 5);
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
        tileList.RemoveAt(index);
        return selectedTile;
    }

    bool IsTileOpen(GameObject tile)
    {
        return tile != null;
    }
}








