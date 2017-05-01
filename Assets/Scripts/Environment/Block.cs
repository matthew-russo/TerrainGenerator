using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {
    //Base block constructor
    public Block() { }

    // TODO: Fix weird bug where the 16th row and column don't properly show south and east face
    //
    // This is where it determines what faces to show. We only show the ones that the player could potentionally see
    // Always show the top face, never show the bottom face
    // The sides that are shown are determined by whether the neighboring block is higher or not.
    //
    // NEEDS TO BE REFACTORED. JUST CHANGED THIS TODAY
    //
    public virtual MeshData Blockdata (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.useRenderDataForCol = true;

        meshData = FaceDataUp(chunk, x, y, z, meshData);

        //Debug.Log("X: " + x + ", Z: " + z);

        if (z < TerrainManager.CHUNK_SIZE * TerrainManager.GRID_SIZE - 1 && TerrainManager.Instance.GlobalHeightMap[x, z + 1] > y) {
            meshData = FaceDataSouth(chunk, x, y, z, meshData);
        }
        else if (z >= TerrainManager.CHUNK_SIZE * TerrainManager.GRID_SIZE - 1)
        {
            meshData = FaceDataSouth(chunk, x, y, z, meshData);
        }

        if (z > 0 && TerrainManager.Instance.GlobalHeightMap[x, z - 1] > y)
        {
            meshData = FaceDataNorth(chunk, x, y, z, meshData);
        }
        else if (z <= 0)
        {
            meshData = FaceDataNorth(chunk, x, y, z, meshData);
        }

        if (x < TerrainManager.CHUNK_SIZE * TerrainManager.GRID_SIZE - 1 && TerrainManager.Instance.GlobalHeightMap[x + 1, z] > y)
        {
            meshData = FaceDataWest(chunk, x, y, z, meshData);
        }
        else if (x >= TerrainManager.CHUNK_SIZE * TerrainManager.GRID_SIZE - 1)
        {
            meshData = FaceDataWest(chunk, x, y, z, meshData);
        }

        if (x > 0 && TerrainManager.Instance.GlobalHeightMap[x - 1, z] > y)
        {
            meshData = FaceDataEast(chunk, x, y, z, meshData);
        }
        else if (x <= 0)
        {
            meshData = FaceDataEast(chunk, x, y, z, meshData);
        }

        return meshData;
    }

    protected virtual MeshData FaceDataUp (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.north));
        return meshData;
    }

    protected virtual MeshData FaceDataEast (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.east));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.south));
        return meshData;
    }

    protected virtual MeshData FaceDataWest (Chunk chunk, int x, float y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.west));
        return meshData;
    }

    public virtual bool IsSolid(Direction direction)
    {
        switch (direction)
        {
            case Direction.north:
                return true;
            case Direction.east:
                return true;
            case Direction.south:
                return true;
            case Direction.west:
                return true;
            case Direction.up:
                return true;
            case Direction.down:
                return true;
        }
        return false;
    }

    public enum Direction { north, east, south, west, up, down };

    public struct Tile
    {
        public int x;
        public int y;
    }

    private const float tileSize = .25f;

    public virtual Tile TexturePosition(Direction dir)
    {
        Tile tile = new Tile();
        tile.x = 0;
        tile.y = 0;
        return tile;
    }
    public virtual Vector2[] FaceUVs(Direction direction)
    {
        Vector2[] UVs = new Vector2[4];
        Tile tilePos = TexturePosition(direction);
        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y);
        return UVs;
    }
}
