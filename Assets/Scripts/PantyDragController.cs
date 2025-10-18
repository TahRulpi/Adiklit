using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Needed for the Image component

// This script should be on the parent GameObject of your cup/panty.
public class PantyDragController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    // The maximum horizontal distance the panty can move from the center (0).
    public float xLimit = 300f;

    // Reference to the GameManager for scoring and game state.
    public GameManager gameManager;

    // --- FIELDS FOR BLOOD FILL VISUAL EFFECT ---
    [Header("Blood Fill Visuals")]
    // Assign the 'BloodFill' Image component here in the Inspector.
    // This Image MUST have its 'Image Type' set to 'Filled' and 'Fill Method' to 'Vertical'.
    public Image bloodFillImage;

    // How much the fill amount increases per absorbed drop (e.g., 0.05f for 5% fill per drop).
    [Range(0.01f, 0.2f)]
    public float bloodAbsorbAmount = 0.05f;

    // The maximum fill amount (usually 1.0 for completely full).
    public float maxFillAmount = 1.0f;
    // ------------------------------------

    private RectTransform rectTransform;
    private Vector2 dragStartPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("PantyDragController requires a RectTransform component.");
        }

        // Ensure the fill is reset when the script starts
        if (bloodFillImage != null)
        {
            bloodFillImage.fillAmount = 0f;
        }
        else
        {
            Debug.LogError("BloodFillImage not assigned! The filling effect will not work.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculate new X position based on drag delta
        Vector2 newPos = dragStartPos + new Vector2(eventData.position.x - eventData.pressPosition.x, 0);

        // Clamp the position within the limits
        newPos.x = Mathf.Clamp(newPos.x, -xLimit, xLimit);

        // Apply the new position.
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
                gameManager.AddScore();

                // --- CALL THE FILL METHOD ---
                FillWithBlood();
            }
            else
            {
                Debug.LogError("GameManager not assigned to PantyDragController!");
            }
        }
    }

    // --- METHOD TO FILL THE CUP ---
    /// <summary>
    /// Increases the fill amount of the bloodFillImage.
    /// </summary>
    private void FillWithBlood()
    {
        if (bloodFillImage == null) return;

        // Increase the fill amount, clamping it at the maximum
        bloodFillImage.fillAmount = Mathf.Min(bloodFillImage.fillAmount + bloodAbsorbAmount, maxFillAmount);
    }
}