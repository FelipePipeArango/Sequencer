using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using static GameTiles;
using static GridManager;

public class UnitController : MonoBehaviour
{
    [HideInInspector] public float fallSpeed = 3f;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;
    [HideInInspector] public int moveNumber = 0;
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
            case TileTypes.KeyTile:
                {
                    MoveToTile(direction, spawnTileType);

                    KeyItemCheck();
                }
                break;
            case TileTypes.ItemTile:
                {
                    MoveToTile(direction, spawnTileType);
                    NumberItemCheck(checkPos);
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
                Addressables.LoadSceneAsync(SceneHolder.sceneHolderInstance.GetCurrentScene());
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
        if (hasItem)
        {
            MoveToTile(direction, pawnTileType);

            PlayerManager.playerManagerInstance.PlayerWon();
        }
        else
            Debug.Log("Need key");
    }

    protected virtual IEnumerator Movement(Vector2Int direction){ return null; }

    protected void KeyItemCheck()
    {
        if (hasItem != true)
        {
            hasItem = true;
            gridManager.keyItem.SetActive(false);
        }
    }

    protected void ThrowKey(TileScript tile)
    {
        if (hasItem != false)
        {
            hasItem = false;
            
            gridManager.keyItem.transform.position =
                tile.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
            gridManager.keyItem.SetActive(true);

            gridManager.UpdateTileType(
                tile.transform.position,
                TileTypes.KeyTile
                );
        }
    }

    protected void NumberItemCheck(Vector3 pos)
    {
        if(gridManager.numberPickUp != null)
        {
            gridManager.DetermineItem(pos).PickedUp();
        }

        hasNumber = true;
    }
}

