using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AICompanion : MonoBehaviour
{
    [SerializeField] public float fallSpeed = 1.0f;

    private GameActions.AIActions action;

    private bool isBoardBelow = true;

    private void Start()
    {
        GridManager.Instance.UpdateTileType(transform.position,
            GameActions.TileTypes.PawnTile);
    }


    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Q))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        //Used constant vectors instead of hard coded numbers
        //BUG can make it go over the board fixed by the card actions
        if (Input.GetKeyDown(KeyCode.W)) /*(action == GameActions.AIActions.Forward)*/ StartCoroutine(Movement(Vector2Int.up));

        if (Input.GetKeyDown(KeyCode.S)) /*(action == GameActions.AIActions.Back)   */StartCoroutine(Movement(Vector2Int.down));

        if (Input.GetKeyDown(KeyCode.D)) /*(action == GameActions.AIActions.Right)  */StartCoroutine(Movement(Vector2Int.right));

        if (Input.GetKeyDown(KeyCode.A)) /*(action == GameActions.AIActions.Left)   */StartCoroutine(Movement(Vector2Int.left));


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

    //Changed the previous movement implementation to make it more easy to calculate
    IEnumerator Movement(Vector2Int direction)
    {
        Vector3 checkPos = new Vector3(transform.position.x + direction.x, 0, transform.position.z + direction.y);
        while (GridManager.Instance.CheckWhatNextTileIs(checkPos) != GameActions.TileTypes.None)
        {
            if (GridManager.Instance.CheckWhatNextTileIs(checkPos) == GameActions.TileTypes.GoalTile)
            {
                if (GridManager.Instance.playerActions.hasItem)
                {
                    GridManager.Instance.ResetTileType(GameActions.TileTypes.PawnTile);
                    transform.position += new Vector3(direction.x, 0, direction.y);


                    GridManager.Instance.GoalCheck();
                }

                else Debug.Log("Need key");
            }
            else if (GridManager.Instance.CheckWhatNextTileIs(checkPos) == GameActions.TileTypes.PlayerTile)
            {
                Debug.Log("Player");
                break;
            }
            else
            {
                GridManager.Instance.ResetTileType(GameActions.TileTypes.PawnTile);


                transform.position += new Vector3(direction.x, 0, direction.y);
                switch (GridManager.Instance.CheckWhatNextTileIs(checkPos))
                {
                    case GameActions.TileTypes.KeyTile:

                        GridManager.Instance.KeyItemCheck();

                        break;
                    case GameActions.TileTypes.ItemTile:

                        GridManager.Instance.NumberItemCheck();

                        break;
                    case GameActions.TileTypes.EmptyTile:

                        Debug.Log("Empty");

                        break;
                }

                GridManager.Instance.UpdateTileType(transform.position,
                    GameActions.TileTypes.PawnTile);
            }

            yield return new WaitForSeconds(0.5f);
            checkPos += new Vector3(direction.x, 0, direction.y);
        }
    }
}
