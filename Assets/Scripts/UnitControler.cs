using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameActions;
using static GridManager;

public class UnitControler : MonoBehaviour
{
    private Vector2Int playerPreviousPosition;

    public int moveAmount = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;


    [SerializeField] public float fallSpeed = 1.0f;
    public GameObject aiCompanion;
    public GameObject arrow;
    private bool isBoardBelow = true;

    private void Start()
    {
        gridManager.UpdateTileType(transform.position,
            TileTypes.PlayerTile);
        
    }

    public void MovementReceiver(int recievedNumber)
    {
        moveAmount = recievedNumber;
    }


    public void ThrowReceiver(int recievedNumber)
    {
        if (hasItem)
        {
            hasItem = false;
            if (recievedNumber >= gridManager.CalculateDistance(
                    gridManager.goal.transform.position, transform.position))
            {
                gridManager.GoalCheck();
            }
        }
    }

    public void PickUpReceiver (int recievedNumber)
    {
        if (recievedNumber == gridManager.CalculateDistance(
                gridManager.keyItem.transform.position, transform.position))
        {
            gridManager.KeyItemCheck();
            gridManager.ResetTileType(TileTypes.KeyTile);
        }

        if (recievedNumber == gridManager.CalculateDistance(
                gridManager.pickUpNumber.transform.position, transform.position))
        {
            if (gridManager.pickUpNumber != null ||
                gridManager.numberHUD != null)
            {
                gridManager.NumberItemCheck();
                gridManager.ResetTileType(TileTypes.KeyTile);
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name; 
            SceneManager.LoadScene(currentScene);
        }
        if (moveAmount > 0 && gridManager.isCompanionMoving() == false) 
        {
            if (Input.GetKeyDown(KeyCode.W)) Movement(Vector2Int.up);

            if (Input.GetKeyDown(KeyCode.S)) Movement(Vector2Int.down);

            if (Input.GetKeyDown(KeyCode.D)) Movement(Vector2Int.right);

            if (Input.GetKeyDown(KeyCode.A)) Movement(Vector2Int.left);
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

    
    void Movement(Vector2Int direction)
    {
        Vector3 checkPos = new Vector3(
            transform.position.x + direction.x,
            0, 
            transform.position.z + direction.y);

        if (gridManager.CheckWhatNextTileIs(checkPos)
            == TileTypes.None) 
        {
            transform.position += new Vector3(direction.x, 0, direction.y);
            moveAmount = 0;
            isBoardBelow = false;
        }
        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.GoalTile)
        {
            if (hasItem)
            {
                gridManager.ResetTileType(TileTypes.PlayerTile);

                transform.position += new Vector3(direction.x, 0, direction.y);

                moveAmount--;

                gridManager.GoalCheck();
            }

            else Debug.Log("Need key");
        }
        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.PawnTile)
        {
            //TODO Move the Companion
            Debug.Log("Move the Companion");
        }
        else
        {
            gridManager.ResetTileType(TileTypes.PlayerTile);

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

            gridManager.UpdateTileType(transform.position, TileTypes.PlayerTile);

            if(gridManager.companion != null) gridManager.companion.canMove = true;
            
            moveAmount--;
        }
    }
}
