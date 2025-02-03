using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] public int unitGridsize;

    Vector2Int[] battlefield;
    int gridTotal = 0;

    public void Awake()
    {
        gridTotal = gridSize.x * gridSize.y;
        int i = 0;
        battlefield = new Vector2Int[gridTotal];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int cords = new Vector2Int(x, y);
                battlefield[i] = cords;
                i++;
            }
        }
    }
}
 