using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions : ScriptableObject {

    public enum Actions
    {
        Move,
        PickUp, 
        Throw, 
        Enable
    }

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
