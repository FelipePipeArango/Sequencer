using UnityEngine;
using UnityEngine.SceneManagement;
using static GameActions;
using static GridManager;

public class Player : UnitController
{

    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool isAIBefore = false;
    [HideInInspector] public int push;


    private void Start()
    {
        gridManager.UpdateTileType(
            transform.position, TileTypes.PlayerTile);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        if (number > 0 && gridManager.AIActions == null)
        {
            if (Input.GetKeyDown(KeyCode.W)) Movement(Vector2Int.up);

            if (Input.GetKeyDown(KeyCode.S)) Movement(Vector2Int.down);

            if (Input.GetKeyDown(KeyCode.D)) Movement(Vector2Int.right);

            if (Input.GetKeyDown(KeyCode.A)) Movement(Vector2Int.left);
        }
        else if (number > 0 && gridManager.AIActions != null 
            && gridManager.AIActions.canMove == false)
        {
            if (Input.GetKeyDown(KeyCode.W)) Movement(Vector2Int.up);

            if (Input.GetKeyDown(KeyCode.S)) Movement(Vector2Int.down);

            if (Input.GetKeyDown(KeyCode.D)) Movement(Vector2Int.right);

            if (Input.GetKeyDown(KeyCode.A)) Movement(Vector2Int.left);
        }
        if (number == 0)
            push = 1;
        
        IfFall();
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
        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.PawnTile)
        {
            if (push == 1)
            {
                gridManager.AIActions.PushCompanion(direction);
                MoveTo(direction, TileTypes.PlayerTile);
                number--;

                push = 0;
            }
        }
        else
        {
            MoveTo(direction, TileTypes.PlayerTile);
            number--;
        }
        if(isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }


    public void ThrowReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        if (hasItem)
        {
            hasItem = false;
            if (recievedNumber >= gridManager.CalculateDistance(
                    gridManager.goal.transform.position, transform.position))
            {
                GoalCheck();
            }
        }
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }

    public void PickUpReceiver(int recievedNumber, GameActions.Actions usedAction)
    {
        if (recievedNumber == gridManager.CalculateDistance(
                gridManager.keyItem.transform.position, transform.position))
        {
            KeyItemCheck();
            gridManager.ResetTileType(TileTypes.KeyTile);
        }

        if (recievedNumber == gridManager.CalculateDistance(
                gridManager.pickUpNumber.transform.position, transform.position))
        {
            if (gridManager.pickUpNumber != null ||
                gridManager.numberHUD != null)
            {
                NumberItemCheck();
                gridManager.ResetTileType(TileTypes.ItemTile);
            }
        }
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }
}


