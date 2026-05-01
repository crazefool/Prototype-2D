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

    // ⭐ RESTORED PUBLIC GETTERS (UI_Mana needs these)
    public int HitsPerMana => hitsPerMana;
    public int HitCounter => hitCounter;

    [Header("Heal Settings")]
    [SerializeField] private float healChargeTime = 1.2f;
    [SerializeField] private GameObject healChargeEffectPrefab;
    [SerializeField] private GameObject healBurstEffectPrefab;

    private GameObject activeChargeEffect;
    private bool isHealing = false;
    public bool IsHealing => isHealing;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private Coroutine invincibleRoutine;

    public bool IsInvincible => isInvincible;

    private HitFlash hitFlash;

    void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentMana = 0;
        hitFlash = GetComponent<HitFlash>();
    }

    // ---------------- HEALTH ----------------

    public void TakeDamage(int amount)
    {
        if (isInvincible)
            return;

        if (hitFlash != null)
            hitFlash.Flash();

        InterruptHeal();

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        else
        {
            if (invincibleRoutine != null)
                StopCoroutine(invincibleRoutine);

            invincibleRoutine = StartCoroutine(InvincibilityFrames());
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

    // ⭐ RESTORED FOR HOOKSHOT + SLASHDASH
    public void SetInvincible(bool value)
    {
        isInvincible = value;

        if (!value && invincibleRoutine != null)
        {
            StopCoroutine(invincibleRoutine);
            invincibleRoutine = null;
        }
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

    // ---------------- NEW HEAL SYSTEM ----------------

    public void BeginHealCharge()
    {
        if (isHealing || CurrentMana < 1)
            return;

        StartCoroutine(HealChargeRoutine());
    }

    public void CancelHealCharge()
    {
        isHealing = false;
    }

    private IEnumerator HealChargeRoutine()
    {
        isHealing = true;

        var movement = FindFirstObjectByType<PlayerMovement>();
        var attack = FindFirstObjectByType<PlayerAttack>();

        movement.SetCanMove(false);
        attack.SetCanAttack(false);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        activeChargeEffect = Instantiate(healChargeEffectPrefab, transform.position, Quaternion.identity, transform);

        float timer = 0f;

        while (timer < healChargeTime)
        {
            if (!isHealing)
            {
                CleanupChargeEffects();
                yield break;
            }

            timer += Time.deltaTime;

            float progress = timer / healChargeTime;
            float scale = Mathf.Lerp(0.5f, 1.3f, progress);

            if (activeChargeEffect != null)
                activeChargeEffect.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        SpendMana(1);
        Heal(1);

        if (healBurstEffectPrefab != null)
            Instantiate(healBurstEffectPrefab, transform.position, Quaternion.identity);

        CleanupChargeEffects();
    }

    private void CleanupChargeEffects()
    {
        if (activeChargeEffect != null)
            Destroy(activeChargeEffect);

        var movement = FindFirstObjectByType<PlayerMovement>();
        var attack = FindFirstObjectByType<PlayerAttack>();

        movement.SetCanMove(true);
        attack.SetCanAttack(true);

        isHealing = false;
    }

    public void InterruptHeal()
    {
        if (!isHealing)
            return;

        isHealing = false;
        CleanupChargeEffects();
    }
}
