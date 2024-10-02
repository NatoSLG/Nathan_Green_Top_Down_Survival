using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class calculates temperature using perlin noise.
/// </summary>
public class TemperatureGenerator : MonoBehaviour
{
    public int seed;
    public float temperatureNoiseScale = 0.03f; // Scale for the temperature noise
    public float temperatureOffset = 0.5f; // Offset to adjust temperature distribution

    public void Initialize(int mapSeed)
    {
        seed = mapSeed;
    }

    public float GetTemperature(Vector2 tileCoord)
    {
        float temperatureNoise = Mathf.PerlinNoise((tileCoord.x + seed) * temperatureNoiseScale, (tileCoord.y + seed) * temperatureNoiseScale);
        return temperatureNoise + temperatureOffset;
    }
}
