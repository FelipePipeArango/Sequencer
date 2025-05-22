using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameTiles;
using static GridManager;
using System.Net.Security;
using static UnityEditor.PlayerSettings;


public class Player : UnitController
{
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canThrow = false;
    [HideInInspector] public bool canPickUp = false;

    [HideInInspector] public bool isAIBefore = false;
    [HideInInspector] public int push;
    [HideInInspector] public int throwNumber;
    [HideInInspector] public int pickUpNumber;

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
        else if (canPickUp)
        {
            gridManager.PickUpHighLight(pickUpNumber);
            if (Input.GetMouseButtonDown(0))
            {
                PickUp();
            }
        }
        else if (canMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Move();
            }
        }
    }

    void PickUp()
    {
        if (gridManager.ClickedTile() != null)
        {
            PickUpFrom(gridManager.ClickedTile());
            gridManager.TurnOffHighlight();
        }
    }

    void Throw()
    {
        if (gridManager.ClickedTile() != null)
        {
            ThrowTo(gridManager.ClickedTile());
            gridManager.TurnOffHighlight();
        }
    }
    void Move()
    {
        Debug.Log("Move");
        //Can move one by one 

        //click a tile if can mmove to it "teleport" at first
        //then make it go tile by tile by using path finding on a grid algorythm
        //destination validated by amount of moves
        //goes through the tiles by comparing the current with the destination
        //
        //It can either iterate once at a time or make the calculation once
        //which would also require it to itarate same amount of times
        //
        //I am thinking of improving path finding system i guess
        //Recieved pos(x,y) from click 
        //Direction("The arrow directions") StorePathForNumber(recievedNumber)
        //for(recievedNumber
        //if(pos.x == Pos.x but pos.y >= Pos.y) turn back 
        //else if(pos.x == Pos.x but pos.y <= Pos.y) turn up 
        //else if(pos.y == Pos.y but pos.x >= Pos.x) turn left 
        //else if(pos.y == Pos.y but pos.x <= Pos.x) turn right 
        //
        //How to make it dynamic 
        //I can store a calulated path in a variable
        //and only call it's when needed
        //
        //Design hought: if player would be draged by a hand on the board
        //then I think it make sense to make the gaps on the tile map 
        //as pillars that even by hand wouldnt be able to be passed through

        if (gridManager.ClickedTile() != null)
        {
            TileScript tile = gridManager.ClickedTile();
            int clickedTileDistance = gridManager.CalculateDistance(
                tile.transform.position, transform.position);

            if (clickedTileDistance <= 1)
            {
                Vector3 pos = tile.transform.position;
                Vector3 playerPos = transform.position;
                if (pos.x == playerPos.x)
                {
                    if (pos.z > playerPos.z) StartCoroutine(Movement(Vector2Int.up));
                    else if (pos.z < playerPos.z) StartCoroutine(Movement(Vector2Int.down));
                }
                else if (pos.z == playerPos.z)
                {
                    if (pos.x > playerPos.x) StartCoroutine(Movement(Vector2Int.right));
                    else if (pos.x < playerPos.x) StartCoroutine(Movement(Vector2Int.left));
                }
                //if diagonaly what happens?
                //Move twice to which direction?
                //or just let it be one at a time for now 
            }
        }
    }
    void PerformInput()
    {

        if (moveNumber > 0 && gridManager.AIActions == null)
        {

        }
        else if (moveNumber > 0 && gridManager.AIActions != null
            && gridManager.AIActions.canMove == false)
        {

        }
        if (moveNumber == 0)
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
            moveNumber = 0;
            canMove = false;
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
                moveNumber--;

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
            moveNumber--;

            /*if (uiHandler != null)
                uiHandler.UpdateMovementPointsText(number);*/

            yield return new WaitForSeconds(0.0f);

            //animController.UpdateAnimations(false);
        }
        PlayerManager.playerManagerInstance.PlayerMoved(moveNumber);

        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
        if (moveNumber == 0) canMove = false;

    }

    public void MovementReceiver(int receivedNumber)
    {
        moveNumber += receivedNumber;
        canMove = true;

        /*if (uiHandler != null)
        {
            uiHandler.UpdateMovementPointsText(number);
        }*/
        PlayerManager.playerManagerInstance.PlayerPreMove(moveNumber);
        UIHandler.UIHandlerInstance.UpdateMovementPointsText(moveNumber);
    }




    public void ThrowReceiver(int receivedNumber)
    {
        if (hasItem)
        {
            canThrow = true;
            throwNumber = receivedNumber;
            hasItem = false;
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


    private void ThrowTo(TileScript tile)
    {
        if (throwNumber == gridManager.CalculateDistance(
                   tile.transform.position, transform.position))
        {
            if (tile.tileType == TileTypes.GoalTile)
            {
                canThrow = false;
                PlayerManager.playerManagerInstance.PlayerWon();
            }
            else if (tile.tileType == TileTypes.EmptyTile)
            {
                ThrowKey(tile);
                canThrow = false;

            }
        }
    }
    /**/
    private void PickUpFrom(TileScript tile)
    {
        if (tile.tileType == TileTypes.KeyTile)
        {
            PlayerManager.playerManagerInstance.PlayerPickedUp();
            KeyItemCheck();
            gridManager.ResetTileType(TileTypes.KeyTile);
        }
        if (tile.tileType == TileTypes.ItemTile)
        {
            Debug.Log("Click");
            NumberItemCheck();
            gridManager.ResetTileType(TileTypes.ItemTile);
        }
        if (hasNumber && hasItem)
                canPickUp = false;
        
        if(gridManager.numberPickUp == null)
        {
            if (hasNumber)
                canPickUp = false;
        }
    }

    public void PickUpReceiver(int receivedNumber)
    {
        pickUpNumber = receivedNumber;
        canPickUp = true;
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }
}


