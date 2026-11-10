
using UnityEngine;

/// <summary>
/// Handles manual snake growth for testing.
/// Press G to add one body segment.
/// </summary>
public class SnakeGrowth : MonoBehaviour
{
    [Header("Growth References")]
    [SerializeField] private SnakeController snakeController;

    private void Awake()
    {
        if (snakeController == null)
        {
            snakeController = FindAnyObjectByType<SnakeController>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddSegment();
        }
    }

    public void AddSegment()
    {
        if (snakeController == null)
        {
            Debug.LogError("SnakeGrowth: SnakeController not found!");
            return;
        }

        snakeController.AddSegment();

        Debug.Log("SnakeGrowth: One segment added!");
    }
}
