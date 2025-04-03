using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameTiles;
using static GridManager;

public class UnitController : MonoBehaviour
{
    [SerializeField] public CompleteScreen goalSequence;

    [HideInInspector] public float fallSpeed = 1.0f;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;
    [HideInInspector] public int number = 0;
    [HideInInspector] protected bool isBoardBelow = true;


    protected void MoveTo(Vector2Int direction, TileTypes spawnTileType)
    {
        Vector3 checkPos = new Vector3(
           transform.position.x + direction.x,
           0,
           transform.position.z + direction.y);

        switch (gridManager.CheckWhatNextTileIs(checkPos))
        {

            case TileTypes.GoalTile:
                { 
                    MoveToGoal(direction, spawnTileType);
                }
                break;
            case TileTypes.PlayerTile:
                { 
                    Debug.Log("Player");
                }
                break;
            case TileTypes.PawnTile:
                { 
                    Debug.Log("Companion");
                }
                break;
            case TileTypes.KeyTile:
                {
                    MoveToTile(direction, spawnTileType);

                    KeyItemCheck();
                }
                break;
            case TileTypes.ItemTile:
                {
                    MoveToTile(direction, spawnTileType);

                    NumberItemCheck();
                }
                break;
            case TileTypes.EmptyTile:
                {
                    MoveToTile(direction, spawnTileType);
                }
                break;
            
            case TileTypes.None: break;
        }

    }

   


    protected void IfFall()
    {
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

    private void MoveToTile(Vector2Int direction, TileTypes pawnTileType)
    {
        gridManager.ResetTileType(pawnTileType);

        transform.position += new Vector3(direction.x, 0, direction.y);

        gridManager.UpdateTileType(transform.position, pawnTileType);
    }

    private void MoveToGoal(Vector2Int direction, TileTypes pawnTileType)
    {
        MoveToTile(direction, pawnTileType);
        GoalCheck();
    }

    protected virtual IEnumerator Movement(Vector2Int direction){ return null; }

    protected void GoalCheck()
    {
        if (hasItem)
        {
            StartCoroutine(goalSequence.OpenCompleteScreen());
        }
    }

    protected void KeyItemCheck()
    {
        if (hasItem != true)
        {
            hasItem = true;
            gridManager.keyItem.SetActive(false);
        }
    }

    protected void NumberItemCheck()
    {
        if(gridManager.pickUpNumber != null)
            gridManager.pickUpNumber.SetActive(false);
        gridManager.numberHUD.SetActive(true);
        hasNumber = true;
    }
}

