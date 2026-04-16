using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    public int CurrentHealth { get; private set; }

    [Header("Mana Settings")]
    [SerializeField] private int maxMana = 3;
    public int CurrentMana { get; private set; }
    public int MaxMana => maxMana;

    [SerializeField] private int hitsPerMana = 3;
    private int hitCounter = 0;

    private bool isInvincible = false;
    [SerializeField] private float invincibilityDuration = 0.5f;

    void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentMana = 0;
    }

    // ---------------- HEALTH ----------------

    public void TakeDamage(int amount)
    {
        if (isInvincible)
            return;

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }

        // Update HP UI
        
        FindFirstObjectByType<UI_Health>().UpdateHearts();

    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;

        
        // Update HP UI
        FindFirstObjectByType<UI_Health>().UpdateHearts();

    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    private void Die()
    {
        Debug.Log("Player died");
        // TODO: Respawn or death animation
    }

    // ---------------- MANA ----------------

    public void GainManaFromHit()
    {
        hitCounter++;

        if (hitCounter >= hitsPerMana)
        {
            hitCounter = 0;
            AddMana(1);
        }
    }

    public void AddMana(int amount)
    {
        CurrentMana += amount;
        if (CurrentMana > maxMana)
            CurrentMana = maxMana;

        // TODO: Update MP UI
    }

    public bool SpendMana(int amount)
    {
        if (CurrentMana < amount)
            return false;

        CurrentMana -= amount;

        // TODO: Update MP UI
        return true;
    }
}
