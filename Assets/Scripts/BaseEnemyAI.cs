using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    protected Transform player;
    protected Enemy enemy;

    [Header("AI Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 5f;

    [Header("Environment")]
    [SerializeField] private LayerMask pitTriggerMask;   // Assign PitTrigger layer
    [SerializeField] private float pitAvoidRadius = 0.45f; // ⭐ NEW: safer radius

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        enemy = GetComponent<Enemy>();
    }

    protected bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) < detectionRange;
    }

    protected void MoveTowardsPlayer()
    {
        if (enemy.IsStunned) 
            return;

        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 targetPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;

        // ⭐ NEW: Avoid pits BEFORE stepping into the trigger
        if (IsNearPit(targetPos))
            return;

        transform.position = targetPos;
    }

    // ⭐ NEW: Enemy avoids pits using a larger radius
    protected bool IsNearPit(Vector2 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, pitAvoidRadius, pitTriggerMask) != null;
    }
}
