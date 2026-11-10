

using UnityEngine;

public class ArenaBoundary : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;

    [Header("Pixel Border Colors")]
    [SerializeField] private Color outerColor =
        new Color(0.12f, 0.28f, 0.17f, 1f);

    [SerializeField] private Color innerColor =
        new Color(0.88f, 0.82f, 0.61f, 1f);

    [Header("Border Settings")]
    [SerializeField] private float outerThickness = 0.22f;
    [SerializeField] private float innerThickness = 0.07f;
    [SerializeField] private float innerOffset = 0.12f;

    private Sprite squareSprite;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = FindAnyObjectByType<GridManager>();

        if (gridManager == null)
        {
            Debug.LogError("ArenaBoundary: GridManager not found!");
            return;
        }

        squareSprite = CreateSquareSprite();
        CreateBoundary();
    }

    private void CreateBoundary()
    {
        float width = gridManager.width * gridManager.cellSize;
        float height = gridManager.height * gridManager.cellSize;

        Vector3 center = Vector3.zero;

        // Outer dark frame
        CreateLine(
            "Top Outer Border",
            center + new Vector3(0, height / 2f + outerThickness / 2f, 0),
            new Vector2(width + outerThickness * 2f, outerThickness),
            outerColor, 3
        );

        CreateLine(
            "Bottom Outer Border",
            center + new Vector3(0, -height / 2f - outerThickness / 2f, 0),
            new Vector2(width + outerThickness * 2f, outerThickness),
            outerColor, 3
        );

        CreateLine(
            "Left Outer Border",
            center + new Vector3(-width / 2f - outerThickness / 2f, 0, 0),
            new Vector2(outerThickness, height),
            outerColor, 3
        );

        CreateLine(
            "Right Outer Border",
            center + new Vector3(width / 2f + outerThickness / 2f, 0, 0),
            new Vector2(outerThickness, height),
            outerColor, 3
        );

        // Inner cream accent
        CreateLine(
            "Top Inner Accent",
            center + new Vector3(0, height / 2f - innerOffset, 0),
            new Vector2(width, innerThickness),
            innerColor, 4
        );

        CreateLine(
            "Bottom Inner Accent",
            center + new Vector3(0, -height / 2f + innerOffset, 0),
            new Vector2(width, innerThickness),
            innerColor, 4
        );

        CreateLine(
            "Left Inner Accent",
            center + new Vector3(-width / 2f + innerOffset, 0, 0),
            new Vector2(innerThickness, height),
            innerColor, 4
        );

        CreateLine(
            "Right Inner Accent",
            center + new Vector3(width / 2f - innerOffset, 0, 0),
            new Vector2(innerThickness, height),
            innerColor, 4
        );
    }

    private void CreateLine(
        string objectName,
        Vector3 position,
        Vector2 size,
        Color color,
        int sortingOrder
    )
    {
        GameObject boundary = new GameObject(objectName);

        boundary.transform.SetParent(transform);
        boundary.transform.position = position;

        SpriteRenderer renderer =
            boundary.AddComponent<SpriteRenderer>();

        renderer.sprite = squareSprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        boundary.transform.localScale =
            new Vector3(size.x, size.y, 1f);
    }

    private Sprite CreateSquareSprite()
    {
        return Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }

    private void OnDrawGizmos()
    {
        if (gridManager == null)
            gridManager = FindAnyObjectByType<GridManager>();

        if (gridManager == null)
            return;

        float width = gridManager.width * gridManager.cellSize;
        float height = gridManager.height * gridManager.cellSize;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.7f);

        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(width, height, 0f)
        );
    }
}
