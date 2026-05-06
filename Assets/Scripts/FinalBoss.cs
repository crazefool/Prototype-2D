using UnityEngine;
using System.Collections;

public class FinalBoss : BaseEnemyAI, IBossHealth
{
    [Header("Boss Settings")]
    [SerializeField] private BossManager bossManager;

    [Header("Pattern Settings")]
    [SerializeField] private int dashesPerCycle = 3;
    [SerializeField] private float waitAfterDash = 0.25f;
    [SerializeField] private float waitAfterHazard = 0.4f;
    [SerializeField] private float waitAfterAOE = 0.5f;

    [Header("Teleport Settings")]
    [SerializeField] private float minTeleportDistance = 2f;
    [SerializeField] private float maxTeleportDistance = 5f;
    [SerializeField] private float teleportCheckRadius = 0.4f;
    [SerializeField] private LayerMask teleportBlockedMask;
    [SerializeField] private float squashDuration = 0.1f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashAnticipationTime = 0.25f;
    [SerializeField] private Color dashAnticipationColor = Color.red;
    [SerializeField] private LayerMask wallMask;

    [Header("Hazard Projectile Settings")]
    [SerializeField] private GameObject hazardProjectilePrefab;
    [SerializeField] private float hazardProjectileSpeed = 7f;
    [SerializeField] private float hazardAnticipationTime = 0.35f;
    [SerializeField] private Color hazardAnticipationColor = Color.yellow;

    [Header("Phase Summon Settings")]
    [SerializeField] private GameObject[] phase2Summons;
    [SerializeField] private GameObject[] phase3Summons;
    [SerializeField] private float summonRadius = 2.5f;
    [SerializeField] private float summonAnticipationTime = 0.5f;
    [SerializeField] private Color summonAnticipationColor = new Color(1f, 0f, 1f); // Purple

    private bool phase2Triggered = false;
    private bool phase3Triggered = false;

    [Header("AOE Settings")]
    [SerializeField] private GameObject aoePrefab;
    [SerializeField] private float aoeChargeTime = 1.2f;
    [SerializeField] private float aoeFlashInterval = 0.15f;
    [SerializeField] private Color aoeChargeColor = Color.red;

    [Header("Visuals")]
    [SerializeField] private Color hitColor = Color.white;

    private SpriteRenderer sr;
    private Color originalColor;
    private Rigidbody2D rb;

    private bool fightActive = false;
    private bool isDead = false;

    public int CurrentHealth => enemy.CurrentHealth;
    public int MaxHealth => enemy.MaxHealth;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        enemy.isBoss = true;

        if (bossManager == null)
            bossManager = FindFirstObjectByType<BossManager>();
    }

    private void Update()
    {
        if (!fightActive && Input.GetKeyDown(KeyCode.T))
        {
            BeginFight();
        }

        if (!fightActive || isDead)
            return;

        if (enemy.CurrentHealth <= 0 && !isDead)
        {
            Die();
            return;
        }
    }

    public void BeginFight()
    {
        if (fightActive || isDead)
            return;

        fightActive = true;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (fightActive && !isDead)
        {
            CheckPhaseTransitions();

            // --- DASH / TELEPORT PHASE ---
            for (int i = 0; i < dashesPerCycle; i++)
            {
                yield return TeleportRoutine();
                if (isDead) yield break;

                yield return DashRoutine();
                if (isDead) yield break;

                yield return new WaitForSeconds(waitAfterDash);
            }

            // --- HAZARD PROJECTILE ---
            yield return ShootHazardProjectileRoutine();
            if (isDead) yield break;

            yield return new WaitForSeconds(waitAfterHazard);

            // --- AOE CHARGE ---
            yield return AOEChargeRoutine();
            if (isDead) yield break;

            // --- AOE EXPLOSION ---
            yield return AOEExplosionRoutine();
            if (isDead) yield break;

            yield return new WaitForSeconds(waitAfterAOE);
        }
    }

    // ---------- PHASE CHECK ----------
    private void CheckPhaseTransitions()
    {
        float hpPercent = (float)enemy.CurrentHealth / enemy.MaxHealth;

        if (!phase2Triggered && hpPercent <= 0.66f)
        {
            phase2Triggered = true;
            StartCoroutine(SummonPhaseEnemies(phase2Summons));
        }

        if (!phase3Triggered && hpPercent <= 0.33f)
        {
            phase3Triggered = true;
            StartCoroutine(SummonPhaseEnemies(phase3Summons));
        }
    }

    // ---------- SUMMON ROUTINE ----------
    private IEnumerator SummonPhaseEnemies(GameObject[] list)
    {
        if (list == null || list.Length == 0)
            yield break;

        // Anticipation flash
        if (sr != null)
            sr.color = summonAnticipationColor;

        yield return new WaitForSeconds(summonAnticipationTime);

        if (sr != null)
            sr.color = originalColor;

        // Spawn in circle
        float angleStep = 360f / list.Length;

        for (int i = 0; i < list.Length; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * summonRadius;
            Vector2 spawnPos = (Vector2)transform.position + offset;

            Instantiate(list[i], spawnPos, Quaternion.identity);
        }
    }

    // ---------- TELEPORT ----------
    private IEnumerator TeleportRoutine()
    {
        Vector3 originalScale = transform.localScale;

        float t = 0f;
        while (t < squashDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.2f, t / squashDuration);
            yield return null;
        }

        transform.position = FindValidTeleportPosition();

        t = 0f;
        while (t < squashDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * 0.2f, originalScale, t / squashDuration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private Vector2 FindValidTeleportPosition()
    {
        Vector2 playerPos = GetPlayerCenter();
        Vector2 chosen = transform.position;

        for (int i = 0; i < 20; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(minTeleportDistance, maxTeleportDistance);

            Vector2 candidate = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            if (Vector2.Distance(candidate, playerPos) < minTeleportDistance)
                continue;

            if (Physics2D.OverlapCircle(candidate, teleportCheckRadius, teleportBlockedMask))
                continue;

            chosen = candidate;
            break;
        }

        return chosen;
    }

    // ---------- DASH ----------
    private IEnumerator DashRoutine()
    {
        if (enemy.IsStunned)
            yield break;

        Vector2 dashDirection = (GetPlayerCenter() - (Vector2)transform.position).normalized;

        if (sr != null)
            sr.color = dashAnticipationColor;

        yield return new WaitForSeconds(dashAnticipationTime);

        if (sr != null)
            sr.color = originalColor;

        float elapsed = 0f;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(wallMask);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[1];

        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            int hitCount = rb.Cast(dashDirection, filter, hits, dashSpeed * Time.deltaTime);
            if (hitCount > 0)
            {
                dashDirection = Vector2.Reflect(dashDirection, hits[0].normal).normalized;
                rb.linearVelocity = dashDirection * (dashSpeed * 0.7f);
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    // ---------- HAZARD PROJECTILE ----------
    private IEnumerator ShootHazardProjectileRoutine()
    {
        if (sr != null)
            sr.color = hazardAnticipationColor;

        yield return new WaitForSeconds(hazardAnticipationTime);

        if (sr != null)
            sr.color = originalColor;

        if (hazardProjectilePrefab != null)
        {
            Vector2 target = GetPlayerCenter();

            GameObject proj = Instantiate(hazardProjectilePrefab, transform.position, Quaternion.identity);
            BossHazardProjectile p = proj.GetComponent<BossHazardProjectile>();

            if (p != null)
                p.Initialize(target, hazardProjectileSpeed);
        }
    }

    // ---------- AOE ----------
    private IEnumerator AOEChargeRoutine()
    {
        float elapsed = 0f;
        bool toggle = false;

        while (elapsed < aoeChargeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            toggle = !toggle;

            if (sr != null)
                sr.color = toggle ? aoeChargeColor : originalColor;

            yield return new WaitForSecondsRealtime(aoeFlashInterval);
        }

        if (sr != null)
            sr.color = originalColor;
    }

    private IEnumerator AOEExplosionRoutine()
    {
        if (aoePrefab != null)
            Instantiate(aoePrefab, transform.position, Quaternion.identity);

        yield return new WaitForSecondsRealtime(0.2f);
    }

    // ---------- DAMAGE / DEATH ----------
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            HookshotController hook = FindFirstObjectByType<HookshotController>();
            if (hook != null && hook.IsPulling)
                return;

            enemy.TakeDamage(1, (transform.position - other.transform.position).normalized);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;

        if (sr != null)
            sr.color = Color.gray;

        if (bossManager != null)
            bossManager.OnBossDefeated();

        Destroy(gameObject, 1.5f);
    }
}
