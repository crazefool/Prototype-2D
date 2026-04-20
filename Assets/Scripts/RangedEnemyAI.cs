using UnityEngine;

public class RangedEnemyAI : BaseEnemyAI
{
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

        fireTimer -= Time.deltaTime;

        // 1️⃣ TOO FAR → MOVE CLOSER
        if (dist > shootRange)
        {
            MoveTowardsPlayer();
            return;
        }

        // 2️⃣ TOO CLOSE → RETREAT
        if (dist < retreatRange)
        {
            MoveAwayFromPlayer();
            return;
        }

        // 3️⃣ IN RANGE → SHOOT
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
