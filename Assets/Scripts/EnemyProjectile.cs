using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;

    // Called by the ranged enemy when spawning the projectile
    public void Initialize(Vector2 dir, float spd, float life)
    {
        direction = dir.normalized;
        speed = spd;
        lifetime = life;
    }

    void Update()
    {
        // Move the projectile
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Lifetime countdown
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damage player if hit
        PlayerStats ps = other.GetComponent<PlayerStats>();
        if (ps != null)
        {
            ps.TakeDamage(1);
            Destroy(gameObject);
            return;
        }
    }
}
