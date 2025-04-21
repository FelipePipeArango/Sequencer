// LevelLoader.cs
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    public void LoadLevel(LevelData data)
    {
        if (TileRegistry.Instance == null)
        {
            Debug.LogError("TileRegistry not found in scene");
            return;
        }

        TileRegistry.Instance.InitializeGrid(data.gridSize);

        foreach (TileEntry entry in data.tiles)
        {
            TileRegistry.Instance.CreateTile(entry.tileType, entry.position);
        }

        SpawnPlayer(data.playerPosition);
        SpawnAI(data.aiPosition);
        SpawnKeyItem(data.keyItemPosition);
        SpawnGoal(data.goalPosition);
    }

    private void SpawnPlayer(Vector2Int pos)
    {
        // Assume prefab is handled elsewhere
        GameObject player = Instantiate(Resources.Load<GameObject>("Player"), new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }

    private void SpawnAI(Vector2Int pos)
    {
        GameObject ai = Instantiate(Resources.Load<GameObject>("AICompanion"), new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }

    private void SpawnKeyItem(Vector2Int pos)
    {
        GameObject key = Instantiate(Resources.Load<GameObject>("KeyItem"), new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }

    private void SpawnGoal(Vector2Int pos)
    {
        GameObject goal = Instantiate(Resources.Load<GameObject>("Goal"), new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }
}
