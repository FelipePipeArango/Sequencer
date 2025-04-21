// LevelData.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using static GameTiles;

[Serializable]
public class LevelData
{
    public Vector2Int gridSize;
    public List<TileEntry> tiles = new();
    public Vector2Int playerPosition;
    public Vector2Int aiPosition;
    public Vector2Int keyItemPosition;
    public Vector2Int goalPosition;
}

[Serializable]
public class TileEntry
{
    public Vector2Int position;
    public TileTypes tileType;

    public TileEntry(Vector2Int pos, TileTypes type)
    {
        position = pos;
        tileType = type;
    }
}
