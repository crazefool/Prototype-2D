using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private float hitStopDuration = 0.05f;

    private PlayerAttack player;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerAttack>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 attackDir = player.GetLastAttackDirection();

        // ⭐ 1. Punchable objects
        PunchableObject punchable = collision.GetComponent<PunchableObject>();
        if (punchable != null)
        {
            punchable.Punch(attackDir);
            return;
        }

        // ⭐ 2. Enemies
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            player.TryDealDamage(enemy, attackDir);
            player.ApplyHitPushback(attackDir);
            player.StartCoroutine(player.HitStop(hitStopDuration));
        }
    }
}
