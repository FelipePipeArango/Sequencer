using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AICompanion : MonoBehaviour
{

    public delegate void PickUpObject(GameActions.Actions action, GameObject affected);

    public static event PickUpObject OnObjectPickUp;

    public delegate void MoveAction(GameActions.Actions usedAction, GameObject affected);

    public static event MoveAction OnMovement;

    private Vector2Int playerPreviousPosition;

    public int moveAmount = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;

    private WaitForSeconds wait;

    [SerializeField] public float fallSpeed = 1.0f;

    private GameActions.AIActions action;

    private void Start()
    {
        GridManager.Instance.UpdateTileType(transform.position,
            GameActions.TileTypes.PawnTile);
    }

    public void MovementReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        moveAmount = recievedNumber;
    }

    //Manages the PickUp action
    public void PickUpReceiver
    (int recievedNumber, int distanceToItem, int distanceToNumber,
        GameObject item, GameObject pickUpNumber, GameObject numberHUD)
    {
        if (!hasItem)
        {
            if (recievedNumber == distanceToItem)
            {
                hasItem = true;
                item.SetActive(false);

                if (OnObjectPickUp != null)
                    OnObjectPickUp(GameActions.Actions.PickUp, item);

            }
        }
        else
        {
            if (OnObjectPickUp != null)
                OnObjectPickUp(GameActions.Actions.PickUp, null);

        }

        if (!hasNumber)
        {
            if (distanceToNumber != 0 && recievedNumber == distanceToNumber)
            {
                if (pickUpNumber != null || numberHUD != null)
                {
                    pickUpNumber.SetActive(false);
                    numberHUD.SetActive(true);
                }

                hasNumber = true;

                if (OnObjectPickUp != null)
                    OnObjectPickUp(GameActions.Actions.PickUp, pickUpNumber);
            }
        }
        else
        {
            if (OnObjectPickUp != null)
                OnObjectPickUp(GameActions.Actions.PickUp, null);

        }
    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        //Used constant vectors instead of hard coded numbers
        if /*(Input.GetKeyDown(KeyCode.W))*/ (action == GameActions.AIActions.Forward) Movement(Vector2Int.up);

        if /*(Input.GetKeyDown(KeyCode.S))*/ (action == GameActions.AIActions.Back) Movement(Vector2Int.down);

        if /*(Input.GetKeyDown(KeyCode.D))*/ (action == GameActions.AIActions.Right) Movement(Vector2Int.right);

        if /*(Input.GetKeyDown(KeyCode.A))*/ (action == GameActions.AIActions.Left) Movement(Vector2Int.left);


        if (!IsBoardBelow())
        {
            Vector3 targetPos = new Vector3(transform.position.x, -1f, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);

            if (transform.position.y <= -0.99f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private bool IsBoardBelow()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f))
        {
            return true;
        }

        return false;
    }

    //Changed the previous movement implementation to make it more easy to calculate
    void Movement(Vector2Int direction)
    {
        Vector3 checkPos = new Vector3(transform.position.x + direction.x, 0, transform.position.z + direction.y);
        while (GridManager.Instance.CheckWhatNextTileIs(checkPos) != GameActions.TileTypes.None)
        {
            if (GridManager.Instance.CheckWhatNextTileIs(checkPos) == GameActions.TileTypes.PlayerTile)
            {
                break;
            }

            GridManager.Instance.UpdateTileType(transform.position,
                GameActions.TileTypes.EmptyTile);

            transform.position += new Vector3(direction.x, 0, direction.y);

            GridManager.Instance.UpdateTileType(transform.position,
                GameActions.TileTypes.PawnTile);

            moveAmount--;

            if (OnMovement != null)
            {
                OnMovement(GameActions.Actions.Move, null);
            }

            checkPos += new Vector3(direction.x, 0, direction.y);
        }
    }


    public void ExecuteMove()
    {

        //
        //if ground check is true movement 
        //if false movement and moveAmount = 0
        //if check ground is null Stay
        //if checkground to the (direction)left is Empty (AIAction)Left
        //if checkGround to the (direction)left is PlayerTile (AIAction) Stay
        //if checkGround to the (direction)left is ItemTile||KeyTile||GoalTile (AIAction)//
        //if AIAction = //
    }
}
