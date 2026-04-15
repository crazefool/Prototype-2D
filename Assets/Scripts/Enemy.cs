using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackStrength = 0.2f;   // small twitch
    [SerializeField] private float knockbackDuration = 0.1f;   // short freeze

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

        // Start knockback twitch
        StartCoroutine(Knockback(knockbackDirection));

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;

        // Apply tiny positional twitch (not physics)
        transform.position += (Vector3)(direction * knockbackStrength);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
