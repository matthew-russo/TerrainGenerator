using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

	public GameObject terrainSegmentPrefab;

	public int terrainWidth = 1000;
	public int terrainHeight = 1000;

	public const float SEGMENT_WIDTH = 1f;
	public const float SEGMENT_HEIGHT = 5f;

	protected float[,] _heightMap;

	void Start() {
		generateTerrain();
	}

	void generateTerrain() {
		foreach (Transform child in transform) {
			if (child != transform) {
				Destroy(child.gameObject);
			}
		}


		_heightMap = new float[terrainWidth, terrainHeight];

		// Do some generation
		//generatePureRandomHeightMap();
		//generateInterpolatedHeightMap();
		//generatePerlinNoiseHeightMap();
		generateFractalPerlinNoiseHeightMap();

		// Normalize the height map and spawn our objects
		normalizeHeightMap();
		spawnSegmentsFromHeightMap();

	}

	void normalizeHeightMap() {
		// First, find the largest and the smallest values in the height map
		float minHeight = 0;
		float maxHeight = 0;
		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				float currentHeight = _heightMap[i, j];
				if ((i == 0 && j == 0) || currentHeight < minHeight) {
					minHeight = currentHeight;
				}
				if ((i == 0 && j == 0) || currentHeight > maxHeight) {
					maxHeight = currentHeight;
				}
			}
		}
		// Now that we have a min and max height, normalize it so maxHeight -> 1
		// and minHeight -> 0
		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				if (maxHeight == minHeight) {
					_heightMap[i, j] = 0f;
					continue;
				}
				float normalizedHeight = (_heightMap[i, j]-minHeight) * 8 / (maxHeight-minHeight);
				_heightMap[i, j] = normalizedHeight;
			}
		}
	}

	void spawnSegmentsFromHeightMap() {
		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				GameObject segmentObj = Instantiate(terrainSegmentPrefab);
				segmentObj.transform.parent = transform;
				float segmentX = i*SEGMENT_WIDTH + SEGMENT_WIDTH/2f;
				float segmentZ = j*SEGMENT_WIDTH + SEGMENT_WIDTH/2f;
				float segmentY = (SEGMENT_HEIGHT)*_heightMap[i, j];
				segmentObj.transform.localPosition = new Vector3(segmentX, segmentY, segmentZ);
			}
		}
	}

	void generatePureRandomHeightMap() {
		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				_heightMap[i, j] = Random.Range(0f, 1f);
			}
		}
	}

	void generateInterpolatedHeightMap() {

		int latticeSize = 10;
		for (int i = 0; i < terrainWidth; i+=latticeSize) {
			for (int j = 0; j < terrainHeight; j+=latticeSize) {
				_heightMap[i, j] = Random.Range(0f, 1f);
			}
		}
		// Now we've generated lattice heights, we need to interpolate between them.
		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				if ((i % latticeSize) == 0 && (j % latticeSize) == 0) {
					continue;
				}
				// Find the different lattice points related to us
				int leftLattice = i / latticeSize;
				int rightLattice = leftLattice+1;
				int downLattice = j / latticeSize;
				int upLattice = downLattice+1;

				float upLeftLattice = 0f;
				if (leftLattice*latticeSize < terrainWidth && upLattice*latticeSize < terrainHeight) {
					upLeftLattice = _heightMap[leftLattice*latticeSize, upLattice*latticeSize];
				}
				float upRightLattice = 0f;
				if (rightLattice*latticeSize < terrainWidth && upLattice*latticeSize < terrainHeight) {
					upRightLattice = _heightMap[rightLattice*latticeSize, upLattice*latticeSize];
				}
				float downRightLattice = 0;
				if (rightLattice*latticeSize < terrainWidth && downLattice*latticeSize < terrainHeight) {
					downRightLattice = _heightMap[rightLattice*latticeSize, downLattice*latticeSize];
				}
				float downLeftLattice = 0;
				if (leftLattice*latticeSize < terrainWidth && downLattice*latticeSize < terrainHeight) {
					downLeftLattice = _heightMap[leftLattice*latticeSize, downLattice*latticeSize];
				}

				float distanceToRight = (rightLattice*latticeSize-i) / (float)(latticeSize);
				float distanceToLeft = 1f - distanceToRight;

				float upXInterp = upRightLattice*distanceToLeft+upLeftLattice*distanceToRight;
				float downXInterp = downRightLattice*distanceToLeft+downLeftLattice*distanceToRight;

				float distanceToTop = (upLattice*latticeSize-j) / (float)(latticeSize);
				float distanceToBottom = 1f - distanceToTop;

				float yInterp = upXInterp*distanceToBottom + downXInterp*distanceToTop;
				_heightMap[i, j] = yInterp;

			}
		}
	}

	void generatePerlinNoiseHeightMap() {
		float randXOffset = Random.value;
		float randYOffset = Random.value;

		float frequency = 32f;

		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				float xCoord = randXOffset + frequency*(i / (float)terrainWidth);
				float yCoord = randYOffset + frequency*(j / (float)terrainHeight);
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				_heightMap[i, j] = sample;
			}
		}
	}

	void generateFractalPerlinNoiseHeightMap() {
		float randXOffset = Random.value;
		float randYOffset = Random.value;

		float frequency = 1.5f;

		for (int i = 0; i < terrainWidth; i++) {
			for (int j = 0; j < terrainHeight; j++) {
				float xCoord1 = randXOffset + frequency*(i / (float)terrainWidth);
				float yCoord1 = randYOffset + frequency*(j / (float)terrainHeight);
				float elevation = Mathf.PerlinNoise(xCoord1, yCoord1);

                float xCoord2 = randXOffset + 2*frequency*(i / (float)terrainWidth);
				float yCoord2 = randYOffset + 2*frequency*(j / (float)terrainHeight);
				float roughness = Mathf.PerlinNoise(xCoord2, yCoord2);

                float xCoord3 = randXOffset + 3*frequency*(i / (float)terrainWidth);
				float yCoord3 = randYOffset + 3*frequency*(j / (float)terrainHeight);
				float detail = Mathf.PerlinNoise(xCoord3, yCoord3);

				//_heightMap[i, j] = elevation + 0.20f* roughness + 0.10f* detail;
                _heightMap[i, j] = (elevation + (roughness * detail))*64 + 64;

			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R)) {
			generateTerrain();
		}
	}
}
