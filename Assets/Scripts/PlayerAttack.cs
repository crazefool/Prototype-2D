using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee Attack")]
    [SerializeField] private GameObject attackHitboxPrefab;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float attackOffset = 1f;

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
    private bool isHitStopping = false;
    private bool canAttack = true;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            TryBladeBeam();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TryMegaSlash();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            TrySlashDash();

        if (Input.GetKey(KeyCode.Alpha4))
            TryHeal();
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        Vector3 spawnPos = transform.position + transform.right * attackOffset;
        Instantiate(attackHitboxPrefab, spawnPos, transform.rotation);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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

        Instantiate(megaSlashPrefab, transform.position, Quaternion.identity);
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
