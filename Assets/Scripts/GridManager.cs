
using UnityEngine;
using UnityEngine.SceneManagement;
using Color = UnityEngine.Color;
using static GameTiles;
using System.Collections;
using static UnityEditor.PlayerSettings;

public class GridManager : MonoBehaviour
{
    public static GridManager gridManager { get; private set; }

    [Header("BOARD")]
    [SerializeField] Color highlightColor;
    [SerializeField] public Vector2Int size;

    private GameObject[] allTiles;
    public TileScript[,] grid;
    TileScript[] tiles;

    [Header("MANDATORY PIECES IN A LEVEL")]
    public GameObject player;
    public GameObject keyItem;
    public GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")]
    public GameObject AICompanion;
    public GameObject pickUpNumber;
    public GameObject numberHUD; //Should at some point go to card manager
    [HideInInspector] public Player playerActions;
    [HideInInspector] public AICompanion AIActions;

    [Header("OBJECT POINTERS")]
    [SerializeField] float goalPointerHeight;
    [SerializeField] float keyItemPointerHeight;
    [SerializeField] float tileHeight;


    void Awake()
    {
        if (gridManager != null && gridManager != this)
        {
            Destroy(gameObject); 
            return;
        }

        gridManager = this;

        playerActions = player.GetComponent<Player>();
       
        if(AICompanion != null)
            AIActions = AICompanion.GetComponent<AICompanion>();  
        
        grid = new TileScript[size.x, size.y];
        StoreGrid();
    }

    private void Start()
    {
        UpdateTileType(keyItem.transform.position, TileTypes.KeyTile);
        if (pickUpNumber != null) 
        { 
            UpdateTileType(pickUpNumber.transform.position, TileTypes.ItemTile);
        }
        UpdateTileType(goal.transform.position, TileTypes.GoalTile);
    }

    public Vector3 GetTileMap()
    {
        return new Vector3(size.x, 1, size.y);   
    }
    public TileScript GetTile(Vector2Int pos)
    {
        if(size.x > pos.x && pos.x >= 0 
            && size.y > pos.y && pos.y >= 0)
        { 
            if (grid[pos.x, pos.y] != null)
                return grid[pos.x, pos.y];
            else
                return null;
        }
        else
            return null;
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
   
    public void TurnOnHighlight(GameActions.Actions action, int value)
    {
        switch (action)
        {
            case GameActions.Actions.Move:
                MoveDistanceHighLight(value, playerActions.transform.position);
                break;

            case GameActions.Actions.PickUp:
                PickUpHighLight(value);
                break;

            case GameActions.Actions.Throw:
                ThrowHighLight(value);
                break;

        }

        foreach (var tile in tiles)
        {
            if (tile.isHighLight) tile.SetColor(highlightColor);
        }
    }
   
    public void TurnOffHighlight()
    {
        if (tiles != null)
        {
            foreach (TileScript tile in tiles)
            {
                tile.isHighLight = false;
                if (tile.pointer.activeSelf)
                    tile.pointer.SetActive(false);
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
        return TileTypes.None;
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
                        tile.tileType = TileTypes.EmptyTile;
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

    private void PickUpHighLight(int amount)
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
                if (tile.tileType == TileTypes.KeyTile || tile.tileType == TileTypes.ItemTile)
                {
                    tile.pointer.transform.position = new Vector3 (tile.pointer.transform.position.x, keyItemPointerHeight, tile.pointer.transform.position.z);

                    tile.pointer.gameObject.SetActive(true);
                }
                if (tile.tileType == TileTypes.EmptyTile)
                {
                    //No PickUp Available
                }
            }
        }
    }

    private void ThrowHighLight(int amount)
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
                if (tile.tileType == TileTypes.EmptyTile)
                {
                    tile.pointer.transform.position = new Vector3(tile.pointer.transform.position.x, tileHeight, tile.pointer.transform.position.z);
                    tile.pointer.gameObject.SetActive(true);
                }
                else if (tile.tileType == TileTypes.GoalTile)
                {
                    tile.pointer.transform.position = new Vector3(tile.pointer.transform.position.x, goalPointerHeight, tile.pointer.transform.position.z);
                    tile.pointer.gameObject.SetActive(true);
                }
            }
        }
    }


    public void UpdateTileType(Vector3 pos, TileTypes type)
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

    public void ResetTileType(TileTypes type)
    {
        foreach (var tile in tiles)
        {
            if (tile.tileType == type)
            {
                tile.tileType = TileTypes.EmptyTile;
            }
        }
    }
    /*
    if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //if (hit.collider.gameObject != gridVFX && selectedTileType != TileTypes.None) return; // Only interact if it's the grid

                Vector3 hitPoint = hit.point;
    Vector3 localPos = gridVFX.transform.InverseTransformPoint(hitPoint);
    Vector2 gridCellSize = new Vector2(gridVFX.transform.localScale.x, gridVFX.transform.localScale.z);
    int x = Mathf.FloorToInt(localPos.x / gridCellSize.x);
    int z = Mathf.FloorToInt(localPos.z / gridCellSize.y);
    Vector2Int pos = new Vector2Int(x + 6, z + 6);

    //if (playerButton.interactable == false) playerPos = pos;
    //else if (aiButton.interactable == false) aiPos = pos;
    //else if (keyButton.interactable == false) keyPos = pos;
    //else if (goalButton.interactable == false) goalPos = pos;
    //else
    //{
    var tile = TileRegistry.Instance.GetTile(pos);

                    if (tile == null && selectedTileType != TileTypes.None)
                    {
                        TileRegistry.Instance.CreateTile(selectedTileType, pos);
                    }
                    else if (tile != null)
{
    if (selectedTileType == TileTypes.None)
    {
        TileRegistry.Instance.RemoveTile(pos);
    }
    else
    {
        TileRegistry.Instance.CreateTile(selectedTileType, pos);
    }
}
                //}
            }
        }
    }
    */
}
