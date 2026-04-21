using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackStrength = 0.2f;

    public void Initialize(Vector2 dir, float spd, float life)
    {
        direction = dir.normalized;
        speed = spd;
        lifetime = life;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats ps = other.GetComponent<PlayerStats>();
        if (ps != null)
        {
            // Damage
            ps.TakeDamage(1);

            // Knockback (same style as Enemy.OnCollisionEnter2D)
            Vector2 dir = (other.transform.position - transform.position).normalized;
            other.transform.position += (Vector3)(dir * knockbackStrength);

            Destroy(gameObject);
            return;
        }
    }
}
