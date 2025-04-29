using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private Transform gridOrigin;

    [SerializeField] private int width = 6, height = 6;
    [SerializeField] private float cellSize = 0.5f;

    private Dictionary<Vector2Int, GameObject> occupiedSlots = new();

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        Vector3 offset = new Vector3(
            -(width - 1) * 0.5f * cellSize,
            0,
            -(height - 1) * 0.5f * cellSize
        );
        return gridOrigin.position + offset + new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 offset = worldPos - gridOrigin.position;

        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(offset.x / cellSize + (width - 1) * 0.5f),
            Mathf.RoundToInt(offset.z / cellSize + (height - 1) * 0.5f)
        );
        return gridPos;
    }

    public bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public bool IsOccupied(Vector2Int gridPos)
    {
        return occupiedSlots.ContainsKey(gridPos);
    }

    public void Occupy(Vector2Int pos, GameObject tile)
    {
        occupiedSlots[pos] = tile;
    }

    public void Vacate(Vector2Int pos)
    {
        if (occupiedSlots.ContainsKey(pos))
            occupiedSlots.Remove(pos);
    }
}