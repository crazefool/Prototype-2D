using UnityEngine;

public class MegaSlash : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float lifetime = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector2 dir = (enemy.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, dir * knockbackForce);
        }
    }
}
