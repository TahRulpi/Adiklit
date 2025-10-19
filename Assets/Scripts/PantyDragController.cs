using UnityEngine;
using UnityEngine.EventSystems;

public class PantyDragController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public float xLimit = 300f;

    // Reference to the GameManager for scoring and game state.
    public GameManager gameManager;

    private RectTransform rectTransform;
    private Vector2 dragStartPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("PantyDragController requires a RectTransform component (for UI elements).");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = dragStartPos + new Vector2(eventData.position.x - eventData.pressPosition.x, 0);
        newPos.x = Mathf.Clamp(newPos.x, -xLimit, xLimit);
        rectTransform.anchoredPosition = new Vector2(newPos.x, rectTransform.anchoredPosition.y);
    }

    // --- Collision Logic (Catching the Drop) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BloodDrop"))
        {
            Destroy(collision.gameObject);

            if (gameManager != null)
            {
                // Call AddScore, which now handles score, visual fill, and button logic
                gameManager.AddScore();
            }
            else
            {
                Debug.LogError("GameManager not assigned to PantyDragController!");
            }
        }
    }
}