using UnityEngine;
using UnityEngine.EventSystems;

// IMPORTANT: This script requires the UI element (the panty) to have a RectTransform.
// It also needs a 2D Collider (like Box Collider 2D) and a Rigidbody 2D to handle OnTriggerEnter2D.
public class PantyDragController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    // The maximum horizontal distance the panty can move from the center (0).
    // Adjust this value in the Inspector based on your screen size (e.g., 300f for Canvas or 2.5f for World Space).
    public float xLimit = 300f;

    // Reference to the GameManager for scoring and spawning logic.
    public GameManager gameManager;

    private RectTransform rectTransform;
    private Vector2 dragStartPos;

    void Awake()
    {
        // Get the RectTransform component if this is a UI element
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("PantyDragController requires a RectTransform component (for UI elements).");
        }
    }

    // Called when a drag operation officially starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the current anchored position of the UI element
        dragStartPos = rectTransform.anchoredPosition;
    }

    // Called every frame while dragging is active
    public void OnDrag(PointerEventData eventData)
    {
        // Calculate the new X position based on the drag start position plus the X difference (delta) from the pointer
        // We ignore the Y delta to only allow horizontal movement.
        Vector2 newPos = dragStartPos + new Vector2(eventData.position.x - eventData.pressPosition.x, 0);

        // Clamp the new X position to keep the panty within the defined limits
        newPos.x = Mathf.Clamp(newPos.x, -xLimit, xLimit);

        // Apply the new position. We keep the original Y position.
        rectTransform.anchoredPosition = new Vector2(newPos.x, rectTransform.anchoredPosition.y);
    }

    // --- Collision Logic (Moved from PlayerController) ---

    // Note: Since this is likely a UI element, ensure it has a 2D Collider (Box Collider 2D) 
    // and a Rigidbody 2D set to 'Kinematic' for this to work with falling "BloodDrop" objects.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BloodDrop"))
        {
            Destroy(collision.gameObject);

            // Check if gameManager is assigned before calling its methods
            if (gameManager != null)
            {
                gameManager.AddScore();
                gameManager.SpawnNextDrop();
            }
            else
            {
                Debug.LogError("GameManager not assigned to PantyDragController!");
            }
        }
    }
}
