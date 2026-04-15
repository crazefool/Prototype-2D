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
    private PlayerMovement playerMovement; // your movement script

    private void Start()
    {
        playerAttack = FindFirstObjectByType<PlayerAttack>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        // Make player invincible
        playerStats.SetInvincible(true);

        // ALWAYS dash in the direction the player is facing
        Vector2 dashDir = transform.right.normalized;

        // Start the dash
        playerMovement.Dash(dashDir, dashDistance, dashSpeed);

        // Destroy this hitbox after dash duration
        float dashTime = dashDistance / dashSpeed;
        Destroy(gameObject, dashTime);
    }

    private void OnDestroy()
    {
        // Remove invincibility when dash ends
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
