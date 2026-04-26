using UnityEngine;
using System.Collections;

public class RangedEnemyAI : BaseEnemyAI
{
    private bool isAggro = false;
    private bool isShooting = false;

    [Header("Ranged Settings")]
    [SerializeField] private float shootRange = 5f;
    [SerializeField] private float retreatRange = 2f;
    [SerializeField] private float fireCooldown = 1.2f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float projectileLifetime = 2f;

    [Header("Anticipation Settings")]
    [SerializeField] private float anticipationTime = 0.25f;
    [SerializeField] private Color anticipationColor = Color.red;

    private float fireTimer = 0f;
    private SpriteRenderer sr;
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Update()
    {
        if (enemy.IsStunned)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        // --- AGGRO LOGIC ---
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

        // --- SHOOTING STATE ---
        if (isShooting)
            return; // Enemy is locked in place during anticipation + shooting

        fireTimer -= Time.deltaTime;

        // --- MOVEMENT LOGIC ---
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

        // --- READY TO SHOOT ---
        if (fireTimer <= 0f)
            StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        fireTimer = fireCooldown;

        // --- ANTICIPATION FLASH ---
        sr.color = anticipationColor;
        yield return new WaitForSeconds(anticipationTime);
        sr.color = originalColor;

        // --- SHOOT ---
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        p.Initialize(dir, projectileSpeed, projectileLifetime);

        // --- RESUME KITING ---
        isShooting = false;
    }

    private void MoveAwayFromPlayer()
    {
        if (enemy.IsStunned)
            return;

        Vector2 dir = (transform.position - player.position).normalized;
        Vector2 targetPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;

        if (IsNearPit(targetPos))
            return;

        transform.position = targetPos;
    }
}
