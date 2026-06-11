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
    private HookshotController hookshot;

    private bool isHitStopping = false;
    private bool canAttack = true;
    private bool pendingAttack = false;
    private Vector2 lastAttackDirection;

    [SerializeField] private Transform Face;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        hookshot = FindFirstObjectByType<HookshotController>();
    }

    void OnEnable()
    {
        // Load saved spell unlocks whenever the scene loads
        ApplySavedSpellUnlocks();
    }

    public void ApplySavedSpellUnlocks()
    {
        bladeBeamUnlocked = SaveGameManager.bladeBeamUnlocked;
        megaSlashUnlocked = SaveGameManager.megaSlashUnlocked;
        slashDashUnlocked = SaveGameManager.slashDashUnlocked;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
            pendingAttack = true;

        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.LeftShift)))
            TryBladeBeam();

        if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Q)))
            TryMegaSlash();

        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.E)))
            TrySlashDash();

        if (Input.GetKeyDown(KeyCode.R))
            playerStats.BeginHealCharge();

        if (Input.GetKeyUp(KeyCode.R))
            playerStats.CancelHealCharge();
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
        if (hookshot != null && hookshot.IsPulling)
            hookshot.CancelHookshot();

        canAttack = false;

        lastAttackDirection = Face.right.normalized;

        Vector3 spawnPos = transform.position + (Vector3)lastAttackDirection * attackOffset;
        Instantiate(
            attackHitboxPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, Mathf.Atan2(lastAttackDirection.y, lastAttackDirection.x) * Mathf.Rad2Deg)
        );

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public Vector2 GetLastAttackDirection() => lastAttackDirection;

    public void TryDealDamage(Enemy enemy, Vector2 attackDirection)
    {
        bool didDamage = enemy.TakeDamage(1, attackDirection);
        if (didDamage)
            playerStats.GainManaFromHit();
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
        if (isHitStopping) yield break;

        isHitStopping = true;
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isHitStopping = false;
    }

    private void TryBladeBeam()
    {
        if (!bladeBeamUnlocked || !canAttack) return;
        if (!playerStats.SpendMana(1)) return;

        Vector3 spawnPos = transform.position + (Vector3)Face.right * bladeBeamOffset;
        Instantiate(
            bladeBeamPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, Mathf.Atan2(Face.right.y, Face.right.x) * Mathf.Rad2Deg)
        );
    }

    private void TryMegaSlash()
    {
        if (!megaSlashUnlocked || !canAttack) return;
        if (!playerStats.SpendMana(1)) return;

        Instantiate(megaSlashPrefab, transform.position, Quaternion.identity);
    }

    private void TrySlashDash()
    {
        if (!slashDashUnlocked || !canAttack) return;
        if (!playerStats.SpendMana(1)) return;

        Vector2 dir = Face.right.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Instantiate(
            slashDashPrefab,
            transform.position,
            Quaternion.Euler(0, 0, angle)
        );
    }

    public void SetCanAttack(bool value) => canAttack = value;
}
