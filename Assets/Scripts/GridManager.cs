using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

    public int distaceToItem, distaceToGoal, distaceToNumber;
    public int AIdistaceToItem, AIdistaceToGoal, AIdistaceToNumber;

    [Header("Pieces")] [SerializeField] 
    public GameObject AICompanion;
    public GameObject player;
    public GameObject keyItem;
    public GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")] [SerializeField]
    public GameObject pickUpNumber;
    public UnitControler playerActions;
    public AICompanion AIActions;


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
        AIActions = AICompanion.GetComponent<AICompanion>();  
        grid = new TileScript[size.x, size.y];
        StoreGrid();
    }

    private void Start()
    {
        
    }

    void StoreGrid()
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
                        grid[i, j] = tile;
                }
            }
        }

        updateTileType(playerActions.transform.position, GameActions.TileTypes.PlayerTile);
        updateTileType(keyItem.transform.position, GameActions.TileTypes.KeyTile);
        updateTileType(pickUpNumber.transform.position, GameActions.TileTypes.ItemTile);
        updateTileType(goal.transform.position, GameActions.TileTypes.GoalTile);
    }

    public int CalculateDistance(Vector3 position,
        Vector3 start)
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
            distance = CalculateDistance(
                tile.transform.position, 
                playerActions.transform.position);
            if (distance == amount)
            {
                tile.SetColor(color);
            }
        }
    }
    
    public void GoalCheck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        distaceToGoal = CalculateDistance(
            goal.transform.position, 
            playerActions.transform.position);

        if (distaceToGoal == 0 && playerActions.hasItem)
        {
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
    public void KeyItemCheck()
    {

        if (!playerActions.hasItem)
        {
            distaceToItem = CalculateDistance(
                keyItem.transform.position,
                playerActions.transform.position);

            if (distaceToItem == 0)
            {
                Vector2Int currentPosition = PosConverter(player.transform.position);
                playerActions.hasItem = true;
                keyItem.SetActive(false);
            }
        }
    }
    public void NumberItemCheck()
    {
        if (pickUpNumber != null)
        {
            distaceToNumber = CalculateDistance(
                pickUpNumber.transform.position,
                playerActions.transform.position);
            AIdistaceToNumber = CalculateDistance(
                pickUpNumber.transform.position,
                playerActions.transform.position);
            if (distaceToNumber == 0 && !playerActions.hasNumber)
            {
                Vector2Int currentPosition = PosConverter(player.transform.position);

                pickUpNumber.SetActive(false);
                GameManager.Instance.numberHUD.SetActive(true);
                playerActions.hasNumber = true;
            }
        }
    }
    public void PickUpCheck(GameActions.Actions usedAction, GameObject affected)
    {
        if (usedAction == GameActions.Actions.PickUp && affected != null)
        {
            affected.SetActive(false);
        }
    }
    
    public Vector3 GetPlayerPos()
    {
        return playerActions.transform.position;
    }

    public void updateTileType(Vector3 pos, GameActions.TileTypes type)
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

}