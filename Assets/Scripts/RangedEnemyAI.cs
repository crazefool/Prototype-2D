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

        // DETECTION / DE-AGGRO USING BaseEnemyAI.detectionRange
        if (!isAggro)
        {
            if (dist <= detectionRange)
                isAggro = true;
            else
                return; // not aggroed yet
        }
        else
        {
            if (dist > detectionRange * 1.3f) // small buffer
            {
                isAggro = false;
                return;
            }
        }

        // NORMAL RANGED ENEMY LOGIC
        fireTimer -= Time.deltaTime;

        // Too far → move closer
        if (dist > shootRange)
        {
            MoveTowardsPlayer();
            return;
        }

        // Too close → retreat
        if (dist < retreatRange)
        {
            MoveAwayFromPlayer();
            return;
        }

        // In range → shoot
        ShootAtPlayer();
    }

    private void MoveAwayFromPlayer()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
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
