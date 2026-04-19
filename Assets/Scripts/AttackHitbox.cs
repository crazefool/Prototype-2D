using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitStopDuration = 0.05f;

    private PlayerAttack player; 
    private PlayerStats playerStats;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerAttack>();
        playerStats = FindFirstObjectByType<PlayerStats>();
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
            Vector2 attackDir = player.GetLastAttackDirection();

            enemy.TakeDamage(damage, attackDir);

            if (playerStats != null)
                playerStats.GainManaFromHit();

            if (player != null)
                player.ApplyHitPushback(attackDir);

            if (player != null)
                player.StartCoroutine(player.HitStop(hitStopDuration));
        }
    }
}
