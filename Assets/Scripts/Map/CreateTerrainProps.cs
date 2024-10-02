using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class uses Perlin Noise to determin a random location
/// for props to spawn. 
/// Here, props are determined which boime they are able to spawn in,
/// the verity, scarcity each can have.
/// </summary>
public class CreateTerrainProps
{
    MapGenerator mapGenerator;
    private float propNoiseScale = 0.01f; // Add to constructor if wanting to adjust outside the class

    private float minDistance = 3f; // Minimum distance between props
    List<Vector2> placedPropPositions = new List<Vector2>(); //Store the positions of already placed props

    // Dictionary to store the minimum distance for each prop type
    private Dictionary<GameObject, (float minDistance, float maxDistance)> propMinDistances = new Dictionary<GameObject, (float minDistance, float maxDistance)>();

    // Constructor to pass MapGenerator
    public CreateTerrainProps(MapGenerator mapGen)
    {
        mapGenerator = mapGen;
        InitializePropMinDistances();
    }

    [System.Serializable]
    public struct PropData
    {
        public int propType;
        public GameObject prefab; // The specific prefab used
        public Quaternion rotation; // The rotation of the tile
        public Vector3 position;
    }

    private const int TILE_WATER = 0;
    private const int TILE_BEACH = 1;
    private const int TILE_PLAINS = 2;
    private const int TILE_FOREST = 3;
    private const int TILE_MOUNTAIN = 4;

    public (GameObject propPrefab, Quaternion position) PlaceProps(Vector2 position, int terrainType)
    {
        // Generate a Perlin noise value for prop placement (adjust propNoiseScale accordingly)
        float noiseX = (position.x * propNoiseScale) + Random.Range(-0.1f, 0.1f);
        float noiseY = (position.y * propNoiseScale) + Random.Range(-0.1f, 0.1f);

        // Jittered position to avoid grid-like placement
        Vector2 jitteredPosition = new Vector2(
            position.x + Random.Range(-0.2f, 0.2f),
            position.y + Random.Range(-0.2f, 0.2f)
        );

        float propNoiseValue = Mathf.PerlinNoise(noiseX, noiseY);
        float propNoiseValue2 = Mathf.PerlinNoise(noiseX * 0.5f, noiseY * 0.5f);
        float propNoiseValue3 = Mathf.PerlinNoise(noiseX * 0.25f, noiseY * 0.25f);

        float combinedPropNoiseValue = (propNoiseValue + propNoiseValue2 * 0.5f + propNoiseValue3 * 0.25f) / 1.75f;

        if (combinedPropNoiseValue > 0.4f && combinedPropNoiseValue < 0.5f )
        {
            Debug.Log(combinedPropNoiseValue);
        }
        // Check the terrain type and decide on prop placement. Adjust noise to fit need.
        // If adding more props, add and adjust distance in InitializePropMinDistance
        switch (terrainType)
        {
            case TILE_MOUNTAIN:
                if (combinedPropNoiseValue > 0.45f) 
                {
                    // Randomly choose between rock, ore, and brown mushroom
                    int propType = Random.Range(0, 3); // 0 = rock, 1 = ore, 2 = brown mushroom
                    GameObject selectedProp = null;

                    switch (propType)
                    {
                        case 0: // Select a random rock
                            if (mapGenerator.terrainProps.rockPrefabs.Length > 0 && combinedPropNoiseValue > 0.48f)
                            {
                                selectedProp = mapGenerator.terrainProps.rockPrefabs[Random.Range(0, mapGenerator.terrainProps.rockPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;

                        case 1: // Select a random ore
                            if (mapGenerator.terrainProps.oreRockPrefabs.Length > 0 && combinedPropNoiseValue > 0.5f)
                            {
                                selectedProp = mapGenerator.terrainProps.oreRockPrefabs[Random.Range(0, mapGenerator.terrainProps.oreRockPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;

                        case 2: // Select brown mushroom
                            if (mapGenerator.terrainProps.brownMushroomPrefabs != null && combinedPropNoiseValue > 0.54f)
                            {
                                selectedProp = mapGenerator.terrainProps.brownMushroomPrefabs;
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                    }

                    // Check if we can place the prop based on proximity
                    bool canPlaceProp = true;
                    foreach (Vector2 placedPosition in placedPropPositions)
                    {
                        if (Vector2.Distance(placedPosition, position) < minDistance)
                        {
                            canPlaceProp = false;
                            break;
                        }
                    }

                    // If the position is valid, return the prop and store its position
                    if (canPlaceProp && selectedProp != null)
                    {
                        placedPropPositions.Add(jitteredPosition);  // Store the new prop's position
                        return (selectedProp, Quaternion.identity);
                    }
                }
                break;

            case TILE_FOREST:
                if (combinedPropNoiseValue > 0.4f) // Add tree, mushroom, and rock prefabs
                {
                    // Randomly choose between tree, red mushroom, and rock
                    int propType = Random.Range(0, 5); // 0 = tree, 1 = red mushroom, 2 = rock, 3 = fruit plants, 4 = tree stump
                    GameObject selectedProp = null;

                    switch (propType)
                    {
                        case 0: // Select a random tree
                            if (mapGenerator.terrainProps.treePrefabs.Length > 0 && combinedPropNoiseValue > 0.4f)
                            {
                                selectedProp = mapGenerator.terrainProps.treePrefabs[Random.Range(0, mapGenerator.terrainProps.treePrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;

                        case 1: // Select a random mushroom
                            if (mapGenerator.terrainProps.redMushroomPrefabs != null && combinedPropNoiseValue > 0.54f) 
                            {
                                selectedProp = mapGenerator.terrainProps.redMushroomPrefabs;
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;

                        case 2: // Select a random rock
                            if (mapGenerator.terrainProps.rockPrefabs.Length > 0 && combinedPropNoiseValue > 0.5f)
                            {
                                selectedProp = mapGenerator.terrainProps.rockPrefabs[Random.Range(0, mapGenerator.terrainProps.rockPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                        case 3: // Select random fruit
                            if (mapGenerator.terrainProps.fruitPlantPrefabs.Length > 0 && combinedPropNoiseValue > 0.54f)
                            {
                                selectedProp = mapGenerator.terrainProps.fruitPlantPrefabs[Random.Range(0, mapGenerator.terrainProps.fruitPlantPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                        case 4: // Select tree stump
                            if (mapGenerator.terrainProps.treeStumpPrefab != null && combinedPropNoiseValue > 0.54f)
                            {
                                selectedProp = mapGenerator.terrainProps.treeStumpPrefab;
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                    }

                    // Check if we can place the prop based on proximity
                    bool canPlaceProp = true;
                    foreach (Vector2 placedPosition in placedPropPositions)
                    {
                        if (Vector2.Distance(placedPosition, position) < minDistance)
                        {
                            canPlaceProp = false;
                            break;
                        }
                    }

                    // If the position is valid, return the prop and store its position
                    if (canPlaceProp && selectedProp != null)
                    {
                        placedPropPositions.Add(jitteredPosition);  // Store the new prop's position
                        return (selectedProp, Quaternion.identity);
                    }
                }
                break;

            case TILE_PLAINS:
                if (combinedPropNoiseValue > 0.4f) // Add tree and bush prefabs 
                {
                    // Randomly choose between tree or bush
                    int propType = Random.Range(0, 3); // 0 = tree, 1 = bush, 2 = fruit plants
                    GameObject selectedProp = null;

                    switch (propType)
                    {
                        case 0: // Select a random tree
                            if (mapGenerator.terrainProps.treePrefabs.Length > 0 && combinedPropNoiseValue > 0.4f)
                            {
                                selectedProp = mapGenerator.terrainProps.treePrefabs[Random.Range(0, mapGenerator.terrainProps.treePrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;

                        case 1: // Select a random bush
                            if (mapGenerator.terrainProps.bushPrefabs.Length > 0 && combinedPropNoiseValue > 0.53f)
                            {
                                selectedProp = mapGenerator.terrainProps.bushPrefabs[Random.Range(0, mapGenerator.terrainProps.bushPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                        case 2:
                            if (mapGenerator.terrainProps.fruitPlantPrefabs.Length > 0 && combinedPropNoiseValue > 0.54f)
                            {
                                selectedProp = mapGenerator.terrainProps.fruitPlantPrefabs[Random.Range(0, mapGenerator.terrainProps.fruitPlantPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                    }

                    // Check if we can place the prop based on proximity
                    bool canPlaceProp = true;
                    foreach (Vector2 placedPosition in placedPropPositions)
                    {
                        if (Vector2.Distance(placedPosition, position) < minDistance)
                        {
                            canPlaceProp = false;
                            break;
                        }
                    }

                    // If the position is valid, return the prop and store its position
                    if (canPlaceProp && selectedProp != null)
                    {
                        placedPropPositions.Add(jitteredPosition);  // Store the new prop's position
                        return (selectedProp, Quaternion.identity);
                    }
                }
                break;

            case TILE_BEACH:
                if (combinedPropNoiseValue > 0.52f) // Add cactus and rock prefabs 
                {
                    // Randomly choose between tree or bush
                    int propType = Random.Range(0, 2); // 0 = cactus, 1 = rock
                    GameObject selectedProp = null;

                    switch (propType)
                    {
                        case 0:
                            if (mapGenerator.terrainProps.cactusPrefabs.Length > 0 && combinedPropNoiseValue > 0.53f)
                            {
                                selectedProp = mapGenerator.terrainProps.cactusPrefabs[Random.Range(0, mapGenerator.terrainProps.cactusPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;

                        case 1:
                            if (mapGenerator.terrainProps.rockPrefabs.Length > 0 && combinedPropNoiseValue > 0.5f)
                            {
                                selectedProp = mapGenerator.terrainProps.rockPrefabs[Random.Range(0, mapGenerator.terrainProps.rockPrefabs.Length)];
                                var distanceRange = propMinDistances[selectedProp];
                                minDistance = Random.Range(distanceRange.minDistance, distanceRange.maxDistance);
                            }
                            break;
                    }

                    // Check if we can place the prop based on proximity
                    bool canPlaceProp = true;
                    foreach (Vector2 placedPosition in placedPropPositions)
                    {
                        if (Vector2.Distance(placedPosition, position) < minDistance)
                        {
                            canPlaceProp = false;
                            break;
                        }
                    }

                    // If the position is valid, return the prop and store its position
                    if (canPlaceProp && selectedProp != null)
                    {
                        placedPropPositions.Add(jitteredPosition);  // Store the new prop's position
                        return (selectedProp, Quaternion.identity);
                    }
                }
                break;
        }
        return (null, Quaternion.identity); // Return null if no props needed
    }

    // Initialize distance ranges for each prop type.
    // Add or edit depending on the ranges needed or prefabs added.
    void InitializePropMinDistances()
    {
        propMinDistances = new Dictionary<GameObject, (float minDistance, float maxDistance)>();

        // MOUNTAIN PROPS (Rock, ore, brown mushroom)
        foreach (var rockPrefab in mapGenerator.terrainProps.rockPrefabs)
        {
            propMinDistances[rockPrefab] = (1f, 3f); // Set the range for rocks
        }

        foreach (var oreRockPrefab in mapGenerator.terrainProps.oreRockPrefabs)
        {
            propMinDistances[oreRockPrefab] = (1f, 3f); // Set the range for ores
        }

        propMinDistances[mapGenerator.terrainProps.brownMushroomPrefabs] = (1f, 2f); // Set the range for brown mushrooms

        // FOREST PROPS (Tree, red mushroom, rock, fruit plants, tree stump)
        foreach (var treePrefab in mapGenerator.terrainProps.treePrefabs)
        {
            propMinDistances[treePrefab] = (2f, 4f); // Set the range for trees
        }

        propMinDistances[mapGenerator.terrainProps.redMushroomPrefabs] = (1f, 3f); //Set the range for red mushrooms

        foreach (var fruitPlantPrefab in mapGenerator.terrainProps.fruitPlantPrefabs)
        {
            propMinDistances[fruitPlantPrefab] = (2f, 4f); // Set the range for fruit plants
        }

        propMinDistances[mapGenerator.terrainProps.treeStumpPrefab] = (1f, 3f); // Set the range for tree stumps

        // PLAINS PROPS (Tree, bush, fruit plants)

        foreach (var bushPrefab in mapGenerator.terrainProps.bushPrefabs)
        {
            propMinDistances[bushPrefab] = (2f, 4f); // Set the range for bushes
        }

        // BEACH PROPS (Cactus, rock)
        foreach (var cactusPrefab in mapGenerator.terrainProps.cactusPrefabs)
        {
            propMinDistances[cactusPrefab] = (1f, 3f); // Set the range for cactus
        }
    }
}
