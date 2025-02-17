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
        EmptyTile, 
        PlayerTile, 
        ItemTile, 
        KeyTile, 
        GoalTile, 
        PawnTile
    }

    public enum AIActions
    {
        Left,
        Right, 
        Forward, 
        Back
    }

    [SerializeField] List<TileTypes> types = new List<TileTypes>();
    [SerializeField] List<Actions> actions = new List<Actions>();
    [SerializeField] List<AIActions> aiActions = new List<AIActions>();
}
