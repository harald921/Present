using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk
{
    Tile[,] tiles;

    public Chunk(int inSize, float[,] inNoiseMap)
    {
        tiles = new Tile[inSize, inSize];
    }
}