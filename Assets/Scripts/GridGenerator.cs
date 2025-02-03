using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    private GameObject[,] Tiles;
    public bool showGrid = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Ensure only one instance exists
            return;
        }

        Tiles = new GameObject[size.x, size.y];
        Instance = this;  // Assign the static instance
        //CreateGrid();
        // DontDestroyOnLoad(gameObject);  // Optional: Prevent this object from being destroyed on scene changes
    
       // CreateGrid();
        UpdateGrid();
        
    }

    
    
    //not implemented yet
    //public void CreateGrid()
    //{ 
    //    for (int i = 0; i < size.x; i++)
    //    {
    //        for (int j = 0; j < size.y; j++)
    //        {
    //            Vector3 position = new Vector3(i, 0, j);
    //            Tiles[i,j] = Instantiate(Tile);
    //            Tiles[i, j].transform.position = position;
    //        }
    //    }
    //}
    //
    
    public void DistanceCheck(int amount)
    {
        int distance;
        foreach (var tile in tiles)
        { 
            distance = GameManager.Instance.CalculateDistance(tile.transform.position);
            Debug.Log($"{distance}");
            if ((distance) <= amount)
            {
                Debug.Log($"{distance - amount}");
                tile.SetColor(color);
                if (distance == 0)
                {
                    tile.ResetColor();
                }
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

    //public void SetTileAt(Vector2Int position)
    //{
    //    if (allTiles != null)
    //    {
    //        foreach (TileScript tile in tiles)
    //        {
    //            Vector2Int Pos2d = new Vector2Int(Mathf.FloorToInt(tile.transform.position.x),
    //                                             Mathf.FloorToInt(tile.transform.position.z));
    //            if (Pos2d == position)
    //            {
    //               // Debug.Log($"At {tile.transform.position} Red");
    //                tile.SetColor(color);
    //            }
    //            //else
    //               // Debug.Log($"At {tile.transform.position} nothing there");
    //        }
    //    }
    //    else
    //        Debug.Log("No tiles here");
        
    //}

    // Update is called once per frame

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
    }

}
