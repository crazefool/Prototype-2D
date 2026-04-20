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
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null && player != null)
        {
            Vector2 attackDir = player.GetLastAttackDirection();

            // Only grants mana if damage was actually applied
            player.TryDealDamage(enemy, attackDir);

            player.ApplyHitPushback(attackDir);
            player.StartCoroutine(player.HitStop(hitStopDuration));
        }
    }
}
