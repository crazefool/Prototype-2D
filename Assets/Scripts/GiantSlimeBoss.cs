using UnityEngine;
using System.Collections;

public class GiantSlimeBoss : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 1.2f;
    [SerializeField] private float enemySpeed = 2f;        // renamed from cooldownSpeed
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

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        HandleHPTriggers();

        if (!isDashing)
            StartCoroutine(DashRoutine());
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

        // Anticipation flash
        Color originalColor = sr.color;
        sr.color = anticipationColor;
        yield return new WaitForSeconds(anticipationTime);
        sr.color = originalColor;

        // Pick dash direction
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        dashDirection = (playerPos - (Vector2)transform.position).normalized;

        float dashTimer = 0f;

        // Prepare contact filter for walls
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(wallMask);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[1];

        while (dashTimer < dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            // Cast using Rigidbody2D.Cast with filter
            int hitCount = rb.Cast(dashDirection, filter, hits, dashSpeed * Time.deltaTime);

            if (hitCount > 0)
            {
                Vector2 normal = hits[0].normal;

                // Reflect direction
                dashDirection = Vector2.Reflect(dashDirection, normal).normalized;

                // Apply new reflected velocity
                rb.linearVelocity = dashDirection * dashSpeed;
            }

            dashTimer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        // Cooldown walk (now using enemySpeed)
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
}
