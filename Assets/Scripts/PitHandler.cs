using System.Collections;
using UnityEngine;

public class PitHandler : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallDelay = 0.15f;
    [SerializeField] private float respawnOffset = 0.6f;

    private PlayerStats stats;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDash dash;
    private HookshotController hookshot;

    private bool isOverPit = false;
    private bool isFalling = false;

    private Vector3 lastSafePosition;
    private Vector2 lastMoveDir = Vector2.up; // default up if standing still

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
        // Track last movement direction (for safe respawn offset)
        Vector2 input = movement.GetMovementInput();
        if (input.sqrMagnitude > 0.01f)
            lastMoveDir = input.normalized;

        // Update last safe ground position
        if (!isOverPit && !isFalling)
            lastSafePosition = transform.position;

        // If over pit and not falling and not being pulled → fall
        if (isOverPit && !isFalling && !hookshot.IsPulling)
            StartCoroutine(FallRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
            isOverPit = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
            isOverPit = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PitTrigger"))
            isOverPit = false;
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;

        movement.SetCanMove(false);
        attack.SetCanAttack(false);
        dash.SetCanDash(false);

        yield return new WaitForSeconds(fallDelay);

        // Damage player
        stats.TakeDamage(1);

        // Respawn slightly away from the pit, opposite of approach direction
        Vector3 offset = -(Vector3)(lastMoveDir * respawnOffset);
        transform.position = lastSafePosition + offset;

        movement.SetCanMove(true);
        attack.SetCanAttack(true);
        dash.SetCanDash(true);

        isFalling = false;
    }
}
