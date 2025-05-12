using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameActions;
using static GameTiles;
using static GridManager;
using System.Security.Cryptography.X509Certificates;

public class Player : UnitController
{

    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool isAIBefore = false;
    [HideInInspector] public int push;
   
    public UIHandler uiHandler;


    private void Start()
    {
        gridManager.UpdateTileType(
            transform.position, TileTypes.PlayerTile);
    }

    void Update()
    {




        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //if (hit.collider.gameObject != gridVFX && selectedTileType != TileTypes.None) return; // Only interact if it's the grid

                Vector3 hitPoint = hit.point;
                GameObject TileMap = new GameObject();
                TileMap.transform.position = gridManager.GetTileMap();
                Vector3 localPos = TileMap.transform.InverseTransformPoint(hitPoint);
                Vector2 gridCellSize = new Vector2(
                    TileMap.transform.localScale.x,
                    TileMap.transform.localScale.z
                    );
                int x = Mathf.FloorToInt(localPos.x / gridCellSize.x + 0.5f);
                int z = Mathf.FloorToInt(localPos.z / gridCellSize.y - 0.3f);
                
                //Right now this value is not dynamic I dont know what it does
                //but it keeps changing if I change the scene 
                Vector2Int pos = new Vector2Int(x + 3/*2*/, z + 4/*5*/);
                
                if (gridManager.GetTile(pos) != null)
                    gridManager.GetTile(pos).SetColor(Color.clear);

                Debug.Log(pos);

                Destroy(TileMap);
            }
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

    public void ThrowReceiver(int receivedNumber)
    {
        if (hasItem)
        {
            hasItem = false;
            if (receivedNumber >= gridManager.CalculateDistance(
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


