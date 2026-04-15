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

    [Header("Sharp Dart Ability")]
    [SerializeField] private GameObject sharpDartPrefab;
    public bool sharpDartUnlocked = false;

    private PlayerStats playerStats;
    private bool isHitStopping = false;
    private bool canAttack = true;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    void Update()
    {
        // Left-click = melee attack
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }

        // "1" key = Blade Beam
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryBladeBeam();
        }

        // "2" key = Mega Slash
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryMegaSlash();
        }

        // "3" key = Sharp Dart
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TrySharpDart();
        }
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
        if (!bladeBeamUnlocked)
            return;

        if (!playerStats.SpendMana(1))
            return;

        Vector3 spawnPos = transform.position + transform.right * bladeBeamOffset;
        Instantiate(bladeBeamPrefab, spawnPos, transform.rotation);
    }

    private void TryMegaSlash()
    {
        if (!megaSlashUnlocked)
            return;

        if (!playerStats.SpendMana(1))
            return;

        Instantiate(megaSlashPrefab, transform.position, Quaternion.identity);
    }

    private void TrySharpDart()
    {
        if (!sharpDartUnlocked)
            return;

        if (!playerStats.SpendMana(1))
            return;

        Instantiate(sharpDartPrefab, transform.position, transform.rotation);

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
