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

    public bool isInvulnerable = false;

    // ⭐ NEW: Is this enemy a boss?
    // If true → Enemy.cs will NOT destroy it on death.
    // Boss script handles destruction instead.
    public bool isBoss = false;

    public bool IsBeingPulled { get; set; } = false;

    public bool IsStunned => isStunned;

    private HitFlash hitFlash;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        hitFlash = GetComponent<HitFlash>();
    }

    public bool TakeDamage(int amount, Vector2 knockbackDirection)
    {
        if (isInvulnerable)
            return false;

        if (hitFlash != null)
            hitFlash.Flash();

        currentHealth -= amount;

        StartCoroutine(Knockback(knockbackDirection));

        if (currentHealth <= 0)
        {
            // ⭐ Normal enemies die here
            if (!isBoss)
                Destroy(gameObject);

            // ⭐ Bosses do NOT get destroyed here
            return true;
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
}
