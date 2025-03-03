using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameActions;
using static GridManager;

public class UnitControler : MonoBehaviour
{
    [SerializeField] public float fallSpeed = 1.0f;

    [HideInInspector] public int number = 0;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;
    [HideInInspector] public bool canMove = false;


    private Actions awaitingActions;
    public GameObject arrow;
    private bool isBoardBelow = true;
    private bool isComplete = false;

    private void Start()
    {
        gridManager.UpdateTileType(
            transform.position, TileTypes.PlayerTile);
    }


    public void MovementReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        awaitingActions = usedAction;
        number = recievedNumber;
        isComplete = true;
    }


    public void ThrowReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        awaitingActions = usedAction;
        number = recievedNumber;

        if (gridManager.AIActions.canMove != true && canMove == true)
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
            isComplete = true;
            canMove = false;

            if (gridManager.AIActions.isBefore != true)
                gridManager.AIActions.canMove = true;

        }
    }

    public void PickUpReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        awaitingActions = usedAction;
        number = recievedNumber;

        if (gridManager.AIActions.canMove != true && canMove == true)
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
            isComplete = true;
            canMove = false;

            if (gridManager.AIActions.isBefore != true)
                gridManager.AIActions.canMove = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        if (number > 0 && canMove == true)
            if (number > 0 && gridManager.AIActions.isMoving == false)
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
        if (canMove == true && isComplete == false)
        {
            Sequencer.sequencer.PlayerAfterAction(number, awaitingActions);
            Debug.Log("Outside stuff");
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
            number = 0;
            isBoardBelow = false;
        }


        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.GoalTile)
        {
            if (hasItem)
            {
                gridManager.ResetTileType(TileTypes.PlayerTile);

                transform.position += new Vector3(direction.x, 0, direction.y);

                number--;

                gridManager.GoalCheck();
            }

            else Debug.Log("Need key");
        }
        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.PawnTile)
        {
            Debug.Log("Companion");
            gridManager.AIActions.PushCompanion(direction);
            Debug.Log("Companion");
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

            if (gridManager.AIActions != null)
                gridManager.AIActions.canMove = true;

            number--;
        }
    }



    void GoalCheck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        Debug.Log("GOAL");
    }

    void KeyItemCheck()
    {
        if (hasItem)
        {
            hasItem = true;
            gridManager.keyItem.SetActive(false);
        }
    }

    void NumberItemCheck()
    {
        gridManager.pickUpNumber.SetActive(false);
        gridManager.numberHUD.SetActive(true);
        hasNumber = true;
    }
}


