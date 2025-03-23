using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameTiles;
using static GridManager;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    public GameObject CardsInLevel;
    public GameObject NumberSlot   ;
    public GameObject CompleteLevel;

    [HideInInspector] public float fallSpeed = 1.0f;
    [HideInInspector] public bool hasItem = false;
    [HideInInspector] public bool hasNumber = false;
    [HideInInspector] public int number = 0;
    [HideInInspector] protected bool isBoardBelow = true;


    protected void MoveTo(Vector2Int direction, TileTypes spawnTileType)
    {
        Vector3 checkPos = new Vector3(
           transform.position.x + direction.x,
           0,
           transform.position.z + direction.y);

        switch (gridManager.CheckWhatNextTileIs(checkPos))
        {

            case TileTypes.GoalTile:
                { 
                    MoveToGoal(direction, spawnTileType);
                }
                break;
            case TileTypes.PlayerTile:
                { 
                    Debug.Log("Player");
                }
                break;
            case TileTypes.PawnTile:
                { 
                    Debug.Log("Companion");
                }
                break;
            case TileTypes.KeyTile:
                {
                    MoveToTile(direction, spawnTileType);

                    KeyItemCheck();
                }
                break;
            case TileTypes.ItemTile:
                {
                    MoveToTile(direction, spawnTileType);

                    NumberItemCheck();
                }
                break;
            case TileTypes.EmptyTile:
                {
                    MoveToTile(direction, spawnTileType);
                }
                break;
            
            case TileTypes.None: break;
        }

    }

   


    protected void IfFall()
    {
        if (!isBoardBelow)
        {
            Vector3 targetPos = new Vector3(transform.position.x, -1f, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);

            if (transform.position.y <= -0.99f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void MoveToTile(Vector2Int direction, TileTypes pawnTileType)
    {
        gridManager.ResetTileType(pawnTileType);

        transform.position += new Vector3(direction.x, 0, direction.y);

        gridManager.UpdateTileType(transform.position, pawnTileType);
    }

    private void MoveToGoal(Vector2Int direction, TileTypes pawnTileType)
    {
        if (hasItem)
        {
            MoveToTile(direction, pawnTileType);

            GoalCheck();
        }
        else
            Debug.Log("Need key");
    }

    protected virtual IEnumerator Movement(Vector2Int direction){ return null; }

    protected void GoalCheck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        //if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        //{
        //    SceneManager.LoadScene(nextSceneIndex);
        //}
        Debug.Log("GOAL");
        //TODO Add functional for this function
        //make it so when triggerred makes the completelevel active and everything else disabled 
        StartCoroutine(CompleteScreen(1));
    }

    IEnumerator CompleteScreen(int count)
    {
        if (count < 0)
        {
          yield return null;
        }
        if (count == 1)
        {
            CardsInLevel.GameObject().SetActive(false);
            NumberSlot.GameObject().SetActive(false);
            CompleteLevel.GameObject().SetActive(true);

            yield return new WaitForSeconds(5.0f);
        }
        else if(count == 0)
        {
            MenuScreen();
        }
        StartCoroutine(CompleteScreen(count-1));
    }
    void MenuScreen()
    {
        SceneManager.LoadScene("MainMenu");
    }



    protected void KeyItemCheck()
    {
        if (hasItem != true)
        {
            hasItem = true;
            gridManager.keyItem.SetActive(false);
        }
    }

    protected void NumberItemCheck()
    {
        if(gridManager.pickUpNumber != null)
            gridManager.pickUpNumber.SetActive(false);
        gridManager.numberHUD.SetActive(true);
        hasNumber = true;
    }
}

