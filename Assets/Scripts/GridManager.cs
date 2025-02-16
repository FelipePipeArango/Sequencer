using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Color = UnityEngine.Color;

public class GridManager : MonoBehaviour
{

    public static GridManager Instance { get; private set; }
    [SerializeField] GameObject Tile;
    [SerializeField] Color color;

    
    [SerializeField] Vector2Int size;
    private GameObject[] allTiles;
    public TileScript[,] grid;
    TileScript[] tiles;

    [Header("Pieces")] 
    
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject keyItem;
    [SerializeField] public GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")] [SerializeField]
    public GameObject pickUpNumber;

    public UnitControler playerActions;

    public int distaceToItem, distaceToGoal, distaceToNumber;


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this; // Assign the static instance


        playerActions = player.GetComponent<UnitControler>();
        grid = new TileScript[size.x, size.y];
        StoreGrid();
    }

    private void Start()
    {
        distaceToItem = CalculateDistance(keyItem.transform.position);
        distaceToGoal = CalculateDistance(goal.transform.position);

        if (pickUpNumber != null)
            distaceToNumber = CalculateDistance(pickUpNumber.transform.position);


        //board = new object[size.x, size.y];
        Vector2Int PlayerPosition = PosConverter(player.transform.position);
        // Clamp player position to ensure it fits within the board size
        //board[PlayerPosition.x, PlayerPosition.y] = playerActions.moveAmount;

        Vector2Int keyPosition = PosConverter(keyItem.transform.position);
        // board[keyPosition.x, keyPosition.y] = keyItem.tag;

        if (pickUpNumber != null)
        {
            Vector2Int PickUpNumberPosition = PosConverter(pickUpNumber.transform.position);
            //board[PickUpNumberPosition.x, PickUpNumberPosition.y] = pickUpNumber.tag;
        }

        //undoManager.InitialState(board);
    }

    public int CalculateDistance(Vector3 position)
    {
        Vector2Int currentPosition = new Vector2Int(
            Mathf.FloorToInt(playerActions.transform.position.x),
            Mathf.FloorToInt(playerActions.transform.position.z)
        );
        Vector2Int targetPosition = new Vector2Int(
            Mathf.FloorToInt(position.x),
            Mathf.FloorToInt(position.z)
        );
        int distance = Mathf.Abs(currentPosition.x - targetPosition.x) +
                       Mathf.Abs(currentPosition.y - targetPosition.y);
        return distance;
    }

    public Vector2Int PosConverter(Vector3 converted)
    {
        Vector2Int PosConverted = new Vector2Int(
            Mathf.Clamp(Mathf.FloorToInt(converted.x), 0, size.x - 1),
            Mathf.Clamp(Mathf.FloorToInt(converted.z), 0, size.y - 1));
        return PosConverted;
    }

    void CheckIfGround(int amount, Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < size.x &&
            pos.z >= 0 && pos.z < size.y)
        {
            if (grid[(int)pos.x, (int)pos.z] != null)
            {
                grid[(int)pos.x, (int)pos.z].SetColor(color);
                MoveDistanceHighLight(amount - 1, grid[(int)pos.x, (int)pos.z].transform.position);
            }
        }
    }

    public void MoveDistanceHighLight(int amount, Vector3 start)
    {
        if (amount == 0)
        {
            return;
        }

        Vector3 right = new Vector3(start.x - 1, start.y, start.z);
        Vector3 left = new Vector3(start.x + 1, start.y, start.z);
        Vector3 up = new Vector3(start.x, start.y, start.z + 1);
        Vector3 down = new Vector3(start.x, start.y, start.z - 1);

        CheckIfGround(amount, right);
        CheckIfGround(amount, left);
        CheckIfGround(amount, up);
        CheckIfGround(amount, down);
    }

    public void PickUpThrowHighLight(int amount)
    {
        int distance;
        foreach (var tile in tiles)
        {
            distance = CalculateDistance(tile.transform.position);
            if (distance == amount)
            {
                tile.SetColor(color);
            }
        }
    }

    public void ResetAllColor()
    {
        if (allTiles != null)
        {
            foreach (TileScript tile in tiles)
            {
                tile.ResetColor();
            }
        }
    }

    void StoreGrid()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Ground");
        if (allTiles != null)
        {
            Debug.Log(allTiles.Length);
            tiles = new TileScript[allTiles.Length];

            for (int i = 0; i < allTiles.Length; i++)
            {
                tiles[i] = allTiles[i].GetComponent<TileScript>();
            }
        }


        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                foreach (var tile in tiles)
                {
                    if (new Vector3(i, 0, j) == tile.transform.position)
                    {
                        grid[i, j] = tile;
                    }
                }
            }
        }
    }

    public Vector3 GetPlayerPos()
    {
        return playerActions.transform.position;
    }

    void GoalCheck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        distaceToGoal = CalculateDistance(goal.transform.position);

        if (distaceToGoal == 0 && playerActions.hasItem)
        {
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

    void KeyItemCheck()
    {
        if (!playerActions.hasItem)
        {
            distaceToItem = CalculateDistance(keyItem.transform.position);

            if (distaceToItem == 0)
            {
                Vector2Int currentPosition = PosConverter(player.transform.position);
                playerActions.hasItem = true;
                keyItem.SetActive(false);
            }
        }
    }

    void NumberItemCheck()
    {
        if (pickUpNumber != null)
        {
            distaceToNumber = CalculateDistance(pickUpNumber.transform.position);

            if (distaceToNumber == 0 && !playerActions.hasNumber)
            {
                Vector2Int currentPosition = PosConverter(player.transform.position);

                pickUpNumber.SetActive(false);
                GameManager.Instance.numberHUD.SetActive(true);
                playerActions.hasNumber = true;
            }
        }
    }

    void PickUpCheck(GameActions.Actions usedAction, GameObject affected)
    {
        if (pickUpNumber == null && affected == pickUpNumber)
        {
            Debug.Log("Pick Up Number is null");
        }
        else
        {
            if (usedAction == GameActions.Actions.PickUp && affected != null)

            {
                Vector2Int affectedPosition = PosConverter(affected.transform.position);
            }
        }

    }

    public void ReCalculateBoard(GameActions.Actions usedAction, GameObject affected)
    {
        GoalCheck();

        KeyItemCheck();

        NumberItemCheck();

        PickUpCheck(usedAction, affected);
    }
}
