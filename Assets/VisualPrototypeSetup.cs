using UnityEngine;

public class VisualPrototypeSetup : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (gridManager == null || mainCamera == null)
        {
            Debug.LogWarning("VisualPrototypeSetup: Missing references.");
            return;
        }

        // Old background is disabled.
        // CreateGardenBackground();

        // Old decorations are disabled.
        // CreateGardenDecorations();

        // Keep the existing camera setup method.
        FrameCameraToArena();
    }

    private void FrameCameraToArena()
    {
        // Intentionally left unchanged.
    }
}
