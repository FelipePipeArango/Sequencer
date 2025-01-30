using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] GameObject Tile;
    TileScript[] tiles;
    private GameObject[] allTiles;
    private GameObject oneTiles;
    // Start is called before the first frame update
    void Start()
    {
        //CreateGrid();
        FindAllTiles();
        SetTileAt(new Vector2Int( 0, 0));
    }
    public void CreateGrid()
    {
        Vector3 position = new Vector3(0, 0, 0);
        oneTiles = Instantiate(Tile);
        oneTiles.transform.position = position;
    }
    public void FindAllTiles()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Ground");
        Debug.Log(allTiles.Length);
        tiles = new TileScript[allTiles.Length];

        for (int i = 0; )
        {
            

            Debug.Log(tile.transform.position);
        }
    }
    public void SetTileAt(Vector2Int position)
    {
        if (allTiles != null)
        {
            foreach (GameObject tile in allTiles)
            {
                Vector2Int Pos2d = new Vector2Int(Mathf.FloorToInt(tile.transform.position.x),
                                                 Mathf.FloorToInt(tile.transform.position.z));
                if (Pos2d == position)
                    tile.GetComponent<TileScript>().SetColor(Color.red);
                else
                    Debug.Log($"At {tile.transform.position} nothing there");
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
