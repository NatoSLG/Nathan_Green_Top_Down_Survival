using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateBeachEdgeTiles
{
    CreateEdgeTiles createEdgeTiles;
    MapGenerator mapGenerator;

    private const int TILE_WATER = 0;
    private const int TILE_BEACH = 1;
    private const int TILE_PLAINS = 2;
    private const int TILE_FOREST = 3;
    private const int TILE_MOUNTAIN = 4;

    public (GameObject tilePrefab, Quaternion rotation) GetBeachEdgeTile(Vector2 tileCoord)
    {
        Dictionary<string, int> tileCheck = createEdgeTiles.GetTileCheck(tileCoord);
        if (tileCheck != null)
        {
            Debug.Log($"tileCheck for {tileCoord}: {string.Join(", ", tileCheck.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
        }
        else
        {
            Debug.LogError($"tileCheck is null for {tileCoord}");
            return (null, Quaternion.identity);
        }

        if (tileCheck != null)
        {
            // Define tiles and their expected types
            var expectedSingleWaterTile = new Dictionary<string, List<int>> // SINGLE WATER TILE
            {
                { "left", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedLeftUTile = new Dictionary<string, List<int>> // LEFT U TILE
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

            var expectedRightUTile = new Dictionary<string, List<int>> // RIGHT U TILE
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

            var expectedUpUTile = new Dictionary<string, List<int>> // UP U TILE
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

            var expectedDownUTile = new Dictionary<string, List<int>> // DOWN U TILE
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

            var expectedDownRightLTile = new Dictionary<string, List<int>> // L TILE, WATER FLOWS DOWN AND RIGHT
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedUpRightLTile = new Dictionary<string, List<int>> // L TILE, WATER FLOWS UP AND RIGHT
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedUpLeftLTile = new Dictionary<string, List<int>> // L TILE, WATER FLOWS UP AND LEFT
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedDownLeftLTile = new Dictionary<string, List<int>> // L TILE, WATER FLOWS DOWN AND LEFT
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedTopRightInner = new Dictionary<string, List<int>> // TOP RIGHT INNER CORNER
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

            var expectedTopLeftInner = new Dictionary<string, List<int>> // TOP LEFT INNER CORNER
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedBottomRightInner = new Dictionary<string, List<int>> // BOTTOM RIGHT INNER CORNER
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

            var expectedBottomLeftInner = new Dictionary<string, List<int>> // BOTTOM LEFT INNER CORNER
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

            var expectedTwoRightCornerOuter = new Dictionary<string, List<int>> // TWO OUTER RIGHT CORNER
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedTwoLeftCornerOuter = new Dictionary<string, List<int>> // TWO OUTER LEFT CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedTwoUpCornerOuter = new Dictionary<string, List<int>> // TWO OUTER UP CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedTwoDownCornerOuter = new Dictionary<string, List<int>> // TWO OUTER DOWN CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedTwoDiagnalCornerOuter = new Dictionary<string, List<int>> // TWO OUTER DIAGNAL CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedTwoDiagnalCornerOuter90 = new Dictionary<string, List<int>> // TWO OUTER DIAGNAL CORNER ROTATION 0,0,90
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedThreeTopRightCornerOuter = new Dictionary<string, List<int>> // THREE TOP RIGHT OUTER CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedThreeTopLeftCornerOuter = new Dictionary<string, List<int>> // THREE TOP LEFT OUTER CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedThreeBottomRightCornerOuter = new Dictionary<string, List<int>> // THREE BOTTOM RIGHT OUTER CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedThreeBottomLeftCornerOuter = new Dictionary<string, List<int>> // THREE BOTTOM LEFT OUTER CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedFourOuterCorner = new Dictionary<string, List<int>> // FOUR OUTER CORNER
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedTopRightOuter = new Dictionary<string, List<int>> // TOP RIGHT OUTER CORNER
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

            var expectedTopLeftOuter = new Dictionary<string, List<int>> // TOP LEFT OUTER CORNER
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

            var expectedBottomRightOuter = new Dictionary<string, List<int>> // BOTTOM RIGHT OUTER CORNER
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

            var expectedBottomLeftOuter = new Dictionary<string, List<int>> // BOTTOM LEFT OUTER CORNER
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomLeft", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedRightBottomLeftOuter = new Dictionary<string, List<int>> // 'expectedRight' combined with 'expectedBottomLeftCornerOuter'
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedUpBottomRightOuter = new Dictionary<string, List<int>> // 'expectedUp' combined with 'expectedBottomRightCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedLeftTopRightOuter = new Dictionary<string, List<int>> // 'expectedLeft' combined with 'expectedTopRightCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedDownTopLeftOuter = new Dictionary<string, List<int>> // 'expectedDown' combined with 'expectedTopLeftCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedRightTopLeftOuter = new Dictionary<string, List<int>> // 'expectedLeft' combined with 'expectedTopLeftCornerOuter'
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedUpBottomLeftOuter = new Dictionary<string, List<int>> // 'expectedUp' combined with 'expectedBottomLeftCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedLeftBottomRightOuter = new Dictionary<string, List<int>> // 'expectedLeft' combined with 'expectedBottomRightCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedDownTopRightOuter = new Dictionary<string, List<int>> // 'expectedDown' combined with 'expectedTopRightCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedRightBottomTopLeftOuter = new Dictionary<string, List<int>> // 'expectedLeft' combined with 'expectedBottomLeftCornerOuter' and 'expectedTopLeftCornerOuter'
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedUpBottomLeftRightOuter = new Dictionary<string, List<int>> // 'expectedUp' combined with 'expectedBottomLeftCornerOuter' and 'expectedBottomRightCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedLeftBottomTopRightOuter = new Dictionary<string, List<int>> // 'expectedLeft' combined with 'expectedBottomRightCornerOuter' and 'expectedTopRightCornerOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } }
            };

            var expectedDownTopLeftRightOuter = new Dictionary<string, List<int>> // 'expectedDown' combined with 'expectedTopLeftOuter' and 'expectedTopRightOuter'
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER } },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedTwoSidedVertical = new Dictionary<string, List<int>> // TWO SIDES VERTICAL
            {
                { "left", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "right", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "up", new List<int> {TILE_WATER } },
                { "down", new List<int> {TILE_WATER } },
                { "bottomLeft", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedTwoSidedHorizontal = new Dictionary<string, List<int>> // TWO SIDES HORIZONTAL
            {
                { "left", new List<int> {TILE_WATER } },
                { "right", new List<int> {TILE_WATER} },
                { "up", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "down", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN } },
                { "bottomLeft", new List<int> { TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "bottomRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topRight", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } },
                { "topLeft", new List<int> {TILE_BEACH, TILE_PLAINS, TILE_FOREST, TILE_MOUNTAIN, TILE_WATER } }
            };

            var expectedLeft = new Dictionary<string, List<int>> // LEFT SIDE
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

            var expectedRight = new Dictionary<string, List<int>> // RIGHT SIDE
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

            var expectedDown = new Dictionary<string, List<int>> // DOWN SIDE
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

            var expectedUp = new Dictionary<string, List<int>> // UP SIDE
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

            // U Tile
            if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedLeftUTile)) // Left U Tile
            {
                Debug.Log($"Returning prefab: {mapGenerator.beachEdgePrefabs.beachToWaterUShape} at tileCoord {tileCoord}");
                return (mapGenerator.beachEdgePrefabs.beachToWaterUShape, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedRightUTile)) // Right U Tile
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterUShape, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUpUTile)) // Up U Tile
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterUShape, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDownUTile)) // Down U Tile
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterUShape, Quaternion.Euler(0f, 0f, -90f));
            }
            // L Tile
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDownRightLTile)) // L TILE, WATER FLOWS DOWN AND RIGHT
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterLShape, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUpRightLTile)) // L TILE, WATER FLOWS UP AND RIGHT
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterLShape, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUpLeftLTile)) // L TILE, WATER FLOWS UP AND LEFT
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterLShape, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDownLeftLTile)) // L TILE, WATER FLOWS DOWN AND LEFT
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterLShape, Quaternion.Euler(0f, 0f, -90f));
            }
            // Straight edges with corners
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedRightBottomTopLeftOuter)) // 'expectedLeft' combined with 'expectedBottomLeftCornerOuter' and 'expectedTopLeftCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideTwoCorners, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUpBottomLeftRightOuter)) // 'expectedUp' combined with 'expectedBottomLeftCornerOuter' and 'expectedBottomRightCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideTwoCorners, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedLeftBottomTopRightOuter)) // 'expectedLeft' combined with 'expectedBottomRightCornerOuter' and 'expectedTopRightCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideTwoCorners, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDownTopLeftRightOuter)) // 'expectedDown' combined with 'expectedTopLeftOuter' and 'expectedTopRightOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideTwoCorners, Quaternion.Euler(0f, 0f, -90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedRightBottomLeftOuter)) // 'expectedRight' combined with 'expectedBottomLeftCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInTopRight, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUpBottomRightOuter)) // 'expectedUp' combined with 'expectedBottomRightCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInTopRight, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedLeftTopRightOuter)) // 'expectedLeft' combined with 'expectedTopRightCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInTopRight, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDownTopLeftOuter)) // 'expectedDown' combined with 'expectedTopLeftCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInTopRight, Quaternion.Euler(0f, 0f, -90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedRightTopLeftOuter)) // 'expectedRight' combined with 'expectedTopLeftCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInBottomRight, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUpBottomLeftOuter)) // 'expectedUp' combined with 'expectedBottomLeftCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInBottomRight, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedLeftBottomRightOuter)) // 'expectedLeft' combined with 'expectedBottomRightCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInBottomRight, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDownTopRightOuter)) // 'expectedDown' combined with 'expectedTopRightCornerOuter'
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterSideCornerInBottomRight, Quaternion.Euler(0f, 0f, -90f));
            }
            // Four Outer Corner
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedFourOuterCorner)) // Four Outer Corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterFourOuter, Quaternion.identity);
            }
            // Three Outer Corner
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedThreeTopRightCornerOuter)) // Three Top Right Outer Corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterThreeOuter, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedThreeTopLeftCornerOuter)) // Three Top Left Outer Corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterThreeOuter, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedThreeBottomRightCornerOuter)) // Three Bottom Right Outer Corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterThreeOuter, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedThreeBottomLeftCornerOuter)) // Three Bottom Left Outer Corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterThreeOuter, Quaternion.Euler(0f, 0f, -90f));
            }
            //Two Outer Diagnal
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoDiagnalCornerOuter)) // TWO OUTER DIAGNAL CORNER
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterTwoDiagnalOuter, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoDiagnalCornerOuter90)) // TWO OUTER DIAGNAL CORNER ROTATION 0,0,90
            {
                return (mapGenerator.beachEdgePrefabs.BeachToWaterTwoDiagnalOuter, Quaternion.Euler(0f, 0f, 90f));
            }
            // Two Outer Corner
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoRightCornerOuter)) // Two Right Outer corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterTwoOuter, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoLeftCornerOuter)) // Two Left Outer corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterTwoOuter, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoUpCornerOuter)) // Two Up Outer corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterTwoOuter, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoDownCornerOuter)) // Two Down Outer corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterTwoOuter, Quaternion.Euler(0f, 0f, -90f));
            }
            // Inward Corners
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTopRightInner)) // Top-right inner corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterInner, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTopLeftInner)) // Top-Left inner  corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterInner, Quaternion.Euler(0f, 0f, -90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedBottomRightInner)) // Bottom-Right inner corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterInner, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedBottomLeftInner)) // Bottom-Left inner corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterInner, Quaternion.identity);
            }
            // Outward corners
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedBottomRightOuter)) // Bottom-Right outward corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterOneOuter, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedBottomLeftOuter)) //Bottom-Left outward corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterOneOuter, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTopRightOuter)) // Top-Right outward corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterOneOuter, Quaternion.Euler(0f, 0f, 180f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTopLeftOuter)) // Top-Left outward corner
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterOneOuter, Quaternion.Euler(0f, 0f, -90f));
            }
            // Straight edges
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedSingleWaterTile))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterSingleWater, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoSidedVertical))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterTwoSide, Quaternion.identity);
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedTwoSidedHorizontal))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterTwoSide, Quaternion.Euler(0f, 0f, 90f));
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedRight))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterSide, Quaternion.identity); // Face Right (default rotation)
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedLeft))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterSide, Quaternion.Euler(0f, 0f, 180f)); // Face Left
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedDown))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterSide, Quaternion.Euler(0f, 0f, -90f)); // Face down
            }
            else if (mapGenerator.CheckAdjacentTiles(tileCheck, expectedUp))
            {
                return (mapGenerator.beachEdgePrefabs.beachToWaterSide, Quaternion.Euler(0f, 0f, 90f)); // Face up 
            }
        }

        Debug.Log("No matching beach edge tile found, returning null.");
        return (null, Quaternion.identity); // Return null if no edge tile is needed
    }
}
