using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles where the player spawns.
/// It checks to make sure if the player is not spawning on
/// water and if the player is, move them to the nearest land tile.
/// </summary>
public class PlayerSpawnManager : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the player prefab
    public Transform mapGenerator; // Reference to the MapGenerator GameObject
    public float spawnRadius = 20f; // Radius to search for a valid spawn point
    public int maxAttempts = 100; // Maximum number of attempts to find a valid position

    private void Start()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random position within the spawn radius
            spawnPosition = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                Random.Range(-spawnRadius, spawnRadius),
                0
            );

            // Check if the position is valid (not in water)
            if (IsValidSpawnPosition(spawnPosition))
            {
                return spawnPosition;
            }
        }

        // If no valid position is found after maxAttempts, return a default position
        return Vector3.zero;
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        // Assuming your tiles are 1 unit in size; adjust if different
        float tileX = Mathf.Floor(position.x);
        float tileY = Mathf.Floor(position.y);

        // Convert the position to a tile coordinate
        Vector2 tileCoord = new Vector2(tileX, tileY);

        // Check the tile type using the MapGenerator script
        MapGenerator mapGen = mapGenerator.GetComponent<MapGenerator>();
        if (mapGen != null && mapGen.tileMapData.ContainsKey(tileCoord))
        {
            int terrainType = mapGen.tileMapData[tileCoord].terrainType;

            // Check if the terrain is not water
            if (terrainType != 0) // Assuming 0 is water
            {
                return true;
            }
        }
        else
        {
            // Debugging: Log if the tileCoord was not found
            //Debug.Log($"Tile Coord: {tileCoord} not found in tileMapData.");
        }

        return false;
    }
}
