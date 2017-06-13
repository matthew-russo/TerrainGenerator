using System.Collections;
using System.Collections.Generic;
using Patterns;
using UnityEngine;

/// <summary>
/// In charge of starting generation and creating the height map
/// </summary>

public class TerrainManager : Singleton<TerrainManager> {
    public const int GRID_SIZE = 58; // How many chunks there will be in each direction
    public const int CHUNK_SIZE = 16; // How many blocks each chunk has

    public float[,] GlobalHeightMap;

    public GameObject chunkPrefab;
    public GameObject terrainSegmentPrefab;

    public float randXOffset, randYOffset;

    protected int localXOrigin;
    protected int localYOrigin;

    protected float frequency;

    public bool timeToBreak;
    public bool showLoading;
    public GameObject loading;


    // Initializes the height map to the total number of individual blocks
    // Picks random seeds
    // Starts Generation
    //
    void Start()
    {
        GlobalHeightMap = new float[CHUNK_SIZE*GRID_SIZE, CHUNK_SIZE * GRID_SIZE];

        randXOffset = Random.value * 10f;
        randYOffset = Random.value * 10f;

        StartCoroutine(Initialize());
    }

    // Opens or closes menu on escape key press
    //
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.OpenOrCloseWindow();
        }
        if (showLoading)
        {
            loading.SetActive(true);
        }
    }

    // Coroutine that generates terrain -- uses coroutine cause it takes a long time so players can at least move around while its generating instead of just waiting
    // In each of the loops we check if its time to break (when a user loads a new terrain) and if so, breaks all loops and the coroutine.
    //
    public IEnumerator Initialize()
    {
        showLoading = true;

        generateFractalPerlinNoiseHeightMap();

        for (int i = 0; i < GRID_SIZE; ++i)
        {
            for (int j = 0; j < GRID_SIZE; ++j)
            {
                LoadIndividualBiomes(i * CHUNK_SIZE, j * CHUNK_SIZE);
                if (timeToBreak)
                {
                    yield break;
                }
            }
            if (timeToBreak)
            {
                yield break;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
        if (timeToBreak)
        {
            yield break;
        }

        showLoading = false;
        loading.SetActive(false);
    }

    // Creates a new chunk at the given x and z coordinates, and sets its parent to the this (generator)
    //
    public void LoadIndividualBiomes(int x, int z)
    {
        GameObject newChunk = GameObject.Instantiate(chunkPrefab);
        Chunk newChunkScript = newChunk.GetComponent<Chunk>();

        newChunkScript.localXOrigin = x;
        newChunkScript.localZOrigin = z;
        newChunkScript.TimeToStart();
        newChunkScript.transform.parent = transform;
    }

    // Uses three layers of perlin noise to create the heightmap.
    // frequency modifies how close together peaks would be.
    // elevation is the "largest map" high amplitude, low frequency
    // roughness is the "medium map" medium amplitude, medium frequency
    // detail is the "small map" small amplitude, high frequency
    // I multiply them together at varying exponential rates
    // I'm pretty bad at explaining this, if anything is confusing let me know and I'll do my best to explain how I got to a certain formula or something
    //
    public void generateFractalPerlinNoiseHeightMap()
    {
        frequency = 8f;

        for (int i = 0; i < GRID_SIZE * CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < GRID_SIZE * CHUNK_SIZE; ++j)
            {
                float xCoord1 = randXOffset + Mathf.Pow(frequency, .3f) * (i / (float)(CHUNK_SIZE * GRID_SIZE));
                float yCoord1 = randYOffset + Mathf.Pow(frequency, .3f) * (j / (float)(CHUNK_SIZE * GRID_SIZE));
                float elevation = Mathf.PerlinNoise(xCoord1, yCoord1);
                //Debug.Log("Elevation : " + elevation);

                float xCoord2 = randXOffset + Mathf.Pow(frequency, 1.1f) * (i / (float)(CHUNK_SIZE * GRID_SIZE));
                float yCoord2 = randYOffset + Mathf.Pow(frequency, 1.1f) * (j / (float)(CHUNK_SIZE * GRID_SIZE));
                float roughness = Mathf.PerlinNoise(xCoord2, yCoord2);
                //Debug.Log("Rougness : " + roughness);

                float xCoord3 = randXOffset + Mathf.Pow(frequency, 1.7f) * (i / (float)(CHUNK_SIZE * GRID_SIZE));
                float yCoord3 = randYOffset + Mathf.Pow(frequency, 1.7f) * (j / (float)(CHUNK_SIZE * GRID_SIZE));
                float detail = Mathf.PerlinNoise(xCoord3, yCoord3);
                //Debug.Log("Detail : " + detail);

                GlobalHeightMap[i, j] = (elevation + (0.5f * roughness * Mathf.Pow(elevation, 2)) + (0.1f * detail * Mathf.Pow(elevation, 3) * roughness)) * 128f;
            }
        }
    }























    // UNUSED / OLD BUT I WANT TO KEEP FOR REFERENCE
    //
    //void SpawnTerrainFromBiomes()
    //{
    //    for (int i = 0; i < GRID_SIZE * CHUNK_SIZE; i++)
    //    {
    //        for (int j = 0; j < GRID_SIZE * CHUNK_SIZE; j++)
    //        {
    //            GameObject segmentObj = Instantiate(terrainSegmentPrefab);
    //            segmentObj.transform.parent = transform;
    //            float segmentX = i * SEGMENT_WIDTH + SEGMENT_WIDTH / 2f;
    //            float segmentZ = j * SEGMENT_WIDTH + SEGMENT_WIDTH / 2f;
    //            float segmentY = (SEGMENT_HEIGHT) * GlobalHeightMap[i, j];
    //            if ((SEGMENT_HEIGHT) * GlobalHeightMap[i, j] < 16)
    //            {
    //                segmentObj.GetComponent<Renderer>().material.color = sand;
    //                float difference = 29f - segmentY;
    //                segmentY = 29f - (difference * .35f);
    //            }
    //            else if ((SEGMENT_HEIGHT) * GlobalHeightMap[i, j] < 29)
    //            {
    //                segmentObj.GetComponent<Renderer>().material.color = grass;
    //                float difference = 29f - segmentY;
    //                segmentY = 29f - difference * .35f;
    //            }
    //            else if ((SEGMENT_HEIGHT) * GlobalHeightMap[i, j] < 50)
    //            {
    //                segmentObj.GetComponent<Renderer>().material.color = rock;
    //            }
    //            else
    //            {
    //                segmentObj.GetComponent<Renderer>().material.color = snow;
    //            }
    //            segmentObj.transform.localPosition = new Vector3(segmentX, segmentY, segmentZ);
    //        }
    //    }
    //}
}