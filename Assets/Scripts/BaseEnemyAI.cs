using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    protected Transform player;
    protected Enemy enemy;

    [Header("AI Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 5f;

    [Header("Movement Type")]
    [SerializeField] protected bool isFlying = false;   // Flying toggle

    public bool IsFlying => isFlying;

    [Header("Environment")]
    [SerializeField] private LayerMask pitTriggerMask;
    [SerializeField] private float pitAvoidRadius = 0.45f;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        enemy = GetComponent<Enemy>();
    }

    protected bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, GetPlayerCenter()) < detectionRange;
    }

    protected void MoveTowardsPlayer()
    {
        if (enemy.IsStunned)
            return;

        Vector2 playerCenter = GetPlayerCenter();
        Vector2 dir = (playerCenter - (Vector2)transform.position).normalized;
        Vector2 targetPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;

        if (!isFlying && IsNearPit(targetPos))
            return;

        transform.position = targetPos;
    }

    protected bool IsNearPit(Vector2 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, pitAvoidRadius, pitTriggerMask) != null;
    }

    // ⭐ Accurate player center based on collider bounds
    protected Vector2 GetPlayerCenter()
    {
        Collider2D playerCol = player.GetComponent<Collider2D>();
        return playerCol != null ? playerCol.bounds.center : player.position;
    }
}
