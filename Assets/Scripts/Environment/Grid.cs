using UnityEngine;

/// <summary>
/// Procedrually generates a mesh using Perlin Noise to simulate a snow-covered ground
/// </summary>

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour {
    public int xSize, ySize;
    public float density;

    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Awake()
    {
        Generate();
        gameObject.AddComponent<MeshCollider>();
    }

    float generateFractalPerlinNoiseHeightMap(float i, float j)
    {
        float randXOffset = Random.value;
        float randYOffset = Random.value;

        float frequency = .1f;

                float xCoord1 = randXOffset + frequency * (i / (float)xSize);
                float yCoord1 = randYOffset + frequency * (j / (float)ySize);
                float elevation = Mathf.PerlinNoise(xCoord1, yCoord1);

                float xCoord2 = randXOffset + 1 * frequency * (i / (float)xSize);
                float yCoord2 = randYOffset + 1 * frequency * (j / (float)ySize);
                float roughness = Mathf.PerlinNoise(xCoord2, yCoord2);

                float xCoord3 = randXOffset + 2 * frequency * (i / (float)xSize);
                float yCoord3 = randYOffset + 2 * frequency * (j / (float)ySize);
                float detail = Mathf.PerlinNoise(xCoord3, yCoord3);

                float output = elevation + 0.25f * roughness + 0.125f * detail;
        return output * 10f;
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Grid";

        _vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[_vertices.Length];
        Vector4[] tangents = new Vector4[_vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                _vertices[i] = new Vector3(x*density, y*density);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x < xSize; x++, i++)
            {
                float perlin = generateFractalPerlinNoiseHeightMap(x+.01f, y+.01f);
                _vertices[i] = new Vector3(_vertices[i].x, _vertices[i].y, perlin *2);
            }
        }

        _mesh.vertices = _vertices;
        _mesh.uv = uv;
        _mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }
}
