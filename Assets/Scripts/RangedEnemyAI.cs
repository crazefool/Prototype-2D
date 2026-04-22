using UnityEngine;

public class RangedEnemyAI : BaseEnemyAI
{
    private bool isAggro = false;

    [Header("Ranged Settings")]
    [SerializeField] private float shootRange = 5f;
    [SerializeField] private float retreatRange = 2f;
    [SerializeField] private float fireCooldown = 1.2f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float projectileLifetime = 2f;

    private float fireTimer = 0f;

    void Update()
    {
        if (enemy.IsStunned)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isAggro)
        {
            if (dist <= detectionRange)
                isAggro = true;
            else
                return;
        }
        else
        {
            if (dist > detectionRange * 1.3f)
            {
                isAggro = false;
                return;
            }
        }

        fireTimer -= Time.deltaTime;

        if (dist > shootRange)
        {
            MoveTowardsPlayer();
            return;
        }

        if (dist < retreatRange)
        {
            MoveAwayFromPlayer();
            return;
        }

        ShootAtPlayer();
    }

    private void MoveAwayFromPlayer()
    {
        if (enemy.IsStunned) return;

        Vector2 dir = (transform.position - player.position).normalized;
        Vector2 targetPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;

        if (IsBlockedByPit(targetPos))
            return;

        transform.position = targetPos;
    }

    private void ShootAtPlayer()
    {
        if (fireTimer > 0f)
            return;

        fireTimer = fireCooldown;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        p.Initialize(dir, projectileSpeed, projectileLifetime);
    }
}
