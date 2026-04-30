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

    // Prevent punching while attached to an enemy
    public bool IsAttached = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Idle: cannot be pushed by enemies
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Punch(Vector2 direction)
    {
        if (IsAttached)
            return;

        if (isMoving)
            return;

        isMoving = true;
        punchDirection = direction.normalized;
        startPosition = transform.position;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = punchDirection * punchSpeed;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        if (Vector2.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            StopMovement();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMoving)
            return;

        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageOnHit, punchDirection);
            Destroy(gameObject);
            return;
        }

        StopMovement();
    }

    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        // Back to idle: no enemy pushing
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
