using System.Collections;
using System.Collections.Generic;
using Patterns;
using UnityEngine;

public class TerrainManager : Singleton<TerrainManager> {
    public float[,] GlobalHeightMap;

    public GameObject chunk;

    public GameObject terrainSegmentPrefab;

    public const int GRID_SIZE = 64;
    public const int BIOME_SIZE = 16;
    public const float SEGMENT_WIDTH = 1f;
    public const float SEGMENT_HEIGHT = 5f;

    public float randXOffset, randYOffset;

    public List<Biome> biomes = new List<Biome>();

    public Color sand;
    public Color grass;
    public Color rock;
    public Color snow;

    protected int localXOrigin;
    protected int localYOrigin;

    protected float frequency;

    void Start()
    {
        GlobalHeightMap = new float[BIOME_SIZE*GRID_SIZE, BIOME_SIZE * GRID_SIZE];

        randXOffset = Random.value * 10f;
        randYOffset = Random.value * 10f;

        StartCoroutine(Initialize());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.OpenOrCloseWindow();
        }
    }

    public IEnumerator Initialize()
    {
        generateFractalPerlinNoiseHeightMap();

        for (int i = 0; i < GRID_SIZE; ++i)
        {
            for (int j = 0; j < GRID_SIZE; ++j)
            {
                LoadIndividualBiomes(i * BIOME_SIZE, j * BIOME_SIZE);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void LoadIndividualBiomes(int x, int z)
    {
        // 1. Randomly pick biome
        //Biome newBiome = new Biome(x,y,randXOffset,randYOffset);
        //newBiome.generateFractalPerlinNoiseHeightMap();

        GameObject newChunk = GameObject.Instantiate(chunk);
        Chunk newChunkScript = newChunk.GetComponent<Chunk>();

        newChunkScript.localXOrigin = x;
        newChunkScript.localZOrigin = z;
        newChunkScript.TimeToStart();
        newChunkScript.transform.parent = transform;
    }

    public void generateFractalPerlinNoiseHeightMap()
    {
        frequency = 8f;

        for (int i = 0; i < GRID_SIZE * BIOME_SIZE; ++i)
        {
            for (int j = 0; j < GRID_SIZE * BIOME_SIZE; ++j)
            {
                // distanceFromCenter appraches 0 as it nears center
                // ie furthest point == 16, center == 0
                // if we subract 16 from distance from center and then grab that absolute value we will have a ratio to use
                //float horizontalDistanceFromCenter = Mathf.Abs(i - (localXOrigin + TerrainManager.BIOME_SIZE / 2f));
                //horizontalDistanceFromCenter = Mathf.Abs(horizontalDistanceFromCenter - TerrainManager.BIOME_SIZE / 2f);

                //float verticalDistanceFromCenter = Mathf.Abs(j - (localYOrigin + TerrainManager.BIOME_SIZE / 2f));
                //verticalDistanceFromCenter = Mathf.Abs(verticalDistanceFromCenter - TerrainManager.BIOME_SIZE / 2f);

                //float distFromCenter = horizontalDistanceFromCenter * verticalDistanceFromCenter;

                float xCoord1 = randXOffset + Mathf.Pow(frequency, .3f) * (i / (float)(BIOME_SIZE * GRID_SIZE));
                float yCoord1 = randYOffset + Mathf.Pow(frequency, .3f) * (j / (float)(BIOME_SIZE * GRID_SIZE));
                float elevation = Mathf.PerlinNoise(xCoord1, yCoord1);
                //Debug.Log("Elevation : " + elevation);

                float xCoord2 = randXOffset + Mathf.Pow(frequency, 1.1f) * (i / (float)(BIOME_SIZE * GRID_SIZE));
                float yCoord2 = randYOffset + Mathf.Pow(frequency, 1.1f) * (j / (float)(BIOME_SIZE * GRID_SIZE));
                float roughness = Mathf.PerlinNoise(xCoord2, yCoord2);
                //Debug.Log("Rougness : " + roughness);

                float xCoord3 = randXOffset + Mathf.Pow(frequency, 1.7f) * (i / (float)(BIOME_SIZE * GRID_SIZE));
                float yCoord3 = randYOffset + Mathf.Pow(frequency, 1.7f) * (j / (float)(BIOME_SIZE * GRID_SIZE));
                float detail = Mathf.PerlinNoise(xCoord3, yCoord3);
                //Debug.Log("Detail : " + detail);

                GlobalHeightMap[i, j] = (elevation + (0.5f * roughness * Mathf.Pow(elevation, 2)) + (0.1f * detail * Mathf.Pow(elevation, 3) * roughness)) * 128f;
                //TerrainManager.Instance.GlobalHeightMap[i, j] = (elevation + (roughness * detail)) * 3.5f;
            }
        }
    }
























    void SpawnTerrainFromBiomes()
    {
        for (int i = 0; i < GRID_SIZE * BIOME_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE * BIOME_SIZE; j++)
            {
                GameObject segmentObj = Instantiate(terrainSegmentPrefab);
                segmentObj.transform.parent = transform;
                float segmentX = i * SEGMENT_WIDTH + SEGMENT_WIDTH / 2f;
                float segmentZ = j * SEGMENT_WIDTH + SEGMENT_WIDTH / 2f;
                float segmentY = (SEGMENT_HEIGHT) * GlobalHeightMap[i, j];
                if ((SEGMENT_HEIGHT) * GlobalHeightMap[i, j] < 16)
                {
                    segmentObj.GetComponent<Renderer>().material.color = sand;
                    float difference = 29f - segmentY;
                    segmentY = 29f - (difference * .35f);
                }
                else if ((SEGMENT_HEIGHT) * GlobalHeightMap[i, j] < 29)
                {
                    segmentObj.GetComponent<Renderer>().material.color = grass;
                    float difference = 29f - segmentY;
                    segmentY = 29f - difference * .35f;
                }
                else if ((SEGMENT_HEIGHT) * GlobalHeightMap[i, j] < 50)
                {
                    segmentObj.GetComponent<Renderer>().material.color = rock;
                }
                else
                {
                    segmentObj.GetComponent<Renderer>().material.color = snow;
                }
                segmentObj.transform.localPosition = new Vector3(segmentX, segmentY, segmentZ);
            }
        }
    }

    void NormalizeHeightMap()
    {
        // First, find the largest and the smallest values in the height map
        float minHeight = 0;
        float maxHeight = 0;
        for (int i = 0; i < GRID_SIZE * BIOME_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE * BIOME_SIZE; j++)
            {
                float currentHeight = GlobalHeightMap[i, j];
                if ((i == 0 && j == 0) || currentHeight < minHeight)
                {
                    minHeight = currentHeight;
                }
                if ((i == 0 && j == 0) || currentHeight > maxHeight)
                {
                    maxHeight = currentHeight;
                }
            }
        }
        // Now that we have a min and max height, normalize it so maxHeight -> 1
        // and minHeight -> 0
        for (int i = 0; i < GRID_SIZE * BIOME_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE * BIOME_SIZE; j++)
            {
                if (maxHeight == minHeight)
                {
                    GlobalHeightMap[i, j] = 0f;
                    continue;
                }
                float normalizedHeight = (GlobalHeightMap[i, j] - minHeight) / (maxHeight - minHeight);
                GlobalHeightMap[i, j] = normalizedHeight * 6;
            }
        }
    }
}