using UnityEngine;
using System.Collections;

public class FinalBoss : BaseEnemyAI, IBossHealth
{
    [Header("Boss Settings")]
    [SerializeField] private BossManager bossManager;

    [Header("Pattern Settings")]
    [SerializeField] private int dashesPerCycle = 3;
    [SerializeField] private float waitAfterDash = 0.25f;
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
            Debug.Log("DEBUG: T pressed — starting boss fight.");
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
        Debug.Log("DEBUG: Boss fight started.");
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        Debug.Log("DEBUG: Boss loop entered.");

        while (fightActive && !isDead)
        {
            for (int i = 0; i < dashesPerCycle; i++)
            {
                Debug.Log($"DEBUG: Starting teleport + dash {i + 1}/{dashesPerCycle}");
                yield return TeleportRoutine();
                if (isDead) yield break;

                yield return DashRoutine();
                if (isDead) yield break;

                yield return new WaitForSeconds(waitAfterDash);
            }

            Debug.Log("DEBUG: Starting AOE charge.");
            yield return AOEChargeRoutine();
            if (isDead) yield break;

            Debug.Log("DEBUG: Starting AOE explosion.");
            yield return AOEExplosionRoutine();
            if (isDead) yield break;

            Debug.Log("DEBUG: AOE finished, waiting before next cycle.");
            yield return new WaitForSeconds(waitAfterAOE);
        }

        Debug.Log("DEBUG: Boss loop exited.");
    }

    // ---------- TELEPORT ----------

    private IEnumerator TeleportRoutine()
    {
        Vector3 originalScale = transform.localScale;

        float t = 0f;
        while (t < squashDuration)
        {
            t += Time.deltaTime;
            float lerp = t / squashDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.2f, lerp);
            yield return null;
        }

        Vector2 newPos = FindValidTeleportPosition();
        transform.position = newPos;

        t = 0f;
        while (t < squashDuration)
        {
            t += Time.deltaTime;
            float lerp = t / squashDuration;
            transform.localScale = Vector3.Lerp(originalScale * 0.2f, originalScale, lerp);
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
                Vector2 normal = hits[0].normal;
                dashDirection = Vector2.Reflect(dashDirection, normal).normalized;
                rb.linearVelocity = dashDirection * (dashSpeed * 0.7f);
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    // ---------- AOE (Prefab radius only) ----------

    private IEnumerator AOEChargeRoutine()
    {
        Debug.Log("DEBUG: AOE charge started.");

        float elapsed = 0f;
        bool toggle = false;

        while (elapsed < aoeChargeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            toggle = !toggle;

            if (sr != null)
                sr.color = toggle ? aoeChargeColor : originalColor;

            float wait = aoeFlashInterval > 0f ? aoeFlashInterval : 0.05f;
            yield return new WaitForSecondsRealtime(wait);
        }

        if (sr != null)
            sr.color = originalColor;

        Debug.Log("DEBUG: AOE charge finished.");
    }

    private IEnumerator AOEExplosionRoutine()
    {
        Debug.Log("DEBUG: AOE explosion triggered.");

        if (aoePrefab != null)
        {
            Instantiate(aoePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("DEBUG: AOE prefab is null!");
        }

        yield return new WaitForSecondsRealtime(0.2f);
        Debug.Log("DEBUG: AOE explosion finished.");
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

            Vector2 knockbackDir = (transform.position - other.transform.position).normalized;
            enemy.TakeDamage(1, knockbackDir);
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

        Debug.Log("DEBUG: Boss died.");
        Destroy(gameObject, 1.5f);
    }
}
