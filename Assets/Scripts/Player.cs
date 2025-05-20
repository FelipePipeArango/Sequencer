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

    //[SerializeField] Player_AnimController animController;
   
    //public UIHandler uiHandler;


    private void Start()
    {
        gridManager.UpdateTileType(
            transform.position, TileTypes.PlayerTile);
    }


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
    }

    void PerformInput()
    {
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

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        ClickOnTheBoard();
        PerformInput();
        IfFall();
    }

    }

    protected override IEnumerator Movement(Vector2Int direction)
    {

        //I am thinking of improving path finding system i guess
        //pos(x,y) 
        //if(pos.x == Pos.x but pos.y >= Pos.y) turn back 
        //if(pos.x == Pos.x but pos.y <= Pos.y) turn up 
        //if(pos.y == Pos.y but pos.x >= Pos.x) turn left 
        //if(pos.y == Pos.y but pos.x <= Pos.x) turn right 
        
        //animController.UpdateAnimations(true); 

        Vector3 checkPos = new Vector3(
            transform.position.x + direction.x,
            0,
            transform.position.z + direction.y);

        if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.None)
        {
            transform.position += new Vector3(direction.x, 0, direction.y);
            number = 0;
            isBoardBelow = false;

            /*if (uiHandler != null)
                uiHandler.UpdateMovementPointsText(number);*/
        }
        else if (gridManager.CheckWhatNextTileIs(checkPos) == TileTypes.PawnTile)
        {
            if (push == 1)
            {
                gridManager.AIActions.PushCompanion(direction);
                MoveTo(direction, TileTypes.PlayerTile);
                number--;

                /*if (uiHandler != null)
                    uiHandler.UpdateMovementPointsText(number);*/

                yield return new WaitForSeconds(0.0f);
                //animController.UpdateAnimations(false); 
                push = 0;
            }
        }
        else
        {
            MoveTo(direction, TileTypes.PlayerTile);
            number--;

            /*if (uiHandler != null)
                uiHandler.UpdateMovementPointsText(number);*/

            yield return new WaitForSeconds(0.0f);

            //animController.UpdateAnimations(false);
        }
        PlayerManager.playerManagerInstance.PlayerMoved(number);

        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }

    public void MovementReceiver(int receivedNumber)
    {
        number += receivedNumber;

        /*if (uiHandler != null)
        {
            uiHandler.UpdateMovementPointsText(number);
        }*/
        PlayerManager.playerManagerInstance.PlayerPreMove(number);
        UIHandler.UIHandlerInstance.UpdateMovementPointsText(number);
    }
   

  

    public void ThrowReceiver(int receivedNumber)
    {
        if (hasItem)
        {
            canThrow = true;
            throwNumber = receivedNumber;
            
            hasItem = false;
            if (receivedNumber >= gridManager.CalculateDistance(
                    gridManager.goal.transform.position, transform.position))
            {
                PlayerManager.playerManagerInstance.PlayerWon();
            }
        }
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }


    //I dont like this being in Player.cs
    //I think it should be in grid manager or somewhere else 
    /**/

    //Grid manager is already bloateed...
    void Throw()
    {
        if (gridManager.ClickedTile() != null)
        {
            ThrowTo(throwNumber, gridManager.ClickedTile());
            gridManager.TurnOffHighlight();
        }
    }

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
    /**/

    public void PickUpReceiver(int receivedNumber)
    {

        if (receivedNumber == gridManager.CalculateDistance(
                gridManager.keyItem.transform.position, transform.position))
        {
            PlayerManager.playerManagerInstance.PlayerPickedUp();
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


