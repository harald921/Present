using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile
{
    public struct Parameters
    {
        public BaseType baseType;
        public Floor floor;
    }

    public class BaseType
    {
        int moveSpeed; // How many turns it takes a player to walk onto it
    }
    BaseType _base = null;

    public class Floor
    {
        int moveSpeed; // How many turns it takes a player to walk onto it
    }
    Floor _floor = null;


    public Tile(Parameters inParameters)
    {
        _base = inParameters.baseType;
        _floor = inParameters.floor;
    }

    // Base tile type
    // Floor, if any
    // Furniture, if any
    // Character, if any
    // Collection of items that lie upon this tile
}

