using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitStopDuration = 0.05f;

    private PlayerAttack player; // cached reference
    private PlayerStats playerStats; // NEW

    void Awake()
    {
        player = FindFirstObjectByType<PlayerAttack>();
        playerStats = FindFirstObjectByType<PlayerStats>(); // NEW
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, knockbackDir);

            // ⭐ Gain MP from hitting an enemy
            if (playerStats != null)
                playerStats.GainManaFromHit();

            // Hit stop
            if (player != null)
                player.StartCoroutine(player.HitStop(hitStopDuration));
        }
    }
}
