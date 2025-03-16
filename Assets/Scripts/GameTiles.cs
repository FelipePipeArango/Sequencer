using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTiles : ScriptableObject
{
    public enum TileTypes
    {
        None,
        EmptyTile,
        PlayerTile,
        ItemTile,
        KeyTile,
        GoalTile,
        PawnTile,
    }
}
