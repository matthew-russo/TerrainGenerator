using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRock : Block
{
    public BlockRock() : base() { }
    public override Tile TexturePosition(Direction direction)
    {
        Tile tile = new Tile();
        tile.x = 0;
        tile.y = 0;
        return tile;
    }
}
