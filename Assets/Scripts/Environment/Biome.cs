using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    protected int localXOrigin;
    protected int localYOrigin;
    protected float randXOffset;
    protected float randYOffset;

    protected float frequency;

    public Biome(int x, int y, float rx, float ry)
    {
        localXOrigin = x;
        localYOrigin = y;
        randXOffset = rx;
        randYOffset = ry;
    }

    public void generateFractalPerlinNoiseHeightMap()
    {
        frequency = 8f;

        for (int i = localXOrigin; i < localXOrigin + TerrainManager.BIOME_SIZE; ++i)
        {
            for (int j = localYOrigin; j < localYOrigin + TerrainManager.BIOME_SIZE; ++j)
            {
                // distanceFromCenter appraches 0 as it nears center
                // ie furthest point == 16, center == 0
                // if we subract 16 from distance from center and then grab that absolute value we will have a ratio to use
                //float horizontalDistanceFromCenter = Mathf.Abs(i - (localXOrigin + TerrainManager.BIOME_SIZE / 2f));
                //horizontalDistanceFromCenter = Mathf.Abs(horizontalDistanceFromCenter - TerrainManager.BIOME_SIZE / 2f);

                //float verticalDistanceFromCenter = Mathf.Abs(j - (localYOrigin + TerrainManager.BIOME_SIZE / 2f));
                //verticalDistanceFromCenter = Mathf.Abs(verticalDistanceFromCenter - TerrainManager.BIOME_SIZE / 2f);

                //float distFromCenter = horizontalDistanceFromCenter * verticalDistanceFromCenter;

                float xCoord1 = randXOffset + Mathf.Pow(frequency, .3f) * (i / (float)(TerrainManager.BIOME_SIZE * TerrainManager.GRID_SIZE));
                float yCoord1 = randYOffset + Mathf.Pow(frequency, .3f) * (j / (float)(TerrainManager.BIOME_SIZE * TerrainManager.GRID_SIZE));
                float elevation = Mathf.PerlinNoise(xCoord1, yCoord1);
                //Debug.Log("Elevation : " + elevation);

                float xCoord2 = randXOffset + Mathf.Pow(frequency, 1.1f) * (i / (float)(TerrainManager.BIOME_SIZE * TerrainManager.GRID_SIZE));
                float yCoord2 = randYOffset + Mathf.Pow(frequency, 1.1f) * (j / (float)(TerrainManager.BIOME_SIZE * TerrainManager.GRID_SIZE));
                float roughness = Mathf.PerlinNoise(xCoord2, yCoord2);
                //Debug.Log("Rougness : " + roughness);

                float xCoord3 = randXOffset + Mathf.Pow(frequency, 1.7f) * (i / (float)(TerrainManager.BIOME_SIZE * TerrainManager.GRID_SIZE));
                float yCoord3 = randYOffset + Mathf.Pow(frequency, 1.7f) * (j / (float)(TerrainManager.BIOME_SIZE * TerrainManager.GRID_SIZE));
                float detail = Mathf.PerlinNoise(xCoord3, yCoord3);
                //Debug.Log("Detail : " + detail);

                TerrainManager.Instance.GlobalHeightMap[i, j] = (elevation + (0.5f * roughness * Mathf.Pow(elevation,2)) + (0.1f* detail * Mathf.Pow(elevation,3) * roughness)) * 12f;
                //TerrainManager.Instance.GlobalHeightMap[i, j] = (elevation + (roughness * detail)) * 3.5f;
            }
        }
    }
}
