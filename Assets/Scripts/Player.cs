using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameTiles;
using static GridManager;


public class Player : UnitController
{
    [SerializeField] public float pickUpOffset = 0.5f;

    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canThrow = false;
    [HideInInspector] public bool canPickUp = false;
    [HideInInspector] public bool canEnable = false;

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

    void Update()
    {
        PerformInput();
        IfFall();
    }
    private void SetStateChange(bool state)
    {
        if (state != Sequencer.sequencer.state)   
            Sequencer.sequencer.HandleStateChange(state);
        else
            return;
    }
    private void ClickToPickUp()
    {
        if (IsSomethingWithinPickUpRadiusOfPlayer(pickUpNumber))
        {
            SetStateChange(true);
            gridManager.PickUpHighLight(pickUpNumber);
            if (Input.GetMouseButtonDown(0))
            {
                if (gridManager.ClickedTile(pickUpOffset) != null)
                {
                    PickUpFrom(gridManager.ClickedTile(pickUpOffset));
                    gridManager.TurnOffHighlight();
                }
            }
        }
        else
        {
            Debug.Log("Nothing to pick up");
            canPickUp = false;
            SetStateChange(false);
        }
    }
    private void ClickToThrow()
    {
        if (hasItem)
        {
            SetStateChange(true);
            gridManager.ThrowHighLight(throwNumber);
            if (Input.GetMouseButtonDown(0))
            {
                if (gridManager.ClickedTile(0.6f) != null)
                {
                    ThrowTo(gridManager.ClickedTile(0.6f));
                    gridManager.TurnOffHighlight();
                }
            }
        }
        else
        {
            Debug.Log("Nothing to throw");
            canThrow = false;
            SetStateChange(false);
        }
    }
    private void ClickToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gridManager.ClickedTile(0.6f) != null)
            {
                if (moveNumber != 0)
                {
                    TileScript tile = gridManager.ClickedTile(0.6f);

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
    }
    private void CanEnable()
    {
        canEnable = false;
        Sequencer.sequencer.AICanMoveNow();
    }
    private void WASD_Arrows()
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
    private void WASDToMove()
    {
        if (moveNumber > 0 && gridManager.AIActions == null)
        {
            WASD_Arrows();
        }
        else if (moveNumber > 0 && gridManager.AIActions != null
            && gridManager.AIActions.canMove == false)
        {
            WASD_Arrows();
        }
    }
    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }


    private void PerformInput()
    {
        if (canThrow)
        {
            ClickToThrow();
            return;
        }
        else if(canEnable)
        {
            CanEnable();
            return;
        }
        else if (canPickUp)
        {
            ClickToPickUp();
            return;
        }
        else if (canMove && 
                    (gridManager.AIActions == null ||
                     gridManager.AIActions.canMove != true)
                )
        {
            ClickToMove();
            WASDToMove();
            if (moveNumber == 0)
                push = 1;
        }
        Reload();
    }

    public void EnableReceiver()
    {
        canEnable = true;
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
        canThrow = true;
        throwNumber = receivedNumber; 
    }
    public void PickUpReceiver(int receivedNumber)
    {
        pickUpNumber = receivedNumber;
        canPickUp = true;
    }

    protected override IEnumerator Movement(Vector2Int direction)
    {
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
        Sequencer.sequencer.AICanMoveNow();
        if (moveNumber == 0)
        {
            canMove = false;
            push = 1;
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
                Debug.Log("Click");
                ThrowKey(tile);
                canThrow = false;
                SetStateChange(false);
                Sequencer.sequencer.AICanMoveNow();
            }
        }
        else
        {
            Debug.Log("too close to throw");
        }
    }


    private void PickUpFrom(TileScript tile)
    {

        if (tile.tileType == TileTypes.KeyTile)
        {
            PlayerManager.playerManagerInstance.PlayerPickedUp();
            KeyItemCheck();
            gridManager.ResetTileType(TileTypes.KeyTile);
            canPickUp = false;
            SetStateChange(false);
            Sequencer.sequencer.AICanMoveNow();
        }
        else if (tile.tileType == TileTypes.ItemTile)
        {
            NumberItemCheck();
            gridManager.ResetTileType(TileTypes.ItemTile);
            canPickUp = false;
            SetStateChange(false);
            Sequencer.sequencer.AICanMoveNow();
        }
        else
        {
            Debug.Log("Nothing to pick up");
        }

    }
}


