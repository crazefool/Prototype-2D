using System.Collections;
using UnityEngine;

public class PitHandler : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallDelay = 0.15f;

    [Tooltip("How far away from any pit the player must be for a position to be considered safe.")]
    [SerializeField] private float safeRadiusFromPit = 0.4f;

    [Header("Environment")]
    [SerializeField] private LayerMask pitTriggerMask; // assign PitTrigger layer here

    private PlayerStats stats;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDash dash;
    private HookshotController hookshot;

    private bool isFalling = false;

    private Vector3 lastSafePosition;
    private Collider2D currentPitTrigger; // pit we fell into (for future use if needed)

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        dash = GetComponent<PlayerDash>();
        hookshot = GetComponent<HookshotController>();

        lastSafePosition = transform.position;
    }

    void Update()
    {
        // Only update lastSafePosition when:
        // - Not falling
        // - Not being pulled by hookshot
        // - Clearly not near any pit trigger
        if (!isFalling && (hookshot == null || !hookshot.IsPulling))
        {
            bool nearPit = Physics2D.OverlapCircle(transform.position, safeRadiusFromPit, pitTriggerMask) != null;
            if (!nearPit)
            {
                lastSafePosition = transform.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
        {
            TryStartFall(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
        {
            TryStartFall(other);
        }
    }

    private void TryStartFall(Collider2D pit)
    {
        // Don't fall while being pulled by hookshot
        if (hookshot != null && hookshot.IsPulling)
            return;

        if (isFalling)
            return;

        currentPitTrigger = pit;
        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;

        // Lock controls
        movement.SetCanMove(false);
        attack.SetCanAttack(false);
        dash.SetCanDash(false);

        yield return new WaitForSeconds(fallDelay);

        // Damage player
        stats.TakeDamage(1);

        // TELEPORT to last safe position (no offset, no normal, guaranteed safe)
        transform.position = lastSafePosition;

        // Clear pit reference (not strictly needed, but clean)
        currentPitTrigger = null;

        // Unlock controls
        movement.SetCanMove(true);
        attack.SetCanAttack(true);
        dash.SetCanDash(true);

        isFalling = false;
    }

    // Kept for compatibility if you ever want to trigger a fall from dash explicitly
    public void ForceFallFromDash()
    {
        if (!isFalling)
            StartCoroutine(FallRoutine());
    }
}
