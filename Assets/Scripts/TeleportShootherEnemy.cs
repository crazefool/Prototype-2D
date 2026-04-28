using UnityEngine;
using System.Collections;

public class TeleportShooterEnemyAI : BaseEnemyAI
{
    [Header("Teleport Settings")]
    [SerializeField] private float minTeleportDistance = 2f;
    [SerializeField] private float maxTeleportDistance = 5f;
    [SerializeField] private float teleportCooldown = 2f;
    [SerializeField] private float teleportCheckRadius = 0.4f;
    [SerializeField] private LayerMask blockedMask; // walls + pits

    [Header("Anticipation Settings")]
    [SerializeField] private float anticipationTime = 0.25f;
    [SerializeField] private Color anticipationColor = Color.red;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float projectileLifetime = 2f;

    private SpriteRenderer sr;
    private Color originalColor;

    private bool isAggro = false;
    private bool isTeleporting = false;
    private bool isAnticipating = false;
    private bool isShooting = false;

    private float cooldownTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    private void Update()
    {
        if (enemy.IsStunned)
            return;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isTeleporting || isAnticipating || isShooting)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        // AGGRO
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

        // TELEPORT + SHOOT LOOP
        if (cooldownTimer <= 0f)
            StartCoroutine(TeleportAndShootRoutine());
    }

    private IEnumerator TeleportAndShootRoutine()
    {
        cooldownTimer = teleportCooldown;

        // TELEPORT
        isTeleporting = true;
        yield return StartCoroutine(SquashTeleport());
        isTeleporting = false;

        // ANTICIPATION
        isAnticipating = true;
        if (sr != null)
            sr.color = anticipationColor;

        yield return new WaitForSeconds(anticipationTime);

        if (sr != null)
            sr.color = originalColor;

        isAnticipating = false;

        // SHOOT
        isShooting = true;
        ShootProjectile();
        isShooting = false;
    }

    private IEnumerator SquashTeleport()
    {
        Vector3 originalScale = transform.localScale;

        // SQUASH
        float t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.2f, t / 0.1f);
            yield return null;
        }

        // TELEPORT
        Vector2 newPos = FindValidTeleportPosition();
        transform.position = newPos;

        // UNSQUASH
        t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * 0.2f, originalScale, t / 0.1f);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private Vector2 FindValidTeleportPosition()
    {
        Vector2 playerPos = player.position;
        Vector2 chosen = transform.position;

        for (int i = 0; i < 20; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(minTeleportDistance, maxTeleportDistance);

            Vector2 candidate = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            // Avoid teleporting on top of player
            if (Vector2.Distance(candidate, playerPos) < minTeleportDistance)
                continue;

            // Check walls + pits
            if (Physics2D.OverlapCircle(candidate, teleportCheckRadius, blockedMask))
                continue;

            chosen = candidate;
            break;
        }

        return chosen;
    }

    private void ShootProjectile()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        p.Initialize(dir, projectileSpeed, projectileLifetime);
    }
}
