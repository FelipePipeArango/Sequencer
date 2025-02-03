using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;

public class GridGenerator : MonoBehaviour
{

    public static GridGenerator Instance { get; private set; }
    [SerializeField] GameObject Tile;
    [SerializeField] Color color;

    [Header("Not Implemented yet")] 
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PickUpItem;
    [SerializeField] GameObject KeyItem;
    [SerializeField] TileScript[] tiles;
    [SerializeField] Vector2Int size;
    private GameObject[] allTiles;
    TileScript[,] TwoDTiles;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this; // Assign the static instance

        CreateGrid();
        UpdateGrid();
        
    }

    //not implemented yet
    public void CreateGrid()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector3 position = new Vector3(i, 0, j);
                var spawnedTile = Instantiate(Tile, position, Quaternion.identity);
                spawnedTile.name = $"Tile {i}, {j}";
            }
        }

    }

    void CheckIfGround(int amount, Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < GameManager.Instance.size.x && 
            pos.z >= 0 && pos.z < GameManager.Instance.size.y)
        {
            if (TwoDTiles[(int)pos.x, (int)pos.z] != null)
            {
                TwoDTiles[(int)pos.x, (int)pos.z].SetColor(color);
                MoveDistanceCheck(amount - 1, TwoDTiles[(int)pos.x, (int)pos.z].transform.position);
            }
        }
    }

    public void MoveDistanceCheck(int amount, Vector3 start)
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

    public void PickUpThrowCheck(int amount)
    {
        int distance;
        foreach (var tile in tiles)
        {
            distance = GameManager.Instance.CalculateDistance(tile.transform.position);
            Debug.Log($"{distance}");
            if (distance == amount)
            {
                tile.SetColor(color);
            }

        }
    }

    public void Reset()
    {
        if (allTiles != null)
        {
            foreach (TileScript tile in tiles)
            {
                tile.ResetColor();
            }
        }
        else
            Debug.Log("No tiles here");

    }

    void UpdateGrid()
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

        TileScript[,] placeholder = new TileScript[GameManager.Instance.size.x, GameManager.Instance.size.y];

        for (int i = 0; i < GameManager.Instance.size.x ; i++)
        {
            for (int j = 0; j < GameManager.Instance.size.y; j++)
            {
                foreach (var tile in tiles)
                {
                    if (new Vector3(i, 0, j) == tile.transform.position)
                    {
                        placeholder[i, j] = tile;
                    }
                }
            }
        }

        TwoDTiles = new TileScript[GameManager.Instance.size.x, GameManager.Instance.size.y];
        TwoDTiles = placeholder;
        foreach (var tile in TwoDTiles)
        {
            if (tile != null)
                Debug.Log($"Has Tile at {tile.transform.position}");
            else
            {
                Debug.Log("Nothing");
            }
        }
    }
}
