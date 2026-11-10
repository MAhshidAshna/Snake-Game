

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Playing,
        Paused,
        GameOver
    }

    [Header("Game States")]
    public GameState currentState = GameState.Ready;

    [Header("References")]
    [SerializeField] private SnakeController snakeController;
    [SerializeField] private GridManager gridManager;

    private void Awake()
    {
        if (snakeController == null)
            snakeController = FindAnyObjectByType<SnakeController>();

        if (gridManager == null)
            gridManager = FindAnyObjectByType<GridManager>();

        Time.timeScale = 1f;
    }

    private void Start()
    {
        currentState = GameState.Ready;

        if (snakeController == null)
        {
            Debug.LogError("GameManager: SnakeController reference is missing!");
            return;
        }

        Debug.Log("Game is Ready. Press an arrow key or WASD to start.");
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (currentState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.P))
                TogglePause();
        }
        else if (currentState == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
                Restart();
        }
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;

            snakeController?.PauseGame();

            Debug.Log("Game Paused");
        }
        else if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;

            snakeController?.ResumeGame();

            Debug.Log("Game Resumed");
        }
    }

    public void GameOver(string reason)
    {
        if (currentState == GameState.GameOver)
            return;

        currentState = GameState.GameOver;
        Time.timeScale = 0f;

        Debug.Log("Game Over: " + reason);
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        if (snakeController == null)
        {
            Debug.LogError("GameManager: Cannot restart. SnakeController is missing!");
            return;
        }

        snakeController.RestartGame();

        currentState = GameState.Ready;

        Debug.Log("Game Restarted");
    }

    public void ResetState()
    {
        currentState = GameState.Ready;
        Time.timeScale = 1f;

        Debug.Log("Game state reset.");
    }

    public void SetPlaying()
    {
        if (currentState == GameState.Ready)
        {
            currentState = GameState.Playing;
            Debug.Log("Game Started!");
        }
    }
}
