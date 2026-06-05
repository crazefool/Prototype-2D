using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportShooterBoss : BaseEnemyAI, IBossHealth
{
    [Header("Boss Settings")]
    [SerializeField] private float teleportCooldown = 2.5f;
    [SerializeField] private float waitDuration = 1.2f;
    [SerializeField] private float attackCooldown = 1.0f;

    [Header("Teleport Points")]
    [SerializeField] private List<Transform> teleportPoints;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private float projectileLifetime = 2f;

    [Header("Summon Settings")]
    [SerializeField] private GameObject batPrefab;
    [SerializeField] private int batsPerSummon = 3;
    [SerializeField] private float summonRadius = 2f;

    [Header("HP Thresholds")]
    [SerializeField] private float summonPhase2Threshold = 0.66f;
    [SerializeField] private float summonPhase3Threshold = 0.33f;

    [Header("Visuals")]
    [SerializeField] private float teleportFlashDuration = 0.2f;
    [SerializeField] private Color teleportFlashColor = Color.cyan;

    [Header("Boss Manager Reference")]
    [SerializeField] private BossManager bossManager;

    private SpriteRenderer sr;
    private Color originalColor;

    private bool isTeleporting = false;
    private bool isAttacking = false;
    private bool phase2Triggered = false;
    private bool phase3Triggered = false;
    private bool fightActive = false;
    private bool isDead = false;

    public int CurrentHealth => enemy.CurrentHealth;
    public int MaxHealth => enemy.MaxHealth;

    protected override void Awake()
    {
        base.Awake();

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        enemy.isBoss = true;

        if (bossManager == null)
            bossManager = FindFirstObjectByType<BossManager>();
    }

    private void Update()
    {
        if (!fightActive) return;

        HandleHPTriggers();

        if (enemy.CurrentHealth <= 0 && !isDead)
        {
            Die();
            return;
        }
    }

    private void HandleHPTriggers()
    {
        float hpPercent = (float)enemy.CurrentHealth / enemy.MaxHealth;

        if (!phase2Triggered && hpPercent <= summonPhase2Threshold)
        {
            phase2Triggered = true;
            SummonBats();
        }

        if (!phase3Triggered && hpPercent <= summonPhase3Threshold)
        {
            phase3Triggered = true;
            SummonBats();
        }
    }

    public void BeginFight()
    {
        if (fightActive) return;

        fightActive = true;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (fightActive && !isDead)
        {
            if (isTeleporting || isAttacking)
            {
                yield return null;
                continue;
            }

            yield return StartCoroutine(TeleportRoutine());
            yield return StartCoroutine(AttackRoutine());
            yield return new WaitForSeconds(waitDuration);
        }
    }

    private IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        HookshotController hook = FindFirstObjectByType<HookshotController>();
        if (hook != null && hook.IsPulling)
        {
            if (enemy.IsBeingPulled)
                hook.CancelHookshot();
        }

        sr.color = teleportFlashColor;
        yield return new WaitForSeconds(teleportFlashDuration);
        sr.color = originalColor;

        if (teleportPoints.Count > 0)
        {
            Transform target = teleportPoints[Random.Range(0, teleportPoints.Count)];
            transform.position = target.position;
        }

        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        p.Initialize(dir, projectileSpeed, projectileLifetime);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

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

    private void SummonBats()
    {
        for (int i = 0; i < batsPerSummon; i++)
        {
            Vector2 offset = Random.insideUnitCircle * summonRadius;
            Vector3 spawnPos = transform.position + (Vector3)offset;
            Instantiate(batPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop all behavior immediately
        StopAllCoroutines();

        // Notify boss manager instantly
        if (bossManager != null)
            bossManager.OnBossDefeated();

        // Visual feedback
        sr.color = Color.gray;

        // Disable boss shortly after
        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        // One frame delay is enough
        yield return null;
        gameObject.SetActive(false);
    }
}
