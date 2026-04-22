using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    protected Transform player;
    protected Enemy enemy;

    [Header("AI Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 5f;

    // Pit blocking
    [Header("Environment")]
    [SerializeField] private LayerMask pitSolidMask; // assign PitSolid in inspector

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
        if (enemy.IsStunned) return;

        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 targetPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;

        if (IsBlockedByPit(targetPos))
            return;

        transform.position = targetPos;
    }

    protected bool IsBlockedByPit(Vector2 targetPos)
    {
        // Small radius to check if the next position overlaps a PitSolid
        float radius = 0.1f;
        return Physics2D.OverlapCircle(targetPos, radius, pitSolidMask) != null;
    }
}
