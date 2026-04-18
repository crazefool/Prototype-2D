using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;   // REQUIRED for hookshot
    private bool isDashing = false;

    private Rigidbody2D rb;
    private PlayerMovement movement;
    private Vector2 dashDirection;
    private GhostTrail ghostTrail;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        ghostTrail = GetComponent<GhostTrail>();
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

        if (ghostTrail != null)
            ghostTrail.StartTrail();

        Vector2 moveInput = movement.GetMovementInput();

        if (moveInput != Vector2.zero)
            dashDirection = moveInput;
        else
            dashDirection = transform.right;

        float dashDuration = dashDistance / dashSpeed;

        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;

        if (ghostTrail != null)
            ghostTrail.StopTrail();

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

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

    // ⭐ REQUIRED BY HOOKSHOT
    public void SetCanDash(bool value)
    {
        canDash = value;
    }
}
