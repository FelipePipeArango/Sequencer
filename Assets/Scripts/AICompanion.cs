using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameActions;
using static GridManager;

public class AICompanion : MonoBehaviour
{
    [SerializeField] public float fallSpeed = 1.0f;

    public GameActions.AIActions action;
    public bool canMove = false;
    public bool isMoving = false;
    private bool isBoardBelow = true;


    private void Start()
    {
        gridManager.UpdateTileType(transform.position, TileTypes.PawnTile);
    }

    void Update()
    {
        if (canMove && action != AIActions.Stay)
        {
            if (action == AIActions.Forward) StartCoroutine(Movement(Vector2Int.up));

            if (action == AIActions.Back) StartCoroutine(Movement(Vector2Int.down));

            if (action == AIActions.Right) StartCoroutine(Movement(Vector2Int.right));

            if (action == AIActions.Left) StartCoroutine(Movement(Vector2Int.left));
        }
        if (!isBoardBelow)
        {
            Vector3 targetPos = new Vector3(transform.position.x, -1f, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);

            if (transform.position.y <= -0.99f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
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
                isMoving = true;
                if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.GoalTile)
                {
                    if (gridManager.playerActions.hasItem)
                    {
                        gridManager.ResetTileType(TileTypes.PawnTile);
                        transform.position += new Vector3(direction.x, 0, direction.y);


                        gridManager.GoalCheck();
                    }

                    else Debug.Log("Need key");
                }
                else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.PlayerTile)
                {
                    Debug.Log("Player");
                }
                else
                {
                    gridManager.ResetTileType(TileTypes.PawnTile);


                    transform.position += new Vector3(direction.x, 0, direction.y);
                    switch (gridManager.CheckWhatNextTileIs(checkPos))
                    {
                        case TileTypes.KeyTile:

                            gridManager.KeyItemCheck();

                            break;
                        case TileTypes.ItemTile:

                            gridManager.NumberItemCheck();

                            break;
                        case TileTypes.EmptyTile:

                            Debug.Log("Empty");

                            break;
                    }

                    gridManager.UpdateTileType(transform.position,
                        TileTypes.PawnTile);
                }

                yield return new WaitForSeconds(0.5f);
                checkPos += new Vector3(direction.x, 0, direction.y);
            }

        }

        isMoving = false;

    }
}
