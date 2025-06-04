using UnityEngine;
using Color = UnityEngine.Color;
using static GameTiles;

public class GridManager : MonoBehaviour
{
    public static GridManager gridManager { get; private set; }

    [Header("BOARD")]
    [SerializeField] Color highlightColor;
    [SerializeField] public Vector2Int size;

    [HideInInspector] public GameObject tileMap;
    private GameObject[] allTiles;
    public TileScript[,] grid;
    TileScript[] tiles;
    private bool lastPickUpHighlightSucceeded = false;

    [Header("MANDATORY PIECES IN A LEVEL")]
    public GameObject player;
    public GameObject keyItem;
    public GameObject goal;

    [Header("OPTIONAL OBJECTS IN A LEVEL")]
    public GameObject AICompanion;
    public GameObject numberPickUp;
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
        
        tileMap = new GameObject();
        tileMap.transform.position = new Vector3(size.x, 1, size.y);

        StoreGrid();
    }

    private void Start()
    {
        UpdateTileType(keyItem.transform.position, TileTypes.KeyTile);
        if (numberPickUp != null) 
        { 
            UpdateTileType(numberPickUp.transform.position, TileTypes.ItemTile);
        }
        UpdateTileType(goal.transform.position, TileTypes.GoalTile);
    }

    public TileScript ClickedTile(float offsetZ)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            Vector3 localPos = gridManager.tileMap.transform.InverseTransformPoint(hitPoint);
            Vector2 gridCellSize = new Vector2(
                gridManager.tileMap.transform.localScale.x,
                gridManager.tileMap.transform.localScale.z
                );
            int x = Mathf.FloorToInt(localPos.x / gridCellSize.x + 0.5f);
            int z = Mathf.FloorToInt(localPos.z / gridCellSize.y - offsetZ);

            Vector2Int pos = new Vector2Int(
                x + gridManager.size.x,
                z + gridManager.size.y + 1
                );
            Debug.Log( pos.ToString() );
            if (size.x > pos.x && pos.x >= 0
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

            case GameActions.Actions.Pick_Up:
                PickUpHighLight(value);
                break;

            case GameActions.Actions.Throw:
                ThrowHighLight(value);
                break;

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

    public void PickUpHighLight(int amount)
    {
        int distance;
        bool found = false;

        foreach (var tile in tiles)
        {
            distance = CalculateDistance(
                tile.transform.position,
                playerActions.transform.position);
            if (distance == amount)
            {
                if (tile.tileType == TileTypes.KeyTile ||
                    tile.tileType == TileTypes.ItemTile)
                {
                    HighlightTile(tile);
                    found = true;
                }
            }
        }
        lastPickUpHighlightSucceeded = found;
    }

    public bool WasPickUpHighlightSuccessful()
    {
        return lastPickUpHighlightSucceeded;
    }

    private void HighlightTile(TileScript tile)
    {
        tile.pointer.transform.position = new Vector3(
                        tile.pointer.transform.position.x,
                        2.4f,
                        tile.pointer.transform.position.z);

        tile.pointer.gameObject.SetActive(true);

        tile.isHighLight = true;

        tile.SetColor(highlightColor);
    }

    public void ThrowHighLight(int amount)
    {
        int distance;
        foreach (var tile in tiles)
        {
            distance = CalculateDistance(
                tile.transform.position,
                playerActions.transform.position);
            if (distance == amount)
            {
                if (tile.tileType == TileTypes.EmptyTile)
                {
                    HighlightTile(tile);
                }
                else if (tile.tileType == TileTypes.GoalTile)
                {
                    HighlightTile(tile);
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
}
