using UnityEngine;

public class EnemyDashHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;

    private bool hasHit = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit)
            return;

        PlayerStats player = collision.collider.GetComponent<PlayerStats>();
        if (player != null)
        {
            hasHit = true;

            player.TakeDamage(damage);

            Vector2 dir = (player.transform.position - transform.position).normalized;
            player.transform.position += (Vector3)(dir * knockbackForce);

            PlayerAttack pa = FindFirstObjectByType<PlayerAttack>();
            if (pa != null)
                pa.StartCoroutine(pa.HitStop(0.05f));
        }
    }
}
