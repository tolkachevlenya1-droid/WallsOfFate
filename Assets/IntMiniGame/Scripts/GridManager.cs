using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public GridCell[] cells;

    private GridCell[,] grid;

    void Awake()
    {
        grid = new GridCell[width, height];

        foreach (var cell in cells)
        {
            grid[cell.GridPosition.x, cell.GridPosition.y] = cell;
        }
    }

    public GridCell GetCell(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= width || pos.y >= height)
            return null;

        return grid[pos.x, pos.y];
    }
}