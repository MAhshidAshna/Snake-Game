
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 20;
    public int height = 12;
    public float cellSize = 1f;

    public Vector2Int CellCount =>
        new Vector2Int(width, height);

    public Vector2Int CenterPosition =>
        new Vector2Int(width / 2, height / 2);

    public Vector3 ArenaBottomLeftWorldPos =>
        new Vector3(
            -width * cellSize / 2f + cellSize / 2f,
            -height * cellSize / 2f + cellSize / 2f,
            0f
        );

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(
            (worldPosition.x + width * cellSize / 2f) / cellSize - 0.5f
        );

        int y = Mathf.RoundToInt(
            (worldPosition.y + height * cellSize / 2f) / cellSize - 0.5f
        );

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = gridPosition.x * cellSize
                  - width * cellSize / 2f
                  + cellSize / 2f;

        float y = gridPosition.y * cellSize
                  - height * cellSize / 2f
                  + cellSize / 2f;

        return new Vector3(x, y, 0f);
    }

    public bool IsWithinBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.x < width &&
               gridPosition.y >= 0 &&
               gridPosition.y < height;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.5f, 0.8f, 0.3f);

        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(width * cellSize, height * cellSize, 0f)
        );

        Gizmos.color = new Color(0.2f, 0.5f, 0.8f, 0.1f);

        for (int x = 0; x <= width; x++)
        {
            float worldX = x * cellSize - width * cellSize / 2f;

            Vector3 start = new Vector3(
                worldX,
                -height * cellSize / 2f,
                0f
            );

            Vector3 end = new Vector3(
                worldX,
                height * cellSize / 2f,
                0f
            );

            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= height; y++)
        {
            float worldY = y * cellSize - height * cellSize / 2f;

            Vector3 start = new Vector3(
                -width * cellSize / 2f,
                worldY,
                0f
            );

            Vector3 end = new Vector3(
                width * cellSize / 2f,
                worldY,
                0f
            );

            Gizmos.DrawLine(start, end);
        }
    }
}
