using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BossAOE : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 0.15f;

    private CircleCollider2D circle;

    private void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        circle.isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetRadius(float radius)
    {
        if (circle != null)
            circle.radius = radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerStats stats = collision.GetComponent<PlayerStats>();
        if (stats != null)
            stats.TakeDamage(damage);
    }
}
