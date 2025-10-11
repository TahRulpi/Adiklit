using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    // Public variable to control how fast the drop moves down.
    // Increase this value (e.g., from 3f to 5f or 7f) in the Inspector 
    // to make the drop fall faster.
    public float fallSpeed = 1000f;

    private float destroyY = -6f; // remove drop if it misses

    void Update()
    {
        // 1. Move the drop downwards based on the defined speed.
        // Vector2.down is shorthand for (0, -1). Time.deltaTime ensures 
        // the speed is frame-rate independent.
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // 2. Check if the drop has missed the panty
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
