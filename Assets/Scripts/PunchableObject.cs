using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PunchableObject : MonoBehaviour
{
    [Header("Punch Settings")]
    [SerializeField] private float punchSpeed = 10f;
    [SerializeField] private float maxTravelDistance = 4f;
    [SerializeField] private int damageOnHit = 1;

    private Rigidbody2D rb;
    private Vector2 punchDirection;
    private Vector2 startPosition;
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    /// <summary>
    /// Called by the sword when the object is hit.
    /// </summary>
    public void Punch(Vector2 direction)
    {
        if (isMoving)
            return;

        isMoving = true;
        punchDirection = direction.normalized;
        startPosition = transform.position;

        rb.linearVelocity = punchDirection * punchSpeed;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        // Stop after traveling max distance
        if (Vector2.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            StopMovement();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMoving)
            return;

        // Hit an enemy
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageOnHit, punchDirection);
            Destroy(gameObject);
            return;
        }

        // Hit a wall or anything else
        StopMovement();
    }

    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }
}
