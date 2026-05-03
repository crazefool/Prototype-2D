using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportShooterBoss : BaseEnemyAI, IBossHealth
{
    [Header("Boss Settings")]
    [SerializeField] private int bossMaxHealth = 12;
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

    [Header("Visuals")]
    [SerializeField] private float teleportFlashDuration = 0.2f;
    [SerializeField] private Color teleportFlashColor = Color.cyan;

    private SpriteRenderer sr;
    private Color originalColor;

    private int currentHealth;
    private bool isTeleporting = false;
    private bool isAttacking = false;
    private bool phase2Triggered = false;
    private bool phase3Triggered = false;

    private bool fightActive = false;
    private BossManager manager;

    // IBossHealth interface
    public int CurrentHealth => currentHealth;
    public int MaxHealth => bossMaxHealth;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        currentHealth = bossMaxHealth;
        manager = FindFirstObjectByType<BossManager>();
    }

    // Called by BossManager when intro finishes
    public void BeginFight()
    {
        if (fightActive) return;

        fightActive = true;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (fightActive && currentHealth > 0)
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

        // Flash before teleport
        sr.color = teleportFlashColor;
        yield return new WaitForSeconds(teleportFlashDuration);
        sr.color = originalColor;

        // Choose random teleport point
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

    public void TakeDamage(int amount)
    {
        if (!fightActive) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        CheckPhaseTriggers();
    }

    private void CheckPhaseTriggers()
    {
        float hpRatio = (float)currentHealth / bossMaxHealth;

        if (!phase2Triggered && hpRatio <= 0.66f)
        {
            phase2Triggered = true;
            SummonBats();
        }

        if (!phase3Triggered && hpRatio <= 0.33f)
        {
            phase3Triggered = true;
            SummonBats();
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
        StopAllCoroutines();
        sr.color = Color.gray;

        if (manager != null)
            manager.OnBossDefeated();

        Destroy(gameObject, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            Vector2 dir = (transform.position - other.transform.position).normalized;
            TakeDamage(1);
        }
    }
}
