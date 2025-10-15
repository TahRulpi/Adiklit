using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    // Reference to the GameManager to report a miss.
    // This is set programmatically by GameManager when the drop is spawned.
    [HideInInspector]
    public GameManager gameManager;

    // Public variable to control how fast the drop moves down.
    public float fallSpeed = 10f;

    void Update()
    {
        // 1. Move the drop downwards based on the defined speed.
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    // Use OnTriggerEnter2D to detect when the drop hits the ground.
    // NOTE: This requires:
    // 1. BloodDrop to have a Collider2D (Is Trigger=true) and a Rigidbody2D.
    // 2. The "Ground" sprite to have a Collider2D and the tag "Ground".
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collision with the ground (missed)
        if (collision.CompareTag("Ground"))
        {
            // Report the miss to the GameManager
            if (gameManager != null)
            {
                gameManager.HandleMiss();
            }

            // Destroy the drop object
            Destroy(gameObject);
        }
    }
}
