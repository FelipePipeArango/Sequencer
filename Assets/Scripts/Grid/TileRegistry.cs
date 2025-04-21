// TileRegistry.cs
using UnityEngine;
using System.Collections.Generic;
using static GameTiles;

public class TileRegistry : MonoBehaviour
{
    public static TileRegistry Instance { get; private set; }

    [SerializeField] private GameObject tilePrefab;
    private Dictionary<Vector2Int, TileScript> grid = new();

    public Vector2Int GridSize { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void InitializeGrid(Vector2Int size)
    {
        ClearGrid();
        GridSize = size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int pos = new(x, y);
                CreateTile(TileTypes.EmptyTile, pos);
            }
        }
    }

    public TileScript CreateTile(TileTypes type, Vector2Int position)
    {
        if (grid.ContainsKey(position)) return grid[position];

        GameObject tileObj = Instantiate(tilePrefab, new Vector3(position.x, 0, position.y), Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f)));
        TileScript tileScript = tileObj.GetComponent<TileScript>();
        tileScript.tileType = type;

        grid[position] = tileScript;
        return tileScript;
    }

    public void RemoveTile(Vector2Int position)
    {
        if (!grid.ContainsKey(position)) return;

        Destroy(grid[position].gameObject);
        grid.Remove(position);
    }

    public TileScript GetTile(Vector2Int position)
    {
        grid.TryGetValue(position, out TileScript tile);
        return tile;
    }

    public void ClearGrid()
    {
        foreach (var tile in grid.Values)
        {
            if (tile != null) Destroy(tile.gameObject);
        }
        grid.Clear();
    }

    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < GridSize.x && pos.y < GridSize.y;
    }

    public IEnumerable<TileScript> AllTiles => grid.Values;
}
