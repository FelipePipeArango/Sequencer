using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitControler : MonoBehaviour
{
    private Vector2Int playerPreviousPosition;

    public int moveAmount = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;


    [SerializeField] public float fallSpeed = 1.0f;

    private bool isBoardBelow = true;
    private void Start()
    {
        GridManager.Instance.UpdateTileType(transform.position,
            GameActions.TileTypes.PlayerTile);
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
            if (recievedNumber >= GridManager.Instance.CalculateDistance(
                    GridManager.Instance.goal.transform.position, transform.position))
            {
                GridManager.Instance.GoalCheck();
            }
        }
    }

    public void PickUpReceiver (int recievedNumber)
    {
        if (recievedNumber == GridManager.Instance.CalculateDistance(
                GridManager.Instance.keyItem.transform.position, transform.position))
        {
            GridManager.Instance.KeyItemCheck();
            GridManager.Instance.ResetTileType(GameActions.TileTypes.KeyTile);
        }

        if (recievedNumber == GridManager.Instance.CalculateDistance(
                GridManager.Instance.pickUpNumber.transform.position, transform.position))
        {
            if (GridManager.Instance.pickUpNumber != null ||
                GridManager.Instance.numberHUD != null)
            {
                GridManager.Instance.NumberItemCheck();
                GridManager.Instance.ResetTileType(GameActions.TileTypes.KeyTile);
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
        if (moveAmount > 0)
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

    

    //Changed the previous movement implementation to make it more easy to calculate
    void Movement(Vector2Int direction)
    {
        Vector3 checkPos = new Vector3(
            transform.position.x + direction.x, 0, transform.position.z + direction.y);
        if (GridManager.Instance.CheckWhatNextTileIs(checkPos) == GameActions.TileTypes.None)
        {
            transform.position += new Vector3(direction.x, 0, direction.y);
            moveAmount = 0;
            isBoardBelow = false;
        }
        else if (GridManager.Instance.CheckWhatNextTileIs(checkPos) == GameActions.TileTypes.GoalTile)
        {
            if (hasItem)
            {
                GridManager.Instance.ResetTileType(GameActions.TileTypes.PlayerTile);
                transform.position += new Vector3(direction.x, 0, direction.y);

                moveAmount--;

                GridManager.Instance.GoalCheck();
            }

            else Debug.Log("Need key");
        }
        else if (GridManager.Instance.CheckWhatNextTileIs(checkPos) == GameActions.TileTypes.PawnTile)
        {
            Debug.Log("Companion");
        }
        else
        {
            GridManager.Instance.ResetTileType(GameActions.TileTypes.PlayerTile);

            
            transform.position += new Vector3(direction.x, 0, direction.y);
            switch (GridManager.Instance.CheckWhatNextTileIs(checkPos))
            {
                case GameActions.TileTypes.KeyTile:
                    
                    GridManager.Instance.KeyItemCheck();
                   
                    break;
                case GameActions.TileTypes.ItemTile:
                    
                    GridManager.Instance.NumberItemCheck();
                    
                    break;
                case GameActions.TileTypes.EmptyTile:
                    
                    Debug.Log("Empty");
                    
                    break;
            }

            GridManager.Instance.UpdateTileType(transform.position,
                GameActions.TileTypes.PlayerTile);

            moveAmount--;
        }
    }
}
