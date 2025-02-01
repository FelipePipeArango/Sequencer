using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{

    public static GridGenerator Instance { get; private set; }
    [SerializeField] GameObject Tile;
    [SerializeField] TileScript[] tiles;
    private GameObject[] allTiles;
    private GameObject oneTiles;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Ensure only one instance exists
            return;
        }

        Instance = this;  // Assign the static instance
        DontDestroyOnLoad(gameObject);  // Optional: Prevent this object from being destroyed on scene changes
    
    //CreateGrid();
    allTiles = GameObject.FindGameObjectsWithTag("Ground");
        Debug.Log(allTiles.Length);
        tiles = new TileScript[allTiles.Length];

        for (int i = 0; i < allTiles.Length; i++)
        {
            tiles[i] = allTiles[i].GetComponent<TileScript>();
        }

        foreach (TileScript tile in tiles)
        {
            Debug.Log(tile.GetColor());
        }
        //SetTileAt(new Vector2Int( 0, 3));
    }
    public void CreateGrid()
    {
        Vector3 position = new Vector3(0, 0, 0);
        oneTiles = Instantiate(Tile);
        oneTiles.transform.position = position;
    }

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
                tile.SetColor(Color.red);
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

    public void SetTileAt(Vector2Int position)
    {
        if (allTiles != null)
        {
            foreach (TileScript tile in tiles)
            {
                Vector2Int Pos2d = new Vector2Int(Mathf.FloorToInt(tile.transform.position.x),
                                                 Mathf.FloorToInt(tile.transform.position.z));
                if (Pos2d == position)
                {
                   // Debug.Log($"At {tile.transform.position} Red");
                    tile.SetColor(Color.red);
                }
                //else
                   // Debug.Log($"At {tile.transform.position} nothing there");
            }
        }
        else
            Debug.Log("No tiles here");
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
