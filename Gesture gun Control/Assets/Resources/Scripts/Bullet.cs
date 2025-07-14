using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f; // Ensure no falling
            rb.velocity = transform.up * speed;
        }

        Destroy(gameObject, 3f); // Clean up
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
