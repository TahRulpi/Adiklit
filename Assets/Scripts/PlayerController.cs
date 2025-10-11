using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float xLimit = 2.5f; // limit to keep panty on screen
    public GameManager gameManager;

    void Update()
    {
        float move = Input.GetAxis("Horizontal"); // A/D or arrow keys
        transform.Translate(Vector2.right * move * moveSpeed * Time.deltaTime);

        // Clamp position (keep inside screen)
        float clampedX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BloodDrop"))
        {
            Destroy(collision.gameObject);
            gameManager.AddScore();
            gameManager.SpawnNextDrop();
        }
    }
}
