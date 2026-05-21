using UnityEngine;

public class SlashDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashSpeed = 20f;

    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;

    private PlayerAttack playerAttack;
    private PlayerStats playerStats;
    private PlayerDash playerDash;

    private int playerLayer;
    private int enemyLayer;

    private void Start()
    {
        playerAttack = FindFirstObjectByType<PlayerAttack>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerDash = FindFirstObjectByType<PlayerDash>();

        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");

        // Disable collisions between player and enemies during dash
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // Player is invincible during dash
        if (playerStats != null)
            playerStats.SetInvincible(true);

        // Dash direction is already set by PlayerAttack rotation
        Vector2 dashDir = transform.right.normalized;

        if (playerDash != null)
            playerDash.ForceDash(dashDir, dashDistance, dashSpeed);

        float dashTime = dashDistance / dashSpeed;
        Destroy(gameObject, dashTime);
    }

    private void Update()
    {
        // ⭐ Keep hitbox following the player's position
        if (playerAttack != null)
            transform.position = playerAttack.transform.position;
    }

    private void OnDestroy()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        if (playerStats != null)
            playerStats.SetInvincible(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector2 dir = (enemy.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, dir * knockbackForce);

            if (playerAttack != null)
                playerAttack.StartCoroutine(playerAttack.HitStop(0.05f));
        }
    }
}
