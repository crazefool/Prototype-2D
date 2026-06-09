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

        if (dist <= dashRange && cooldownTimer <= 0f)
        {
            StartCoroutine(DashRoutine());
            return;
        }

        if (dist <= dashRange)
            return;

        MoveTowardsPlayer();
    }

    private IEnumerator DashRoutine()
    {
        isAnticipating = true;
        dashDirection = (GetPlayerCenter() - (Vector2)transform.position).normalized;

        if (sr != null)
        {
            Color flashColor = anticipationColor;

            // If white, force a VERY bright white flash
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

        isAnticipating = false;
        isDashing = true;
        cooldownTimer = dashCooldown;

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
        isDashing = false;
    }
}
