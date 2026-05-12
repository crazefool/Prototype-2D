using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private PlayerDash dash;

    private bool canMove = true;
    private float speedMultiplier = 1f;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform Face;

    // ⭐ Smoothed aim direction to reduce jitter
    private Vector2 smoothedAim = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        dash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        if (!canMove)
        {
            movementInput = Vector2.zero;
            return;
        }

        // Movement input
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        // Mouse position and raw aim direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mousePos - transform.position).normalized;

        // ⭐ Dead‑zone: ignore tiny flickers around zero
        if (Mathf.Abs(aimDirection.x) < 0.1f) aimDirection.x = 0f;
        if (Mathf.Abs(aimDirection.y) < 0.1f) aimDirection.y = 0f;

        // ⭐ Smooth the aim direction to avoid jitter
        smoothedAim = Vector2.Lerp(smoothedAim, aimDirection, Time.deltaTime * 10f);

        // If smoothedAim becomes zero (e.g. mouse exactly on player), keep last direction
        if (smoothedAim.sqrMagnitude < 0.0001f)
            smoothedAim = Vector2.right;

        // Rotate Face so its right vector points toward the smoothed aim
        float angle = Mathf.Atan2(smoothedAim.y, smoothedAim.x) * Mathf.Rad2Deg;
        Face.rotation = Quaternion.Euler(0, 0, angle);

        // Keep Face orbiting around player at fixed radius
        float orbitRadius = 1.0f;
        Face.localPosition = smoothedAim * orbitRadius;

        // Animation direction based on smoothed aim
        animator.SetFloat("Move_DirecX", smoothedAim.x);
        animator.SetFloat("Move_DirecY", smoothedAim.y);
    }

    void FixedUpdate()
    {
        if (!canMove)
            return;

        if (dash != null && dash.IsDashing())
            return;

        Vector2 newPos = rb.position + movementInput * movementSpeed * speedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        Vector2 movementDelta = newPos - rb.position;
        animator.SetFloat("Move_Speed", movementDelta.magnitude);
    }

    public void SetCanMove(bool value) => canMove = value;
    public void SetMovementMultiplier(float value) => speedMultiplier = value;
    public Vector2 GetMovementInput() => movementInput;
}
