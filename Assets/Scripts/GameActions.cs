using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions : ScriptableObject {

    public enum Actions
    {
        Move, 
        Stay, 
        PickUp, 
        Throw, 
        Enable
    }

    public enum TileTypes
    {
        None,
        Hole,
        EmptyTile, 
        PlayerTile, 
        ItemTile, 
        KeyTile, 
        GoalTile, 
        PawnTile,
    }

    public enum AIActions
    {
        Stay,
        Move,
        Left,
        Right, 
        Forward, 
        Back
    }
}
