using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Holds the logic for building each chunk's mesh and collider
/// Decides which texture to use for each block
/// 
/// THIS USES A MODIFIED VERSION OF SCRIPTS PRESENTED HERE: http://alexstv.com/index.php/posts/unity-voxel-block-tutorial
/// Previously I was instantiating cubes which has horrific performance and this seemed to be a good solution that fit my needs
/// without being total overkill. My first notion was to write a compute/geometry shader and push all the computation to the GPU and would like to eventually get to that
/// but wasn't in the scope of this assignment. 
/// 
/// Essentially this creates a set of Blocks that send vertex data to a MeshData class which compiles all the blocks 
/// in the chunk into one large mesh. This greatly reduces the number of individual things the GPU has to render.
/// </summary>

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

        for (int x = 0; x < TerrainManager.CHUNK_SIZE; ++x)
        {
            for (int z = 0; z < TerrainManager.CHUNK_SIZE; ++z)
            {
                float yPos = TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z];
                float val = Random.value;

                // The follow mess of if/else blocks determine which block to use -- grass, rock, or snow, and also accentuate the mountain peaks and flatten out the valleys.
                //
                if (yPos < 50)
                {
                    // Flatten valley by making the height 50% of the difference from the height and 50fs
                    TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z] = 50f - (50f - yPos) * .5f;

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

                    TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z] = yPos+((yPos - 80f) * 1.6f);
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
                    // Accentuate mountains
                    TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z] = yPos + ((yPos - 80f) * 1.6f);

                    if (Random.value < .6f)
                    {
                        blocks[x,z] = new BlockRock();
                    }
                    else blocks[x, z] = new BlockSnow();
                }
                else
                {
                    // Accentuate mountains
                    TerrainManager.Instance.GlobalHeightMap[localXOrigin + x, localZOrigin + z] = yPos + ((yPos - 80f) * 1.6f);

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

    // Sets the chunks position in global coordinates
    //
    public void PositionChunk(int x, int z)
    {
        localXOrigin = x;
        localZOrigin = z;
    }

    // Returns a block at the given indices WITHIN this chunk only
    //
    public Block GetBlock(int x, int z)
    {
        return blocks[x, z];
    }

    // Creates a new mesh from the blocks in this chunk and renders it
    //
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

    // Sends the calculated mesh information to the mesh and collision components
    //
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
