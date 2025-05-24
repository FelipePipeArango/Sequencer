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
        if (gridManager.ClickedTile() != null)
        {
            if (moveNumber != 0)
            {
                TileScript tile = gridManager.ClickedTile();

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
            }
        }
    }
    void PerformInput()
    {

        if (moveNumber > 0 && gridManager.AIActions == null)
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
        else if (moveNumber > 0 && gridManager.AIActions != null
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
        }
        if (isAIBefore == false)
        {
            Sequencer.sequencer.AIAfterAction();
        }
    }


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

    private void PickUpFrom(TileScript tile)
    {
        int distance = gridManager.CalculateDistance(tile.transform.position, transform.position);
        if (distance == pickUpNumber)
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


