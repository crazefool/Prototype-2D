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
    public int HitsPerMana => hitsPerMana;
    public int HitCounter => hitCounter;

    [Header("Heal Settings")]
    [SerializeField] private float healChargeTime = 1.2f;
    private bool isHealing = false;
    public bool IsHealing => isHealing;

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

        // Interrupt healing if charging
        InterruptHeal();

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

        FindFirstObjectByType<UI_Health>().UpdateHearts();
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;

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

        FindFirstObjectByType<UI_Mana>().UpdateMana();
    }

    public void AddMana(int amount)
    {
        CurrentMana += amount;
        if (CurrentMana > maxMana)
            CurrentMana = maxMana;

        FindFirstObjectByType<UI_Mana>().UpdateMana();
    }

    public bool SpendMana(int amount)
    {
        if (CurrentMana < amount)
            return false;

        CurrentMana -= amount;

        FindFirstObjectByType<UI_Mana>().UpdateMana();
        return true;
    }

    // ---------------- HEAL ----------------

    public IEnumerator HealRoutine()
    {
        if (isHealing)
            yield break;

        if (CurrentMana < 1)
            yield break;

        isHealing = true;

        // Disable movement + attacks
        FindFirstObjectByType<PlayerMovement>().SetCanMove(false);
        FindFirstObjectByType<PlayerAttack>().SetCanAttack(false);

        float timer = 0f;

        while (timer < healChargeTime)
        {
            if (!isHealing)
            {
                // Restore control if interrupted
                FindFirstObjectByType<PlayerMovement>().SetCanMove(true);
                FindFirstObjectByType<PlayerAttack>().SetCanAttack(true);
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        SpendMana(1);
        Heal(1);

        // Restore control
        FindFirstObjectByType<PlayerMovement>().SetCanMove(true);
        FindFirstObjectByType<PlayerAttack>().SetCanAttack(true);

        isHealing = false;
    }

    public void InterruptHeal()
    {
        if (!isHealing)
            return;

        isHealing = false;

        // Restore control
        FindFirstObjectByType<PlayerMovement>().SetCanMove(true);
        FindFirstObjectByType<PlayerAttack>().SetCanAttack(true);
    }
}
