using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenBackup : MonoBehaviour
{
    [System.Serializable]
    public struct TileData
    {
        public int terrainType; // 0 = water, 1 = beach, etc.
        public GameObject prefab; // The specific prefab used
        public Quaternion rotation; // The rotation of the tile
    }

    [Header("Map Generator")]
    public GameObject mapGenerator; // Reference to the MapGenerator GameObject
    public TemperatureGenerator temperatureGenerator; // Reference to the TemperatureGenerator script

    [Header("Culling/Despawn")]
    public Transform player; // Reference to the player's transform
    public float cullingDistance = 30f; // Distance from the player to keep tiles active

    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>(); // Dictionary to keep track of active tiles
    [HideInInspector] public Dictionary<Vector2, TileData> tileMapData = new Dictionary<Vector2, TileData>(); // Dictionary to store tile data
    private HashSet<Vector2> activeChunks = new HashSet<Vector2>(); // Set to keep track of active chunks

    private const int TILE_WATER = 0;
    private const int TILE_BEACH = 1;
    private const int TILE_PLAINS = 2;
    private const int TILE_FOREST = 3;
    private const int TILE_MOUNTAIN = 4;

    [Header("Map Values")]
    public int chunkSize = 10; //Size of each chunk
    public int seed;
    public float noiseScale = 0.02f;
    public float noiseScale2 = 0.05f; // Second noise layer scale
    public float noiseScale3 = 0.1f; // Third noise layer scale

    //Change size of neighborCount in GetMostCommonNeighbor depending on the amount of prefabs
    //Array to store different types of prefabs of the same tile type
    [Header("Tilemap Prefab")]
    public GameObject[] waterPrefab;
    public GameObject[] beachPrefab;
    public GameObject[] plainsPrefab;
    public GameObject[] forestPrefab;
    public GameObject[] mountainPrefab;

    [System.Serializable]
    public class BeachEdgePrefabs
    {
        [Header("Edge Prefabs")]
        // Special prefab for beach-to-water edges
        public GameObject beachToWaterSide;
        public GameObject beachToWaterInner;
        public GameObject beachToWaterOneOuter;
        public GameObject beachToWaterTwoOuter;
        public GameObject beachToWaterThreeOuter;
        public GameObject beachToWaterFourOuter;
        public GameObject beachToWaterUShape;
    }
    // Add other transition prefabs here...

    public BeachEdgePrefabs beachEdgePrefabs;

    void Awake()
    {
        if (seed == 0)
        {
            seed = Random.Range(0, 100000);
        }

        // Check if temperatureGenerator is not assigned
        if (temperatureGenerator == null)
        {
            // Create a new instance of TemperatureGenerator
            temperatureGenerator = new TemperatureGenerator();
        }

        // Initialize the TemperatureGenerator with the same seed
        temperatureGenerator.Initialize(seed);

        // Initial map generation
        SpawnTiles();
    }

    private void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }

    private void Update()
    {
        // Call the culling function in each frame to check for unloaded tiles
        // Ensure tiles are spawned first before culling
        SpawnTiles();
        CullTiles();
    }


    // Respawn tiles when the player returns to the area
    void SpawnTiles()
    {
        Vector3 playerPosition = player.position;
        Vector2 playerChunkCoord = new Vector2(Mathf.Floor(playerPosition.x / chunkSize), Mathf.Floor(playerPosition.y / chunkSize));

        // Load chunks around the player
        int chunksInView = Mathf.CeilToInt(cullingDistance / chunkSize);

        for (int x = -chunksInView; x <= chunksInView; x++)
        {
            for (int y = -chunksInView; y <= chunksInView; y++)
            {
                Vector2 chunkCoord = new Vector2(playerChunkCoord.x + x, playerChunkCoord.y + y);

                // Check if this chunk is already loaded
                if (!activeChunks.Contains(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                    activeChunks.Add(chunkCoord);
                }
            }
        }
    }

    void GenerateChunk(Vector2 chunkCoord)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 tileCoord = new Vector2(chunkCoord.x * chunkSize + x, chunkCoord.y * chunkSize + y);
                Vector3 position = new Vector3(tileCoord.x, tileCoord.y, 0);

                if (!tileMapData.ContainsKey(tileCoord))
                {
                    float noise1 = Mathf.PerlinNoise((tileCoord.x + seed) * noiseScale, (tileCoord.y + seed) * noiseScale);
                    float noise2 = Mathf.PerlinNoise((tileCoord.x + seed) * noiseScale2, (tileCoord.y + seed) * noiseScale2);
                    float noise3 = Mathf.PerlinNoise((tileCoord.x + seed) * noiseScale3, (tileCoord.y + seed) * noiseScale3);
                    float combinedNoise = (noise1 + noise2 * 0.5f + noise3 * 0.25f) / 1.75f;

                    // Get the temperature from the TemperatureGenerator
                    float temperature = 0f;
                    if (temperatureGenerator != null)
                    {
                        temperature = temperatureGenerator.GetTemperature(tileCoord);
                    }
                    else
                    {
                        Debug.LogWarning("TemperatureGenerator is not assigned.");
                    }

                    int terrainType;
                    GameObject tile = null;

                    if (combinedNoise > 0.65f)
                    {
                        // Hot or cold mountains (this can be changed with if statements
                        //to control a specific type of prefab selected)
                        terrainType = temperature > 0.6f ? TILE_MOUNTAIN : TILE_MOUNTAIN;
                        tile = GetRandomPrefab(mountainPrefab);
                    }
                    else if (combinedNoise > 0.50f)
                    {
                        terrainType = TILE_FOREST;
                        tile = GetRandomPrefab(forestPrefab);
                    }
                    else if (combinedNoise > 0.40f)
                    {
                        terrainType = TILE_PLAINS;
                        tile = GetRandomPrefab(plainsPrefab);
                    }
                    else if (combinedNoise > 0.35f)
                    {
                        terrainType = TILE_BEACH;
                        tile = GetRandomPrefab(beachPrefab);
                    }
                    else
                    {
                        terrainType = TILE_WATER;
                        tile = GetRandomPrefab(waterPrefab);
                    }

                    // Check for biome edges and place transition tiles with rotation
                    var (edgeTilePrefab, rotation) = GetEdgeTile(tileCoord, terrainType);
                    GameObject prefabToInstantiate = edgeTilePrefab ?? tile;
                    Quaternion prefabRotation = edgeTilePrefab != null ? rotation : Quaternion.identity;

                    GameObject spawnedTile = Instantiate(prefabToInstantiate, position, prefabRotation);
                    spawnedTile.name = string.Format("{0} x{1}, y{2}", tile.name, x, y);
                    spawnedTile.transform.parent = mapGenerator.transform; // Set the parent to MapGenerator
                    activeTiles[tileCoord] = spawnedTile;

                    // Store detailed tile data
                    tileMapData[tileCoord] = new TileData
                    {
                        terrainType = terrainType,
                        prefab = prefabToInstantiate,
                        rotation = prefabRotation
                    };

                    activeTiles[tileCoord] = spawnedTile;
                }
                else
                {
                    // Reload existing tile data
                    TileData tileData = tileMapData[tileCoord];
                    GameObject spawnedTile = Instantiate(tileData.prefab, position, tileData.rotation);
                    spawnedTile.name = string.Format("{0} x{1}, y{2}", tileData.prefab.name, x, y);
                    spawnedTile.transform.parent = mapGenerator.transform;
                    activeTiles[tileCoord] = spawnedTile;
                }
            }
        }
    }

    GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        int index = Random.Range(0, prefabs.Length);
        return prefabs[index];
    }

    GameObject GetPrefabFromType(int terrainType)
    {
        switch (terrainType)
        {
            case 0:
                return GetRandomPrefab(waterPrefab);
            case 1:
                return GetRandomPrefab(beachPrefab);
            case 2:
                return GetRandomPrefab(plainsPrefab);
            case 3:
                return GetRandomPrefab(forestPrefab);
            case 4:
                return GetRandomPrefab(mountainPrefab);
            default:
                return GetRandomPrefab(waterPrefab);
        }
    }

    //Implement Culling to save memory and improve performance,
    //implement culling to unload tiles that are far from the player.
    void CullTiles()
    {
        Vector3 playerPosition = player.position;
        List<Vector2> tilesToRemove = new List<Vector2>();

        foreach (var tile in activeTiles)
        {
            float distance = Vector3.Distance(playerPosition, tile.Value.transform.position);

            // Adjust this to account for chunks, not just individual tiles
            if (distance > cullingDistance + chunkSize * 1.5f)
            {
                Destroy(tile.Value);
                tilesToRemove.Add(tile.Key);
            }
        }

        foreach (var tileCoord in tilesToRemove)
        {
            activeTiles.Remove(tileCoord);
        }

        List<Vector2> chunksToUnload = new List<Vector2>();
        foreach (var chunkCoord in activeChunks)
        {
            Vector2 chunkCenter = chunkCoord * chunkSize + new Vector2(chunkSize / 2, chunkSize / 2);
            float distanceToChunk = Vector2.Distance(playerPosition, chunkCenter);

            // Adjust the unload distance if chunks are unloading too quickly
            if (distanceToChunk > cullingDistance * 1.5f)
            {
                chunksToUnload.Add(chunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            UnloadChunk(chunkCoord);
        }
    }

    void UnloadChunk(Vector2 chunkCoord)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 tileCoord = new Vector2(chunkCoord.x * chunkSize + x, chunkCoord.y * chunkSize + y);
                if (activeTiles.ContainsKey(tileCoord))
                {
                    Destroy(activeTiles[tileCoord]);
                    activeTiles.Remove(tileCoord);
                }
            }
        }

        activeChunks.Remove(chunkCoord);
    }

    (GameObject tilePrefab, Quaternion rotation) GetEdgeTile(Vector2 tileCoord, int terrainType)
    {
        // Check adjacent tiles to determine if this is an edge tile
        int left = GetTerrainType(tileCoord + Vector2.left);
        int right = GetTerrainType(tileCoord + Vector2.right);
        int up = GetTerrainType(tileCoord + Vector2.up);
        int down = GetTerrainType(tileCoord + Vector2.down);

        // Check diagonal tiles to determine if this is an edge tile
        int bottomLeft = GetTerrainType(tileCoord + Vector2.down + Vector2.left);
        int bottomRight = GetTerrainType(tileCoord + Vector2.down + Vector2.right);
        int topLeft = GetTerrainType(tileCoord + Vector2.up + Vector2.left);
        int topRight = GetTerrainType(tileCoord + Vector2.up + Vector2.right);

        if (terrainType == TILE_WATER)
        {
            // Define tiles and their expected types
            var tileCheck = new Dictionary<string, int>
            {
                { "left", left },
                { "right", right },
                { "up", up },
                { "down", down },
                { "bottomLeft", bottomLeft },
                { "bottomRight", bottomRight },
                { "topRight", topRight },
                { "topLeft", topLeft }
            };
            // LEFT U TILE
            var expectedLeftUTile = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // RIGHT U TILE
            var expectedRightUTile = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // UP U TILE
            var expectedUpUTile = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // DOWN U TILE
            var expectedDownUTile = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // TOP RIGHT INNER
            var expectedTopRightInner = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // TOP LEFT INNER
            var expectedTopLeftInner = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // BOTTOM RIGHT INNER
            var expectedBottomRightInner = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_WATER } }
            };
            // BOTTOM LEFT INNER
            var expectedBottomLeftInner = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // TOP RIGHT OUTER
            var expectedTopRightOuter = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // TOP LEFT OUTER
            var expectedTopLeftOuter = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // BOTTOM RIGHT OUTER
            var expectedBottomRightOuter = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };
            // BOTTOM LEFT OUTER
            var expectedBottomLeftOuter = new Dictionary<string, List<int>>
            {
                { "left", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomLeft", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // LEFT SIDE
            var expectedLeft = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER} },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_WATER } }
            };
            // RIGHT SIDE
            var expectedRight = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_WATER } },
                { "topRight", new List<int> {TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // DOWN SIDE
            var expectedDown = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_WATER } },
                { "bottomRight", new List<int> {TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };
            // UP SIDE
            var expectedUp = new Dictionary<string, List<int>>
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_WATER } },
                { "topLeft", new List<int> {TILE_WATER } }
            };


            // PRIORITIZE UNIQUE TILES
            if (CheckAdjacentTiles(tileCheck, expectedLeftUTile)) // Left U Tile
            {
                return (beachEdgePrefabs.beachToWaterUShape, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedRightUTile)) // Right U Tile
            {
                return (beachEdgePrefabs.beachToWaterUShape, Quaternion.identity);
            }
            else if (CheckAdjacentTiles(tileCheck, expectedUpUTile)) // Up U Tile
            {
                return (beachEdgePrefabs.beachToWaterUShape, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedDownUTile)) // Down U Tile
            {
                return (beachEdgePrefabs.beachToWaterUShape, Quaternion.Euler(0f, 0f, -90f));
            }
            // Inward Corners
            else if (CheckAdjacentTiles(tileCheck, expectedTopRightInner)) // Top-right inner corner
            {
                return (beachEdgePrefabs.beachToWaterInner, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedTopLeftInner)) // Top-Left inner  corner
            {
                return (beachEdgePrefabs.beachToWaterInner, Quaternion.Euler(0f, 0f, -90f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedBottomRightInner)) // Bottom-Right inner corner
            {
                return (beachEdgePrefabs.beachToWaterInner, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedBottomLeftInner)) // Bottom-Left inner corner
            {
                return (beachEdgePrefabs.beachToWaterInner, Quaternion.identity);
            }
            // Outward corners
            else if (CheckAdjacentTiles(tileCheck, expectedBottomRightOuter)) // Bottom-Right outward corner
            {
                return (beachEdgePrefabs.beachToWaterOneOuter, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedBottomLeftOuter)) //Bottom-Left outward corner
            {
                return (beachEdgePrefabs.beachToWaterOneOuter, Quaternion.identity);
            }
            else if (CheckAdjacentTiles(tileCheck, expectedTopRightOuter)) // Top-Right outward corner
            {
                return (beachEdgePrefabs.beachToWaterOneOuter, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (CheckAdjacentTiles(tileCheck, expectedTopLeftOuter)) // Top-Left outward corner
            {
                return (beachEdgePrefabs.beachToWaterOneOuter, Quaternion.Euler(0f, 0f, -90f));
            }
            // Handle straight edges
            else if (CheckAdjacentTiles(tileCheck, expectedRight))
            {
                return (beachEdgePrefabs.beachToWaterSide, Quaternion.identity); // Face Right (default rotation)
            }
            else if (CheckAdjacentTiles(tileCheck, expectedLeft))
            {
                return (beachEdgePrefabs.beachToWaterSide, Quaternion.Euler(0f, 0f, 180f)); // Face Left
            }
            else if (CheckAdjacentTiles(tileCheck, expectedDown))
            {
                return (beachEdgePrefabs.beachToWaterSide, Quaternion.Euler(0f, 0f, -90f)); // Face down
            }
            else if (CheckAdjacentTiles(tileCheck, expectedUp))
            {
                return (beachEdgePrefabs.beachToWaterSide, Quaternion.Euler(0f, 0f, 90f)); // Face up 
            }
        }
        //add other conditions for different transitions here

        return (null, Quaternion.identity); // Return null if no edge tile is needed
    }

    int GetTerrainType(Vector2 position)
    {
        if (tileMapData.ContainsKey(position))
        {
            return tileMapData[position].terrainType;
        }
        else
        {
            // Generate a "dummy" noise value to simulate what type would be generated here
            float noise1 = Mathf.PerlinNoise((position.x + seed) * noiseScale, (position.y + seed) * noiseScale);
            float noise2 = Mathf.PerlinNoise((position.x + seed) * noiseScale2, (position.y + seed) * noiseScale2);
            float noise3 = Mathf.PerlinNoise((position.x + seed) * noiseScale3, (position.y + seed) * noiseScale3);
            float combinedNoise = (noise1 + noise2 * 0.5f + noise3 * 0.25f) / 1.75f;

            if (combinedNoise > 0.65f) return 4;
            if (combinedNoise > 0.50f) return 3;
            if (combinedNoise > 0.40f) return 2;
            if (combinedNoise > 0.35f) return 1;
            return 0;
        }
    }

    bool CheckAdjacentTiles(Dictionary<string, int> tilesToCheck, Dictionary<string, List<int>> expectedTypes)
    {
        foreach (var direction in tilesToCheck.Keys)
        {
            if (!expectedTypes[direction].Contains(tilesToCheck[direction]))
            {
                return false; // If any tile doesn't match, return false
            }
        }
        return true; // All tiles match
    }
}
