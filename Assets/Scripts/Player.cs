using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameActions;
using static GameTiles;
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
            if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(Movement(Vector2Int.up));
            else if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(Movement(Vector2Int.up));

            if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(Movement(Vector2Int.down));
            else if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(Movement(Vector2Int.down));

            if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(Movement(Vector2Int.right));
            else if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(Movement(Vector2Int.right));

            if (Input.GetKeyDown(KeyCode.A)) StartCoroutine(Movement(Vector2Int.left));
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(Movement(Vector2Int.left));
        }
        else if (number > 0 && gridManager.AIActions != null 
            && gridManager.AIActions.canMove == false)
        {
            if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(Movement(Vector2Int.up));
            else if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(Movement(Vector2Int.up));

            if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(Movement(Vector2Int.down));
            else if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(Movement(Vector2Int.down));

            if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(Movement(Vector2Int.right));
            else if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(Movement(Vector2Int.right));

            if (Input.GetKeyDown(KeyCode.A)) StartCoroutine(Movement(Vector2Int.left));
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(Movement(Vector2Int.left));
        }
        if (number == 0)
            push = 1;
        
        IfFall();
    }


    protected override IEnumerator Movement(Vector2Int direction) 
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
                yield return new WaitForSeconds(0.0f);
                push = 0;
            }
        }
        else
        {
            MoveTo(direction, TileTypes.PlayerTile);
            number--;
            yield return new WaitForSeconds(0.0f);
        }
        if(isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
        
    }

    public void MovementReceiver(int recievedNumber)
    {
        number += recievedNumber;
    }

    public void ThrowReceiver(int recievedNumber)
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

    public void PickUpReceiver(int recievedNumber)
    {
        if (recievedNumber == gridManager.CalculateDistance(
                gridManager.keyItem.transform.position, transform.position))
        {
            KeyItemCheck();
            gridManager.ResetTileType(TileTypes.KeyTile);
        }
        if (gridManager.pickUpNumber != null)
        {
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
        }
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }
}


