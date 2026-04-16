using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    [Header("Damage Settings")]
    [SerializeField] private int contactDamage = 1;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackStrength = 0.2f;
    [SerializeField] private float knockbackDuration = 0.1f;

    private int currentHealth;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount, Vector2 knockbackDirection)
    {
        currentHealth -= amount;

        StartCoroutine(Knockback(knockbackDirection));

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;

        // Tiny positional twitch
        transform.position += (Vector3)(direction * knockbackStrength);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerStats stats = collision.collider.GetComponent<PlayerStats>();

            if (stats != null)
            {
                // Deal damage
                stats.TakeDamage(contactDamage);

                // Knock the player back slightly
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                collision.transform.position += (Vector3)(direction * knockbackStrength);
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
