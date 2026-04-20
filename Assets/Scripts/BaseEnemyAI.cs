using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    protected Transform player;
    protected Enemy enemy; // your damage + stun script

    [Header("AI Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 5f;

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
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }
}
