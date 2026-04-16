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

    private void Start()
    {
        playerAttack = FindFirstObjectByType<PlayerAttack>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerDash = FindFirstObjectByType<PlayerDash>();

        if (playerStats != null)
            playerStats.SetInvincible(true);

        // Always dash in facing direction
        Vector2 dashDir = transform.right.normalized;

        if (playerDash != null)
            playerDash.ForceDash(dashDir, dashDistance, dashSpeed);

        // Keep hitbox alive for entire dash
        float dashTime = dashDistance / dashSpeed;
        Destroy(gameObject, dashTime);
    }

    private void OnDestroy()
    {
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
