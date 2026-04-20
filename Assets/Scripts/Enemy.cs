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
    private bool isStunned = false;

    // Shell enemies use this
    public bool isInvulnerable = false;

    public bool IsStunned => isStunned;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    // Now returns bool so callers know if damage was applied
    public bool TakeDamage(int amount, Vector2 knockbackDirection)
    {
        if (isInvulnerable)
            return false;

        currentHealth -= amount;

        StartCoroutine(Knockback(knockbackDirection));

        if (currentHealth <= 0)
        {
            Die();
        }

        return true;
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;

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
        // Enemy cannot deal contact damage while stunned
        if (isStunned) return;

        if (collision.collider.CompareTag("Player"))
        {
            PlayerStats stats = collision.collider.GetComponent<PlayerStats>();

            if (stats != null)
            {
                stats.TakeDamage(contactDamage);

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
