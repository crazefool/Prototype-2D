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

    // ⭐ Changed from private → public (fixes PlayerDash access)
    [SerializeField] public Transform Face;

    // Smoothed aim direction to reduce jitter
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

        // Mouse world position
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Direction from player to mouse
        Vector2 aimDirection = (mouseWorld - transform.position).normalized;

        // Smooth the aim direction to avoid jitter
        smoothedAim = Vector2.Lerp(smoothedAim, aimDirection, Time.deltaTime * 10f);

        // If smoothedAim becomes zero (e.g. mouse exactly on player), keep last direction
        if (smoothedAim.sqrMagnitude < 0.0001f)
            smoothedAim = Vector2.right;

        // ⭐ Keep Face orbiting around player at fixed radius (WORLD space)
        float orbitRadius = 1.0f;
        Face.position = transform.position + (Vector3)smoothedAim * orbitRadius;

        // Rotate Face so its right vector points toward the smoothed aim
        float angle = Mathf.Atan2(smoothedAim.y, smoothedAim.x) * Mathf.Rad2Deg;
        Face.rotation = Quaternion.Euler(0, 0, angle);

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
