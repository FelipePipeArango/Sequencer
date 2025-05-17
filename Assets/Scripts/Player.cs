using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameActions;
using static GameTiles;
using static GridManager;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;

public class Player : UnitController
{

    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canThrow = false;
    [HideInInspector] public bool isAIBefore = false;
    [HideInInspector] public int push;
    [HideInInspector] public int throwNumber;
   
    public UIHandler uiHandler;


    private void Start()
    {
        gridManager.UpdateTileType(
            transform.position, TileTypes.PlayerTile);
    }

    //I dont like this being in Player.cs
    //I think it should be in grid manager or somewhere else 
    /**/
    private void ThrowTo(int receivedNumber, TileScript tile)
    {
        if (receivedNumber == gridManager.CalculateDistance(
                   tile.transform.position, transform.position))
        {
            if (tile.tileType == TileTypes.GoalTile)
            {
                canThrow = false;
                GoalCheck();
            }
            else if (tile.tileType == TileTypes.EmptyTile)
            {
                ThrowKey(tile);
                canThrow = false;

            }
        }
    }
    //Grid manager is already bloateed...
    void Throw()
    {
        if (gridManager.ClickedTile() != null)
        {
            ThrowTo(throwNumber, gridManager.ClickedTile());
            gridManager.TurnOffHighlight();
        }
    }
    /**/

    void ClickOnTheBoard()
    {
        if (canThrow)
        {
            gridManager.ThrowHighLight(throwNumber);
            if (Input.GetMouseButtonDown(0))
            {
                Throw();
            }
        }

        else if (number > 0 && gridManager.AIActions == null)
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

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        ClickOnTheBoard();
        
        IfFall();
    }


    protected override IEnumerator Movement(Vector2Int direction)
    {

        //I am thinking of improving path finding system i guess
        //pos(x,y) 
        //if(pos.x == Pos.x but pos.y >= Pos.y) turn back 
        //if(pos.x == Pos.x but pos.y <= Pos.y) turn up 
        //if(pos.y == Pos.y but pos.x >= Pos.x) turn left 
        //if(pos.y == Pos.y but pos.x <= Pos.x) turn right 
        Vector3 checkPos = new Vector3(
            transform.position.x + direction.x,
            0,
            transform.position.z + direction.y);

        if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.None)
        {
            transform.position += new Vector3(direction.x, 0, direction.y);
            number = 0;
            isBoardBelow = false;

            if (uiHandler != null)
                uiHandler.UpdateNumberText(number);
        }
        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.PawnTile)
        {
            if (push == 1)
            {
                gridManager.AIActions.PushCompanion(direction);
                MoveTo(direction, TileTypes.PlayerTile);
                number--;

                if (uiHandler != null)
                    uiHandler.UpdateNumberText(number);

                yield return new WaitForSeconds(0.0f);
                push = 0;
            }
        }
        else
        {
            MoveTo(direction, TileTypes.PlayerTile);
            number--;

            if (uiHandler != null)
                uiHandler.UpdateNumberText(number);

            yield return new WaitForSeconds(0.0f);
        }

        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }

    public void MovementReceiver(int receivedNumber)
    {
        number += receivedNumber;

        if (uiHandler != null)
        {
            uiHandler.UpdateNumberText(number);
        }
    }
   

  

    public void ThrowReceiver(int receivedNumber)
    {
        if (hasItem)
        {
            canThrow = true;
            throwNumber = receivedNumber;
        }
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }

    public void PickUpReceiver(int receivedNumber)
    {

        if (receivedNumber == gridManager.CalculateDistance(
                gridManager.keyItem.transform.position, transform.position))
        {
            KeyItemCheck();
            gridManager.ResetTileType(TileTypes.KeyTile);
        }
        if (gridManager.pickUpNumber != null)
        {
            if (receivedNumber == gridManager.CalculateDistance(
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


