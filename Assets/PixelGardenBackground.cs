
using UnityEngine;

public class PixelGardenBackground : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Pixel Settings")]
    [SerializeField] private int textureWidth = 320;
    [SerializeField] private int textureHeight = 180;

    private Texture2D gardenTexture;
    private Sprite gardenSprite;
    private GameObject backgroundObject;

    // Rich garden palette
    private readonly Color deepGreen = new Color(0.19f, 0.38f, 0.22f);
    private readonly Color darkGreen = new Color(0.12f, 0.27f, 0.16f);
    private readonly Color forestGreen = new Color(0.27f, 0.49f, 0.27f);
    private readonly Color grassGreen = new Color(0.38f, 0.61f, 0.32f);
    private readonly Color lightGreen = new Color(0.52f, 0.70f, 0.38f);
    private readonly Color limeGreen = new Color(0.67f, 0.76f, 0.40f);
    private readonly Color paleGreen = new Color(0.76f, 0.81f, 0.48f);

    private readonly Color darkLeaf = new Color(0.16f, 0.32f, 0.20f);
    private readonly Color leafGreen = new Color(0.31f, 0.53f, 0.29f);
    private readonly Color yellowGreen = new Color(0.77f, 0.78f, 0.40f);

    private readonly Color flowerPink = new Color(0.96f, 0.59f, 0.64f);
    private readonly Color flowerCream = new Color(1.00f, 0.88f, 0.63f);
    private readonly Color flowerPurple = new Color(0.70f, 0.58f, 0.82f);
    private readonly Color flowerWhite = new Color(0.95f, 0.92f, 0.78f);

    private readonly Color stoneColor = new Color(0.57f, 0.61f, 0.49f);
    private readonly Color darkStone = new Color(0.39f, 0.46f, 0.37f);

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("PixelGardenBackground: Camera not found!");
            return;
        }

        CreateGarden();
    }

    private void CreateGarden()
    {
        gardenTexture = new Texture2D(
            textureWidth,
            textureHeight,
            TextureFormat.RGBA32,
            false
        );

        gardenTexture.filterMode = FilterMode.Point;
        gardenTexture.wrapMode = TextureWrapMode.Clamp;

        DrawBaseGrass();
        DrawGrassPatches();
        DrawGrassDetails();
        DrawBushes();
        DrawFlowers();
        DrawStones();
        DrawLightDetails();

        gardenTexture.Apply();

        gardenSprite = Sprite.Create(
            gardenTexture,
            new Rect(0, 0, textureWidth, textureHeight),
            new Vector2(0.5f, 0.5f),
            32
        );

        backgroundObject = new GameObject("Pixel Garden Background");
        backgroundObject.transform.SetParent(transform, false);

        backgroundObject.transform.position =
            mainCamera.transform.position + mainCamera.transform.forward * 10f;

        SpriteRenderer renderer =
            backgroundObject.AddComponent<SpriteRenderer>();

        renderer.sprite = gardenSprite;
        renderer.sortingOrder = -100;

        FitToCamera();
    }

    private void DrawBaseGrass()
    {
        Color[] pixels = new Color[textureWidth * textureHeight];

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float variation = Mathf.PerlinNoise(x * 0.035f, y * 0.035f);

                Color color = Color.Lerp(
                    forestGreen,
                    grassGreen,
                    variation
                );

                pixels[y * textureWidth + x] = color;
            }
        }

        gardenTexture.SetPixels(pixels);
    }

    private void DrawGrassPatches()
    {
        Random.InitState(120);

        for (int i = 0; i < 95; i++)
        {
            int x = Random.Range(0, textureWidth);
            int y = Random.Range(0, textureHeight);

            int width = Random.Range(3, 12);
            int height = Random.Range(2, 7);

            Color color = Random.value > 0.5f
                ? lightGreen
                : forestGreen;

            // Keep the central gameplay area relatively calm.
            if (IsNearCenter(x, y))
                continue;

            DrawRect(x, y, width, height, color);
        }
    }

    private void DrawGrassDetails()
    {
        Random.InitState(245);

        for (int i = 0; i < 850; i++)
        {
            int x = Random.Range(0, textureWidth);
            int y = Random.Range(0, textureHeight);

            if (IsNearCenter(x, y))
                continue;

            Color color;

            int choice = Random.Range(0, 4);

            if (choice == 0)
                color = paleGreen;
            else if (choice == 1)
                color = limeGreen;
            else if (choice == 2)
                color = deepGreen;
            else
                color = lightGreen;

            DrawRect(x, y, Random.Range(2, 5), Random.Range(2, 5), color);
        }

        // Tiny grass blades
        for (int i = 0; i < 180; i++)
        {
            int x = Random.Range(0, textureWidth);
            int y = Random.Range(0, textureHeight);

            if (IsNearCenter(x, y))
                continue;

            DrawRect(x, y, 1, 3, yellowGreen);
            DrawRect(x + 1, y + 1, 2, 1, grassGreen);
        }
    }

    private void DrawBushes()
    {
        // Bushes around the edges
        DrawBush(18, 22, 24);
        DrawBush(65, 155, 20);
        DrawBush(145, 15, 22);
        DrawBush(250, 145, 25);
        DrawBush(295, 55, 20);
        DrawBush(275, 15, 18);
        DrawBush(35, 115, 18);
    }

    private void DrawBush(int x, int y, int size)
    {
        DrawRect(x, y, size, size / 2, darkLeaf);
        DrawRect(x + 3, y + 4, size - 6, size / 2, forestGreen);

        DrawRect(x + 5, y + 8, size / 3, size / 3, leafGreen);
        DrawRect(x + size / 2, y + 10, size / 3, size / 3, grassGreen);
        DrawRect(x + size / 3, y + 13, 4, 3, lightGreen);

        DrawRect(x + 2, y + 2, 3, 3, darkGreen);
    }

    private void DrawFlowers()
    {
        // Flowers are placed mostly around the outer garden.
        DrawFlower(22, 65, flowerPink);
        DrawFlower(48, 145, flowerCream);
        DrawFlower(90, 20, flowerPurple);
        DrawFlower(130, 155, flowerWhite);
        DrawFlower(195, 18, flowerPink);
        DrawFlower(235, 150, flowerCream);
        DrawFlower(290, 105, flowerPurple);
        DrawFlower(275, 35, flowerWhite);
        DrawFlower(12, 135, flowerPink);
        DrawFlower(305, 25, flowerCream);
    }

    private void DrawFlower(int x, int y, Color petal)
    {
        Color center = new Color(1f, 0.78f, 0.28f);

        DrawRect(x + 2, y, 3, 3, petal);
        DrawRect(x, y + 2, 3, 3, petal);
        DrawRect(x + 4, y + 2, 3, 3, petal);
        DrawRect(x + 2, y + 4, 3, 3, petal);

        DrawRect(x + 2, y + 2, 3, 3, center);

        // Small stem and leaf
        DrawRect(x + 3, y - 5, 1, 5, darkLeaf);
        DrawRect(x + 4, y - 3, 3, 2, leafGreen);
    }

    private void DrawStones()
    {
        DrawStone(45, 35);
        DrawStone(110, 165);
        DrawStone(210, 30);
        DrawStone(285, 135);
        DrawStone(155, 45);
        DrawStone(15, 95);
    }

    private void DrawStone(int x, int y)
    {
        DrawRect(x + 2, y, 7, 4, darkStone);
        DrawRect(x, y + 2, 11, 5, stoneColor);
        DrawRect(x + 2, y + 5, 6, 2, lightGreen);
        DrawRect(x + 3, y + 4, 2, 1, paleGreen);
    }

    private void DrawLightDetails()
    {
        Random.InitState(765);

        for (int i = 0; i < 65; i++)
        {
            int x = Random.Range(0, textureWidth);
            int y = Random.Range(0, textureHeight);

            if (IsNearCenter(x, y))
                continue;

            DrawRect(x, y, 2, 2, flowerCream);
        }

        // Small decorative leaf clusters
        for (int i = 0; i < 25; i++)
        {
            int x = Random.Range(0, textureWidth);
            int y = Random.Range(0, textureHeight);

            if (IsNearCenter(x, y))
                continue;

            DrawRect(x, y, 4, 2, darkLeaf);
            DrawRect(x + 2, y + 2, 3, 2, leafGreen);
        }
    }

    private bool IsNearCenter(int x, int y)
    {
        // Keep the middle area clear for the snake and food.
        return x > textureWidth * 0.22f &&
               x < textureWidth * 0.78f &&
               y > textureHeight * 0.22f &&
               y < textureHeight * 0.78f;
    }

    private void DrawRect(int x, int y, int width, int height, Color color)
    {
        for (int px = x; px < x + width; px++)
        {
            for (int py = y; py < y + height; py++)
            {
                if (px >= 0 && px < textureWidth &&
                    py >= 0 && py < textureHeight)
                {
                    gardenTexture.SetPixel(px, py, color);
                }
            }
        }
    }

    private void FitToCamera()
    {
        if (mainCamera == null || gardenSprite == null)
            return;

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;

        Vector2 spriteSize = gardenSprite.bounds.size;

        backgroundObject.transform.localScale = new Vector3(
            width / spriteSize.x,
            height / spriteSize.y,
            1f
        );
    }

    private void OnDestroy()
    {
        if (backgroundObject != null)
            Destroy(backgroundObject);

        if (gardenSprite != null)
            Destroy(gardenSprite);

        if (gardenTexture != null)
            Destroy(gardenTexture);
    }
}
