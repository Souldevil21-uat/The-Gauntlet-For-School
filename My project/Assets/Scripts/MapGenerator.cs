using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum SeedType { Preset, Random, LevelOfTheDay }
    [Header("Seed Settings")]
    public SeedType seedMode = SeedType.Preset;
    public int presetSeed = 0;

    [Header("Map Settings")]
    public int rows = 5;
    public int cols = 5;
    public float tileSize = 10f;

    [Header("Prefabs")]
    public GameObject floorTilePrefab;
    public GameObject wallSegmentPrefab;
    public GameObject doorPrefab;
    public GameObject aiSpawnPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject powerupSpawnPrefab;
    [Header("Wall Prefab")]
    public GameObject wallPrefab;
    private GameObject[,] grid;
    [Header("Map Segments")]
    public GameObject[] mapSegments; // Floor tiles to choose from


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
    }

    void SetSeed()
    {
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

        Quaternion horizontalRot = Quaternion.Euler(0, 90, 0); // For top & bottom
        Quaternion verticalRot = Quaternion.identity;          // For left & right

        // 🔹 Top & Bottom walls
        for (int x = 0; x < cols; x++)
        {
            float xPos = (x * tileSize) - halfWidth + tileSize / 2f;

            Vector3 topPos = new Vector3(xPos, 0, halfHeight);
            Instantiate(wallPrefab, topPos, horizontalRot, transform);

            Vector3 bottomPos = new Vector3(xPos, 0, -halfHeight);
            Instantiate(wallPrefab, bottomPos, horizontalRot, transform);
        }

        // 🔹 Left & Right walls
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
                if (x < rows - 1)
                {
                    Vector3 doorPos = tilePosition + new Vector3(tileSize / 2, 0, 0);
                    Instantiate(doorPrefab, doorPos, Quaternion.Euler(0, 90, 0), transform);
                }
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

        for (int i = 0; i < 4 && availableTiles.Count > 0; i++)
        {
            Vector2Int pos = GetRandomTile(availableTiles);
            PlaceSpawn(aiSpawnPrefab, pos);
        }

        if (availableTiles.Count > 0)
        {
            Vector2Int pos = GetRandomTile(availableTiles);
            PlaceSpawn(playerSpawnPrefab, pos);
        }

        int powerupCount = Mathf.Max(3, rows * cols / 5);
        for (int i = 0; i < powerupCount && availableTiles.Count > 0; i++)
        {
            Vector2Int pos = GetRandomTile(availableTiles);
            PlaceSpawn(powerupSpawnPrefab, pos);
        }
    }

    void PlaceSpawn(GameObject prefab, Vector2Int gridPos)
    {
        if (prefab == null || grid[gridPos.x, gridPos.y] == null) return;
        Vector3 pos = grid[gridPos.x, gridPos.y].transform.position + Vector3.up * 1.5f;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    Vector2Int GetRandomTile(List<Vector2Int> tiles)
    {
        int index = Random.Range(0, tiles.Count);
        Vector2Int pos = tiles[index];
        tiles.RemoveAt(index);
        return pos;
    }
}









