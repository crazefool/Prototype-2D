using UnityEngine;
using System.Collections;

public class DasherEnemyAI : BaseEnemyAI
{
    private enum State { Chase, Anticipate, Dash, Cooldown }
    private State state = State.Chase;

    [Header("Dash Settings")]
    [SerializeField] private float dashRange = 3f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float cooldownTime = 1.2f;

    [Header("Anticipation Settings")]
    [SerializeField] private float anticipationTime = 0.25f;
    [SerializeField] private Color anticipationColor = Color.red;

    [Header("Hitbox")]
    [SerializeField] private GameObject dashHitboxPrefab;

    [Header("Collision")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallCheckDistance = 0.2f;

    private SpriteRenderer sr;
    private Color originalColor;

    private Vector2 dashDir;
    private bool canDash = true;

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

        switch (state)
        {
            case State.Chase:
                ChaseLogic();
                break;

            case State.Anticipate:
                break;

            case State.Dash:
                break;

            case State.Cooldown:
                CooldownLogic();
                break;
        }
    }

    private void ChaseLogic()
    {
        MoveTowardsPlayer();

        if (!canDash)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= dashRange)
            StartCoroutine(AnticipationRoutine());
    }

    private IEnumerator AnticipationRoutine()
    {
        state = State.Anticipate;

        sr.color = anticipationColor;
        yield return new WaitForSeconds(anticipationTime);
        sr.color = originalColor;

        dashDir = (player.position - transform.position).normalized;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        state = State.Dash;
        canDash = false;

        enemy.isInvulnerable = true;

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, true);

        GameObject hitbox = Instantiate(dashHitboxPrefab, transform.position, Quaternion.identity, transform);

        float timer = 0f;

        while (timer < dashDuration)
        {
            if (Physics2D.Raycast(transform.position, dashDir, wallCheckDistance, wallMask))
                break;

            transform.position += (Vector3)(dashDir * dashSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(hitbox);

        enemy.isInvulnerable = false;
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, false);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        state = State.Cooldown;

        yield return new WaitForSeconds(cooldownTime);

        canDash = true;
        state = State.Chase;
    }

    private void CooldownLogic()
    {
        MoveTowardsPlayer();
    }
}
