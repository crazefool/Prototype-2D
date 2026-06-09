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

    [Header("Environment")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallAvoidRadius = 0.3f;

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

        float dist = Vector2.Distance(transform.position, GetPlayerCenter());

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

        if (isShooting)
            return;

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

        if (fireTimer <= 0f)
            StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        fireTimer = fireCooldown;

        if (sr != null)
        {
            Color flashColor = anticipationColor;

            // If white, force a very bright white flash
            if (flashColor == Color.white)
                flashColor = new Color(3f, 3f, 3f);

            var block = new MaterialPropertyBlock();
            sr.GetPropertyBlock(block);
            block.SetColor("_Color", flashColor);
            sr.SetPropertyBlock(block);
        }

        yield return new WaitForSeconds(anticipationTime);

        if (sr != null)
        {
            var block = new MaterialPropertyBlock();
            sr.GetPropertyBlock(block);
            block.SetColor("_Color", originalColor);
            sr.SetPropertyBlock(block);
        }

        Vector2 dir = (GetPlayerCenter() - (Vector2)transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        p.Initialize(dir, projectileSpeed, projectileLifetime);

        isShooting = false;
    }

    private void MoveAwayFromPlayer()
    {
        if (enemy.IsStunned)
            return;

        Vector2 currentPos = transform.position;
        Vector2 dir = (currentPos - GetPlayerCenter()).normalized;
        Vector2 targetPos = currentPos + dir * moveSpeed * Time.deltaTime;

        if (IsNearPit(targetPos))
            return;

        if (IsNearWall(targetPos))
            return;

        transform.position = targetPos;
    }

    private bool IsNearWall(Vector2 targetPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(targetPos, wallAvoidRadius, wallMask);
        return hit != null;
    }
}
