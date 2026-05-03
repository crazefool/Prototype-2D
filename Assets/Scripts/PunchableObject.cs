using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PunchableObject : MonoBehaviour
{
    [Header("Punch Settings")]
    [SerializeField] private float punchSpeed = 10f;
    [SerializeField] private float maxTravelDistance = 4f;
    [SerializeField] private int damageOnHit = 1;

    [Header("Layer Settings")]
    [SerializeField] private string bossLayerName = "Boss";
    [SerializeField] private string objectLayerName = "Object";

    private Rigidbody2D rb;
    private Vector2 punchDirection;
    private Vector2 startPosition;
    private bool isMoving = false;

    private int bossLayer;
    private int objectLayer;

    public bool IsAttached = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Safely convert layer names to indices
        bossLayer = LayerMask.NameToLayer(bossLayerName);
        objectLayer = LayerMask.NameToLayer(objectLayerName);

        // Warn if layers are missing
        if (bossLayer == -1 || objectLayer == -1)
        {
            Debug.LogError($"PunchableObject: One or both layers not found. Boss='{bossLayerName}' ({bossLayer}), Object='{objectLayerName}' ({objectLayer}). Check layer names in Project Settings > Tags and Layers.");
        }
    }

    public void Punch(Vector2 direction)
    {
        if (IsAttached || isMoving)
            return;

        isMoving = true;
        punchDirection = direction.normalized;
        startPosition = transform.position;

        // Only enable collision if layers are valid
        if (bossLayer >= 0 && objectLayer >= 0)
            Physics2D.IgnoreLayerCollision(bossLayer, objectLayer, false);

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
            StopMovement();
            Destroy(gameObject);
            return;
        }

        StopMovement();
    }

    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        // Disable collision again if layers are valid
        if (bossLayer >= 0 && objectLayer >= 0)
            Physics2D.IgnoreLayerCollision(bossLayer, objectLayer, true);

        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
