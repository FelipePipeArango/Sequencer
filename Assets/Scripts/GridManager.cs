
using UnityEngine;
using UnityEngine.SceneManagement;
using Color = UnityEngine.Color;
using static GameActions;

public class GridManager : MonoBehaviour
{
    public static GridManager gridManager { get; private set; }

    [SerializeField] Color color;

    [SerializeField] Vector2Int size;
    private GameObject[] allTiles;
    public TileScript[,] grid;
    TileScript[] tiles;

    [Header("Pieces")] [SerializeField] 
    public GameObject AICompanion;
    public GameObject player;
    public GameObject keyItem;
    public GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")] [SerializeField]
    public GameObject pickUpNumber;
    public GameObject numberHUD; //Should at some point go to card manager
    public UnitControler playerActions;
    public AICompanion companion;


    void Awake()
    {
        if (gridManager != null && gridManager != this)
        {
            Destroy(gameObject); 
            return;
        }

        gridManager = this;

        playerActions = player.GetComponent<UnitControler>();
       
        if(AICompanion != null)
            AIActions = AICompanion.GetComponent<AICompanion>();  
        
        grid = new TileScript[size.x, size.y];
        StoreGrid();
    }

    private void Start()
    {
        UpdateTileType(keyItem.transform.position, TileTypes.KeyTile);
        UpdateTileType(pickUpNumber.transform.position, TileTypes.ItemTile);
        UpdateTileType(goal.transform.position, TileTypes.GoalTile);
    }

    public int CalculateDistance(Vector3 position, Vector3 start)
    {
        Vector2Int currentPosition = new Vector2Int(
            Mathf.FloorToInt(start.x),
            Mathf.FloorToInt(start.z)
        );
        Vector2Int targetPosition = new Vector2Int(
            Mathf.FloorToInt(position.x),
            Mathf.FloorToInt(position.z)
        );
        int distance = Mathf.Abs(currentPosition.x - targetPosition.x) +
                       Mathf.Abs(currentPosition.y - targetPosition.y);
        return distance;
    }
   
    public void TurnOnHighlight(bool isMoveAction, int value)
    {
        if (isMoveAction)
        {
            MoveDistanceHighLight(value, playerActions.transform.position);
        }
        else
        {
            PickUpThrowHighLight(value);
        }
        foreach (var tile in tiles)
        {
            if (tile.isHighLight) tile.SetColor(color);
            
           
        }
    }
   
    public void TurnOffHighlight()
    {
        if (tiles != null)
        {
            foreach (TileScript tile in tiles)
            {
                tile.isHighLight = false;
                tile.ResetColor();
            }
        }
    }
    

    public TileTypes CheckWhatNextTileIs(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < size.x &&
            pos.z >= 0 && pos.z < size.y)
        {
            if (grid[(int)pos.x, (int)pos.z] == null)
            {
                return TileTypes.None;
            }
            else
            {
                return grid[(int)pos.x, (int)pos.z].tileType;
            }
        }
        return GameActions.TileTypes.None;
    }

    private void StoreGrid()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Ground");
        if (allTiles != null)
        {
            tiles = new TileScript[allTiles.Length];

            for (int i = 0; i < allTiles.Length; i++)
                tiles[i] = allTiles[i].GetComponent<TileScript>();
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                foreach (var tile in tiles)
                {
                    if (new Vector3(i, 0, j) == tile.transform.position)
                    {
                        tile.tileType = GameActions.TileTypes.EmptyTile;
                        grid[i, j] = tile;
                    }
                }
            }
        }
    }

    private void CheckIfGround(int amount, Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < size.x &&
            pos.z >= 0 && pos.z < size.y)
        {
            if (grid[(int)pos.x, (int)pos.z] != null)
            {
                grid[(int)pos.x, (int)pos.z].isHighLight = true;
                MoveDistanceHighLight(amount - 1, grid[(int)pos.x, (int)pos.z].transform.position);
            }
        }
    }

    private void MoveDistanceHighLight(int amount, Vector3 start)
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

    private void PickUpThrowHighLight(int amount)
    {
        int distance;
        foreach (var tile in tiles)
        {
            distance = CalculateDistance(
                tile.transform.position, 
                playerActions.transform.position);
            if (distance == amount)
            {
                tile.isHighLight = true;
            }
        }
    }

    public bool isCompanionMoving()
    {
        if (gridManager.companion != null)
        {
            return companion.isMoving;
        }
        else
        {
            return false;
        }
    }
    public void GoalCheck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
            Debug.Log("GOAL");
    }

    public void KeyItemCheck()
    {
        if (!playerActions.hasItem)
        {
            playerActions.hasItem = true;
            keyItem.SetActive(false);
        }
    }

    public void NumberItemCheck()
    {
        pickUpNumber.SetActive(false);
        numberHUD.SetActive(true);
        playerActions.hasNumber = true;
    }

    public void UpdateTileType(Vector3 pos, GameActions.TileTypes type)
    {
        if (size.x > pos.x || size.y > pos.z)
        {
            if (grid[(int)pos.x, (int)pos.z] != null)
                grid[(int)pos.x, (int)pos.z].tileType = type;
            else
                Debug.Log("hole");
        }
        else
            Debug.Log("Set correct Grid size");
    }

    public void ResetTileType(GameActions.TileTypes type)
    {
        foreach (var tile in tiles)
        {
            if (tile.tileType == type)
            {
                tile.tileType = GameActions.TileTypes.EmptyTile;
            }
        }
    }
}
