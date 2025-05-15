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
    
    void Update()
    {
        if (canThrow)
        {
            ClickToThrow();
        }
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
    //
    private void ThrowTo(int receivedNumber, TileScript tile)
    {
        if (receivedNumber >= gridManager.CalculateDistance(
                   tile.transform.position, transform.position))
        {
            ThrowKey(tile);
            canThrow = false;
        }
    }

    void ClickToThrow()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point;
                GameObject TileMap = new GameObject();
                TileMap.transform.position = gridManager.GetTileMap();
                Vector3 localPos = TileMap.transform.InverseTransformPoint(hitPoint);
                Vector2 gridCellSize = new Vector2(
                    TileMap.transform.localScale.x,
                    TileMap.transform.localScale.z
                    );
                int x = Mathf.FloorToInt(localPos.x / gridCellSize.x + 0.5f);
                int z = Mathf.FloorToInt(localPos.z / gridCellSize.y - 0.4f);

                Vector2Int pos = new Vector2Int(
                    x + gridManager.size.x,
                    z + gridManager.size.y + 1
                    );
               
                if (gridManager.GetTile(pos) != null)
                {
                    if (gridManager.GetTile(pos).tileType == TileTypes.GoalTile)
                    {
                        canThrow = false;
                        GoalCheck();
                    }
                    else if (gridManager.GetTile(pos).tileType == TileTypes.EmptyTile)
                    { 
                        ThrowTo(throwNumber, gridManager.GetTile(pos));
                    }

                }
                Destroy(TileMap);
            }
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


