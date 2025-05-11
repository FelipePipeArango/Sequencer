// LevelEditorUI.cs
using static GameTiles;
// LevelEditorUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class LevelEditorUI : MonoBehaviour
{
    [SerializeField] private TileTypes selectedTileType = TileTypes.EmptyTile;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button playerButton;
    [SerializeField] private Button aiButton;
    [SerializeField] private Button keyButton;
    [SerializeField] private Button goalButton;
    [SerializeField] private Vector2Int gridSize = new(3, 3);
    [SerializeField] private GameObject gridVFX; // Reference to GridVFX object


    private Vector2Int? playerPos = null;
    private Vector2Int? aiPos = null;
    private Vector2Int? keyPos = null;
    private Vector2Int? goalPos = null;

    private string savePath;

    private void Start()
    {
        //saveButton.onClick.AddListener(SaveLevel);
        //loadButton.onClick.AddListener(LoadLevel);
        //savePath = Path.Combine(Application.persistentDataPath, "level.json");
        // Generate visual placeholder grid for editing
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int pos = new(x, y);
                TileRegistry.Instance.CreateTile(TileTypes.None, pos); // Placeholder tiles
            }
        }
    }
    private void Update()
    {
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


    private void SaveLevel()
    {
        LevelData data = new LevelData();
        data.tiles = new List<TileEntry>();

        foreach (var tile in TileRegistry.Instance.AllTiles)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.z));
            data.tiles.Add(new TileEntry(pos, tile.tileType));
        }

        data.gridSize = gridSize;
        data.playerPosition = playerPos ?? Vector2Int.zero;
        data.aiPosition = aiPos ?? Vector2Int.zero;
        data.keyItemPosition = keyPos ?? Vector2Int.zero;
        data.goalPosition = goalPos ?? Vector2Int.zero;

        string json = JsonUtility.ToJson(data, true);
        Debug.Log("Level saved:\n" + json);
    }

    public void SetSelectedTileType(int tileTypeInt)
    {
        selectedTileType = (TileTypes)tileTypeInt;
        EnableAllPlacementButtons();
    }

    public void SetPlacingPlayer() => TogglePlacement(playerButton);
    public void SetPlacingAI() => TogglePlacement(aiButton);
    public void SetPlacingKey() => TogglePlacement(keyButton);
    public void SetPlacingGoal() => TogglePlacement(goalButton);

    private void TogglePlacement(Button targetButton)
    {
        EnableAllPlacementButtons();
        targetButton.interactable = false;
    }

    private void EnableAllPlacementButtons()
    {
        playerButton.interactable = true;
        aiButton.interactable = true;
        keyButton.interactable = true;
        goalButton.interactable = true;
    }
}


