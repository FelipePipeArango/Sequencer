using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameActions;
using static GameTiles;
using static GameDirections;
using static GridManager;

public class AICompanion : UnitController
{
    [HideInInspector] public Directions direction;
    [HideInInspector] public AIActions action = AIActions.Stay;
    [HideInInspector] public bool isBefore = false;
    [HideInInspector] public bool canMove = false;
    
    
    private void Start()
    {
        gridManager.UpdateTileType(transform.position, TileTypes.PawnTile);
    }

    void Update()
    {
        if (canMove == true && action != AIActions.Stay)
        {
            if (direction == Directions.Forward) StartCoroutine(Movement(Vector2Int.up));

            if (direction == Directions.Back) StartCoroutine(Movement(Vector2Int.down));

            if (direction == Directions.Right) StartCoroutine(Movement(Vector2Int.right));

            if (direction == Directions.Left) StartCoroutine(Movement(Vector2Int.left));
        }
        IfFall();
    }

    //Changed the previous movement implementation to make it more easy to calculate
    public IEnumerator Movement(Vector2Int direction)
    {
        action = AIActions.Stay;

        Vector3 checkPos = new Vector3(
            transform.position.x + direction.x, 
            0, 
            transform.position.z + direction.y);

        if (canMove)
        {
            while (gridManager.CheckWhatNextTileIs(checkPos) != TileTypes.None)
            {
                MoveTo(direction, TileTypes.PawnTile);

                yield return new WaitForSeconds(0.5f);
                checkPos += new Vector3(direction.x, 0, direction.y);
            }

        }
        if(isBefore == true)
            Sequencer.sequencer.PlayerAfterAction();

        canMove = false;
        
    }

    public void PushCompanion(Vector2Int direction)
    {
        MoveTo(direction, TileTypes.PawnTile);
    } 
}
