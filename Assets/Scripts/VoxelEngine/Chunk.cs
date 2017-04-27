using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    private Block[,] blocks;
    public const int chunksize = 16;
    public bool update = true;

    private MeshFilter filter;
    private MeshCollider coll;

    public int localXOrigin;
    public int localZOrigin;

    public void TimeToStart()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();

        blocks = new Block[chunksize,chunksize];

        for (int x = 0; x < TerrainManager.BIOME_SIZE; ++x)
        {
            for (int z = 0; z < TerrainManager.BIOME_SIZE; ++z)
            {
                float yPos = TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z];
                float val = Random.value;

                if (yPos < 50)
                {
                    if (val < .98f)
                    {
                        blocks[x, z] = new BlockGrass();
                    }
                    else blocks[x, z] = new BlockRock();
                }
                else if (yPos < 70)
                {
                    if (val < .7f)
                    {
                        blocks[x,z] = new BlockGrass();
                    }
                    else blocks[x, z] = new BlockRock();
                }
                else if (yPos < 80)
                {
                    if (val < .5f)
                    {
                        blocks[x, z] = new BlockGrass();
                    }
                    else blocks[x, z] = new BlockRock();
                }
                else if (yPos < 90)
                {
                    if (val < .7f)
                    {
                        blocks[x, z] = new BlockRock();
                    }
                    else if (val < .9f)
                    {
                        blocks[x,z] = new BlockSnow();
                    }
                    else blocks[x, z] = new BlockGrass();
                }
                else if (yPos < 105)
                {
                    if (Random.value < .6f)
                    {
                        blocks[x,z] = new BlockRock();
                    }
                    else blocks[x, z] = new BlockSnow();
                }
                else
                {
                    if (Random.value < .9f)
                    {
                        blocks[x, z] = new BlockSnow();
                    }
                    else blocks[x, z] = new BlockRock();
                }
            }
        }

        UpdateChunk();
    }

    public void PositionChunk(int x, int z)
    {
        localXOrigin = x;
        localZOrigin = z;
    }

    public Block GetBlock(int x, int z)
    {
        return blocks[x, z];
    }

    //Updates the chunk based on its contents
    void UpdateChunk()
    {
        MeshData meshData = new MeshData();
        for (int x = 0; x < chunksize; ++x)
        {   
            for (int z = 0; z < chunksize; ++z)
            {
                float yPos = TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z];
                meshData = blocks[x, z].Blockdata(this, localXOrigin + x, yPos, localZOrigin + z, meshData);
            }
        }
        RenderMesh(meshData);
    }

    //Sends the calculated mesh information
    //to the mesh and collision components
    void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();
        filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }

    
}
