using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee Attack")]
    [SerializeField] private GameObject attackHitboxPrefab;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float attackOffset = 1f;

    [Header("Attack Feedback")]
    [SerializeField] private float hitPushbackForce = 2f;
    [SerializeField] private float hitPushbackDuration = 0.05f;

    [Header("Blade Beam Ability")]
    [SerializeField] private GameObject bladeBeamPrefab;
    [SerializeField] private float bladeBeamOffset = 1f;
    public bool bladeBeamUnlocked = false;

    [Header("Mega Slash Ability")]
    [SerializeField] private GameObject megaSlashPrefab;
    public bool megaSlashUnlocked = false;

    [Header("Slash Dash Ability")]
    [SerializeField] private GameObject slashDashPrefab;
    public bool slashDashUnlocked = false;

    private PlayerStats playerStats;
    private PlayerMovement movement;
    private Rigidbody2D rb;

    private HookshotController hookshot;   // ⭐ REQUIRED

    private bool isHitStopping = false;
    private bool canAttack = true;

    private bool pendingAttack = false;
    private Vector2 lastAttackDirection;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        // ⭐ FIXED: Always finds hookshot, even if on parent/child
        hookshot = FindFirstObjectByType<HookshotController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
            pendingAttack = true;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            TryBladeBeam();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TryMegaSlash();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            TrySlashDash();

        if (Input.GetKey(KeyCode.Alpha4))
            TryHeal();
    }

    void LateUpdate()
    {
        if (pendingAttack)
        {
            pendingAttack = false;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        // ⭐ PERFECT HOOKSHOT CANCEL
        if (hookshot != null && hookshot.IsPulling)
            hookshot.CancelHookshot();

        canAttack = false;

        lastAttackDirection = transform.right.normalized;

        Vector3 spawnPos = transform.position + transform.right * attackOffset;
        Instantiate(attackHitboxPrefab, spawnPos, transform.rotation);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public Vector2 GetLastAttackDirection()
    {
        return lastAttackDirection;
    }

    private void TryBladeBeam()
    {
        if (!bladeBeamUnlocked || !canAttack)
            return;

        if (!playerStats.SpendMana(1))
            return;

        Vector3 spawnPos = transform.position + transform.right * bladeBeamOffset;
        Instantiate(bladeBeamPrefab, spawnPos, transform.rotation);
    }

    private void TryMegaSlash()
    {
        if (!megaSlashUnlocked || !canAttack)
            return;

        if (!playerStats.SpendMana(1))
            return;

        Instantiate(megaSlashPrefab, transform.position, transform.rotation);
    }

    private void TrySlashDash()
    {
        if (!slashDashUnlocked || !canAttack)
            return;

        if (!playerStats.SpendMana(1))
            return;

        Instantiate(slashDashPrefab, transform.position, transform.rotation, transform);
    }

    private void TryHeal()
    {
        if (!canAttack)
            return;

        if (playerStats.CurrentMana < 1)
            return;

        StartCoroutine(playerStats.HealRoutine());
    }

    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }

    public void ApplyHitPushback(Vector2 attackDirection)
    {
        StartCoroutine(HitPushbackRoutine(attackDirection));
    }

    private IEnumerator HitPushbackRoutine(Vector2 attackDirection)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.Sleep();
        rb.WakeUp();

        rb.AddForce(-attackDirection * hitPushbackForce, ForceMode2D.Impulse);

        movement.SetCanMove(false);
        yield return new WaitForSeconds(hitPushbackDuration);
        movement.SetCanMove(true);
    }

    public IEnumerator HitStop(float duration)
    {
        if (isHitStopping)
            yield break;

        isHitStopping = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isHitStopping = false;
    }
}
