using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    private float destroyY = -6f; // remove drop if it misses

    void Update()
    {
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
