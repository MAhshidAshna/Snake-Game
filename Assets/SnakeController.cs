

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementInterval = 0.15f;

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private FoodSpawner foodSpawner;

    [Header("Snake Settings")]
    [SerializeField] public int startingLength = 3;
    [SerializeField] private float segmentSize = 0.9f;

    [Header("Snake Colors")]
    [SerializeField] private Color headColor = new Color(0.25f, 0.72f, 0.95f);
    [SerializeField] private Color bodyColor = new Color(0.12f, 0.48f, 0.78f);
    [SerializeField] private Color eyeColor = Color.white;
    [SerializeField] private Color pupilColor = new Color(0.05f, 0.12f, 0.25f);

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Direction currentDirection = Direction.Right;
    public Direction queuedDirection = Direction.Right;

    public bool isGameOver = false;
    public bool isPaused = false;

    private readonly List<Vector2Int> positions = new List<Vector2Int>();
    private readonly List<GameObject> segments = new List<GameObject>();

    private Coroutine moveRoutine;
    private GameManager gameManager;
    private bool hasStarted = false;

    private Sprite squareSprite;

    public bool IsPositionOccupied(Vector2Int position)
    {
        return positions.Contains(position);
    }

    private void Awake()
    {
        if (gridManager == null)
            gridManager = FindAnyObjectByType<GridManager>();

        if (foodSpawner == null)
            foodSpawner = FindAnyObjectByType<FoodSpawner>();

        gameManager = FindAnyObjectByType<GameManager>();

        if (gridManager == null)
        {
            Debug.LogError("SnakeController: GridManager not found!");
            enabled = false;
            return;
        }

        squareSprite = CreateSquareSprite();

        InitializeSnake();
    }

    private void Update()
    {
        HandleInput();
    }

    public void InitializeSnake()
    {
        StopMovement();
        ClearSnake();

        positions.Clear();
        segments.Clear();

        Vector2Int center = gridManager.CenterPosition;

        currentDirection = Direction.Right;
        queuedDirection = Direction.Right;

        isGameOver = false;
        isPaused = false;
        hasStarted = false;

        for (int i = 0; i < startingLength; i++)
        {
            Vector2Int position = center + Vector2Int.left * i;

            positions.Add(position);

            GameObject segment = CreateSegment(i == 0);
            segment.transform.position = gridManager.GridToWorld(position);

            segments.Add(segment);
        }

        UpdateHeadEyes();

        Debug.Log("Snake initialized. Length: " + positions.Count);
    }

    private GameObject CreateSegment(bool isHead)
    {
        GameObject segment = new GameObject(
            isHead ? "SnakeHead" : "SnakeBody"
        );

        segment.transform.SetParent(transform);

        SpriteRenderer renderer = segment.AddComponent<SpriteRenderer>();

        renderer.sprite = squareSprite;
        renderer.color = isHead ? headColor : bodyColor;
        renderer.sortingOrder = 5;

        float size = gridManager.cellSize * segmentSize;

        segment.transform.localScale = new Vector3(size, size, 1f);

        if (isHead)
            CreateEyes(segment.transform);

        return segment;
    }

    private void CreateEyes(Transform head)
    {
        CreateEye(head, "LeftEye");
        CreateEye(head, "RightEye");
    }

    private void CreateEye(Transform head, string eyeName)
    {
        GameObject eye = new GameObject(eyeName);
        eye.transform.SetParent(head, false);

        SpriteRenderer eyeRenderer = eye.AddComponent<SpriteRenderer>();
        eyeRenderer.sprite = squareSprite;
        eyeRenderer.color = eyeColor;
        eyeRenderer.sortingOrder = 7;

        eye.transform.localScale = new Vector3(0.24f, 0.24f, 1f);

        GameObject pupil = new GameObject("Pupil");
        pupil.transform.SetParent(eye.transform, false);

        SpriteRenderer pupilRenderer = pupil.AddComponent<SpriteRenderer>();
        pupilRenderer.sprite = squareSprite;
        pupilRenderer.color = pupilColor;
        pupilRenderer.sortingOrder = 8;

        pupil.transform.localPosition = new Vector3(0.12f, 0f, 0f);
        pupil.transform.localScale = new Vector3(0.55f, 0.55f, 1f);
    }

    private void UpdateHeadEyes()
    {
        if (segments.Count == 0 || segments[0] == null)
            return;

        Transform head = segments[0].transform;

        Transform leftEye = head.Find("LeftEye");
        Transform rightEye = head.Find("RightEye");

        if (leftEye == null || rightEye == null)
            return;

        Vector2 forward = Vector2.right;
        Vector2 side = Vector2.up;

        switch (currentDirection)
        {
            case Direction.Up:
                forward = Vector2.up;
                side = Vector2.left;
                break;

            case Direction.Down:
                forward = Vector2.down;
                side = Vector2.right;
                break;

            case Direction.Left:
                forward = Vector2.left;
                side = Vector2.down;
                break;

            case Direction.Right:
                forward = Vector2.right;
                side = Vector2.up;
                break;
        }

        Vector2 eyeCenter = forward * 0.27f;
        Vector2 eyeOffset = side * 0.18f;

        leftEye.localPosition = eyeCenter + eyeOffset;
        rightEye.localPosition = eyeCenter - eyeOffset;
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

    public void ClearSnake()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    private void HandleInput()
    {
        if (isGameOver || isPaused)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W))
            SetDirection(Direction.Up);

        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Input.GetKeyDown(KeyCode.S))
            SetDirection(Direction.Down);

        else if (Input.GetKeyDown(KeyCode.LeftArrow) ||
                 Input.GetKeyDown(KeyCode.A))
            SetDirection(Direction.Left);

        else if (Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.D))
            SetDirection(Direction.Right);
    }

    public void SetDirection(Direction newDirection)
    {
        if (isGameOver || isPaused)
            return;

        if (IsOpposite(currentDirection, newDirection))
            return;

        queuedDirection = newDirection;

        if (!hasStarted)
        {
            hasStarted = true;
            StartMovementRoutine();

            gameManager?.SetPlaying();
        }
    }

    private bool IsOpposite(Direction a, Direction b)
    {
        return
            (a == Direction.Up && b == Direction.Down) ||
            (a == Direction.Down && b == Direction.Up) ||
            (a == Direction.Left && b == Direction.Right) ||
            (a == Direction.Right && b == Direction.Left);
    }

    private void StartMovementRoutine()
    {
        if (moveRoutine == null)
            moveRoutine = StartCoroutine(MoveRoutine());
    }

    private void StopMovement()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    private IEnumerator MoveRoutine()
    {
        while (!isGameOver)
        {
            if (!isPaused)
                MoveSnake();

            yield return new WaitForSeconds(movementInterval);
        }

        moveRoutine = null;
    }

    private void MoveSnake()
    {
        if (positions.Count == 0)
            return;

        currentDirection = queuedDirection;
        UpdateHeadEyes();

        Vector2Int newHead = positions[0];

        switch (currentDirection)
        {
            case Direction.Up:
                newHead += Vector2Int.up;
                break;

            case Direction.Down:
                newHead += Vector2Int.down;
                break;

            case Direction.Left:
                newHead += Vector2Int.left;
                break;

            case Direction.Right:
                newHead += Vector2Int.right;
                break;
        }

        if (!gridManager.IsWithinBounds(newHead))
        {
            TriggerGameOver("Wall Collision");
            return;
        }

        Food food = FindAnyObjectByType<Food>();

        bool eatingFood =
            food != null &&
            newHead == food.GridPosition;

        int collisionLimit = eatingFood
            ? positions.Count
            : positions.Count - 1;

        for (int i = 0; i < collisionLimit; i++)
        {
            if (positions[i] == newHead)
            {
                TriggerGameOver("Self Collision");
                return;
            }
        }

        positions.Insert(0, newHead);

        if (!eatingFood)
        {
            positions.RemoveAt(positions.Count - 1);
        }
        else
        {
            GameObject newSegment = CreateSegment(false);
            segments.Add(newSegment);

            if (food != null)
                food.Collect(this);

            Debug.Log("Food collected! Length: " + positions.Count);
        }

        UpdateSegmentPositions();
    }

    private void UpdateSegmentPositions()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            if (i < segments.Count && segments[i] != null)
            {
                segments[i].transform.position =
                    gridManager.GridToWorld(positions[i]);
            }
        }
    }

    public void AddSegment()
    {
        if (isGameOver || positions.Count == 0)
            return;

        Vector2Int tailPosition = positions[positions.Count - 1];

        positions.Add(tailPosition);

        GameObject newSegment = CreateSegment(false);
        segments.Add(newSegment);

        UpdateSegmentPositions();

        Debug.Log("Snake grew! Length: " + positions.Count);
    }

    private void TriggerGameOver(string reason)
    {
        if (isGameOver)
            return;

        isGameOver = true;
        isPaused = true;

        StopMovement();

        gameManager?.GameOver(reason);

        Debug.Log("Game Over: " + reason);
    }

    public void PauseGame()
    {
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (!isGameOver)
            isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        InitializeSnake();

        gameManager?.ResetState();
    }
}
