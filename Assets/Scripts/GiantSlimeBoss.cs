using UnityEngine;
using System.Collections;

public class GiantSlimeBoss : MonoBehaviour, IBossHealth
{
    [Header("Movement Settings")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 1.2f;
    [SerializeField] private float enemySpeed = 2f;
    [SerializeField] private float cooldownDuration = 1.0f;

    [Header("Anticipation")]
    [SerializeField] private float anticipationTime = 0.4f;
    [SerializeField] private Color anticipationColor = Color.red;

    [Header("Summoning")]
    [SerializeField] private GameObject[] normalSlimes;
    [SerializeField] private GameObject[] fastSlimes;
    [SerializeField] private int normalSlimeCount = 2;
    [SerializeField] private int fastSlimeCount = 3;

    [Header("HP Thresholds")]
    [SerializeField] private float summonNormalThreshold = 0.66f;
    [SerializeField] private float summonFastThreshold = 0.33f;

    [Header("Environment")]
    [SerializeField] private LayerMask wallMask;

    private Enemy enemy;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;

    private bool summonedNormal = false;
    private bool summonedFast = false;
    private bool isDashing = false;
    private Vector2 dashDirection;

    public int CurrentHealth => enemy.CurrentHealth;
    public int MaxHealth => enemy.MaxHealth;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        // ⭐ Boss controls its own death (Enemy.cs will NOT destroy it)
        enemy.isBoss = true;

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        HandleHPTriggers();

        if (!isDashing)
            StartCoroutine(DashRoutine());

        if (enemy.CurrentHealth <= 0)
            Die();
    }

    private void HandleHPTriggers()
    {
        float hpPercent = (float)enemy.CurrentHealth / enemy.MaxHealth;

        if (!summonedNormal && hpPercent <= summonNormalThreshold)
        {
            summonedNormal = true;
            SummonSlimes(normalSlimes, normalSlimeCount);
        }

        if (!summonedFast && hpPercent <= summonFastThreshold)
        {
            summonedFast = true;
            SummonSlimes(fastSlimes, fastSlimeCount);
        }
    }

    private void SummonSlimes(GameObject[] prefabs, int count)
    {
        if (prefabs.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * 2f;
            Instantiate(
                prefabs[Random.Range(0, prefabs.Length)],
                (Vector2)transform.position + offset,
                Quaternion.identity
            );
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Color originalColor = sr.color;
        sr.color = anticipationColor;
        yield return new WaitForSeconds(anticipationTime);
        sr.color = originalColor;

        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        dashDirection = (playerPos - (Vector2)transform.position).normalized;

        float dashTimer = 0f;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(wallMask);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[1];

        while (dashTimer < dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            int hitCount = rb.Cast(dashDirection, filter, hits, dashSpeed * Time.deltaTime);

            if (hitCount > 0)
            {
                Vector2 normal = hits[0].normal;
                dashDirection = Vector2.Reflect(dashDirection, normal).normalized;
                rb.linearVelocity = dashDirection * dashSpeed;
            }

            dashTimer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        float cooldownTimer = 0f;
        while (cooldownTimer < cooldownDuration)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 dir = (playerPos - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * enemySpeed;

            cooldownTimer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            Vector2 knockbackDir = (transform.position - other.transform.position).normalized;
            enemy.TakeDamage(1, knockbackDir);
        }
    }

    private void Die()
    {
        FindFirstObjectByType<BossManager>()?.OnBossDefeated();
        Destroy(gameObject, 0.2f);
    }
}
