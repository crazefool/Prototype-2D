using UnityEngine;
using System.Collections;

public class DasherEnemyAI : BaseEnemyAI
{
    [Header("Dash Settings")]
    [SerializeField] private float dashRange = 2.5f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Anticipation Settings")]
    [SerializeField] private float anticipationTime = 0.25f;
    [SerializeField] private Color anticipationColor = Color.red;

    [Header("Environment")]
    [SerializeField] private LayerMask wallMask;

    private bool isAggro = false;
    private bool isDashing = false;
    private bool isAnticipating = false;
    private float cooldownTimer = 0f;

    private Vector2 dashDirection;

    private SpriteRenderer sr;
    private Color originalColor;
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
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

        if (isAnticipating || isDashing)
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

        // --- DASH TRIGGER ---
        if (dist <= dashRange && cooldownTimer <= 0f)
        {
            StartCoroutine(DashRoutine());
            return;
        }

        // --- STOP CHASING WHEN INSIDE DASH RANGE ---
        if (dist <= dashRange)
            return;

        // --- DEFAULT: CHASE ---
        MoveTowardsPlayer();
    }

    private IEnumerator DashRoutine()
    {
        isAnticipating = true;

        dashDirection = (player.position - transform.position).normalized;

        if (sr != null)
            sr.color = anticipationColor;

        yield return new WaitForSeconds(anticipationTime);

        if (sr != null)
            sr.color = originalColor;

        isAnticipating = false;

        isDashing = true;
        cooldownTimer = dashCooldown;
        enemy.isInvulnerable = true;

        float elapsed = 0f;

        // Prepare contact filter for walls
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

                // Reflect direction
                dashDirection = Vector2.Reflect(dashDirection, normal).normalized;

                // Bounce for a short moment
                float bounceTime = 0.12f;
                float bounceElapsed = 0f;

                while (bounceElapsed < bounceTime)
                {
                    rb.linearVelocity = dashDirection * (dashSpeed * 0.7f);
                    bounceElapsed += Time.deltaTime;
                    yield return null;
                }

                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        // --- POST-DASH CORRECTION ---
        Collider2D overlap = Physics2D.OverlapCircle(transform.position, 0.2f, wallMask);
        if (overlap != null)
        {
            Vector2 away = -dashDirection * 0.1f;
            rb.position += away;
        }

        enemy.isInvulnerable = false;
        isDashing = false;
    }
}
