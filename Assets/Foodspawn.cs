



using UnityEngine;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private SnakeController snakeController;

    [Header("Food Settings")]
    [SerializeField] private float foodSize = 0.85f;

    private GameObject currentFood;
    private Sprite appleSprite;

    private void Start()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (snakeController == null)
            snakeController = FindFirstObjectByType<SnakeController>();

        if (gridManager == null || snakeController == null)
        {
            Debug.LogError(
                "FoodSpawner: GridManager or SnakeController is missing!"
            );
            return;
        }

        appleSprite = CreateAppleSprite();
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (gridManager == null || snakeController == null)
        {
            Debug.LogError("FoodSpawner: References are missing!");
            return;
        }

        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }

        List<Vector2Int> availablePositions =
            new List<Vector2Int>();

        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);

                if (!snakeController.IsPositionOccupied(position))
                {
                    availablePositions.Add(position);
                }
            }
        }

        if (availablePositions.Count == 0)
        {
            Debug.Log("No available positions. You won!");
            return;
        }

        Vector2Int randomPosition = availablePositions[
            Random.Range(0, availablePositions.Count)
        ];

        currentFood = new GameObject("Food");

        currentFood.transform.position =
            gridManager.GridToWorld(randomPosition);

        float size = gridManager.cellSize * foodSize;

        currentFood.transform.localScale =
            new Vector3(size, size, 1f);

        SpriteRenderer spriteRenderer =
            currentFood.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = appleSprite;
        spriteRenderer.sortingOrder = 6;

        Food food = currentFood.AddComponent<Food>();
        food.Initialize(this, randomPosition);

        Debug.Log("Food spawned at: " + randomPosition);
    }

    private Sprite CreateAppleSprite()
    {
        int width = 16;
        int height = 16;

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(0, 0, 0, 0);

        Color red = new Color(0.85f, 0.08f, 0.12f);
        Color darkRed = new Color(0.55f, 0.03f, 0.08f);
        Color lightRed = new Color(1f, 0.28f, 0.25f);
        Color highlight = new Color(1f, 0.65f, 0.48f);

        Color brown = new Color(0.35f, 0.18f, 0.08f);
        Color green = new Color(0.25f, 0.65f, 0.18f);
        Color lightGreen = new Color(0.55f, 0.85f, 0.25f);

        Color[,] pixels = new Color[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pixels[x, y] = clear;
            }
        }

        // Apple body
        string[] apple =
        {
            "      XXXX      ",
            "     XXXXXX     ",
            "   XXXXXXXXXX   ",
            "  XXXXXXXXXXXX  ",
            " XXXXXXXXXXXXXX ",
            " XXXXXXXXXXXXXX ",
            " XXXXXXXXXXXXXX ",
            " XXXXXXXXXXXXXX ",
            "  XXXXXXXXXXXX  ",
            "   XXXXXXXXXX   ",
            "    XXXXXXXX    ",
            "     XXXXXX     "
        };

        for (int row = 0; row < apple.Length; row++)
        {
            int y = 11 - row;

            for (int x = 0; x < width; x++)
            {
                if (apple[row][x] == 'X')
                {
                    pixels[x, y] = red;

                    if (x < 3)
                        pixels[x, y] = darkRed;

                    if (x >= 12)
                        pixels[x, y] = lightRed;
                }
            }
        }

        // Apple highlight
        pixels[3, 9] = highlight;
        pixels[4, 9] = highlight;
        pixels[3, 8] = highlight;
        pixels[4, 8] = lightRed;

        // Stem
        pixels[8, 12] = brown;
        pixels[8, 13] = brown;
        pixels[9, 13] = brown;

        // Leaf
        pixels[9, 14] = green;
        pixels[10, 14] = green;
        pixels[11, 14] = lightGreen;
        pixels[10, 15] = green;
        pixels[11, 15] = lightGreen;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, pixels[x, y]);
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            16f
        );
    }
}
