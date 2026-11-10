
using UnityEngine;

public class Food : MonoBehaviour
{
    private FoodSpawner spawner;
    private bool isCollected = false;

    public Vector2Int GridPosition { get; private set; }

    public void Initialize(FoodSpawner foodSpawner, Vector2Int position)
    {
        spawner = foodSpawner;
        GridPosition = position;
        isCollected = false;
    }

    public void Collect(SnakeController snake)
    {
        if (isCollected)
            return;

        if (snake == null || spawner == null)
        {
            Debug.LogError("Food: SnakeController or FoodSpawner is missing!");
            return;
        }

        isCollected = true;

        Destroy(gameObject);

        spawner.SpawnFood();

        Debug.Log("Food collected successfully!");
    }
}

