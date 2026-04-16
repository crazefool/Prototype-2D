using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;
    private bool isDashing = false;

    private Rigidbody2D rb;
    private PlayerMovement movement;
    private Vector2 dashDirection;
    private GhostTrail ghostTrail;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        ghostTrail = GetComponent<GhostTrail>(); // cache reference
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        // Start ghost trail
        if (ghostTrail != null)
            ghostTrail.StartTrail();

        // 1. Try to dash in movement direction
        Vector2 moveInput = movement.GetMovementInput();

        if (moveInput != Vector2.zero)
            dashDirection = moveInput;          // dash where you're moving
        else
            dashDirection = transform.right;    // dash where you're facing

        // Calculate dash duration
        float dashDuration = dashDistance / dashSpeed;

        // Apply velocity
        rb.linearVelocity = dashDirection * dashSpeed;

        // Wait for dash to finish
        yield return new WaitForSeconds(dashDuration);

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Stop ghost trail
        if (ghostTrail != null)
            ghostTrail.StopTrail();

        isDashing = false;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    // -------- FOR SLASHDASH --------
    public void ForceDash(Vector2 direction, float distance, float speed)
    {
        StartCoroutine(ForceDashRoutine(direction, distance, speed));
    }

    private IEnumerator ForceDashRoutine(Vector2 direction, float distance, float speed)
    {
        isDashing = true;

        float dashDuration = distance / speed;

        rb.linearVelocity = direction.normalized * speed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}
