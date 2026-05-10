using System.Collections;
using UnityEngine;

public class PlayerPitHandler : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallDelay = 0.15f;

    [Tooltip("How far away from any pit the player must be for a position to be considered safe.")]
    [SerializeField] private float safeRadiusFromPit = 0.4f;

    [Header("Environment")]
    [SerializeField] private LayerMask pitTriggerMask;   // PitTrigger layer
    [SerializeField] private LayerMask platformMask;     // Platform layer (trigger)

    private PlayerStats stats;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDash dash;
    private HookshotController hookshot;

    private bool isFalling = false;

    private Vector3 lastSafePosition;

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
        // ⭐ NEW: Do NOT update safe position while dashing
        if (dash != null && dash.IsDashing())
            return;

        if (!isFalling && (hookshot == null || !hookshot.IsPulling))
        {
            bool nearPit = Physics2D.OverlapCircle(transform.position, safeRadiusFromPit, pitTriggerMask) != null;

            // ⭐ If inside a platform trigger, treat as safe
            if (!nearPit || IsInsidePlatform())
            {
                lastSafePosition = transform.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
            TryStartFall();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
            TryStartFall();
    }

    private void TryStartFall()
    {
        if (hookshot != null && hookshot.IsPulling)
            return;

        if (isFalling)
            return;

        // ⭐ NEW: Ignore pit while dashing
        if (dash != null && dash.IsDashing())
            return;

        // ⭐ Ignore pit if standing on a platform
        if (IsInsidePlatform())
            return;

        StartCoroutine(FallRoutine());
    }

    private bool IsInsidePlatform()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, platformMask);
        return hit != null;
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;

        movement.SetCanMove(false);
        attack.SetCanAttack(false);
        dash.SetCanDash(false);

        yield return new WaitForSeconds(fallDelay);

        stats.TakeDamage(1);

        transform.position = lastSafePosition;

        movement.SetCanMove(true);
        attack.SetCanAttack(true);
        dash.SetCanDash(true);

        isFalling = false;
    }

    public void ForceFallFromDash()
    {
        if (!isFalling)
            StartCoroutine(FallRoutine());
    }
}
