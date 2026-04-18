using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private PlayerDash dash;

    private bool canMove = true;

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

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void FixedUpdate()
    {
        if (!canMove)
            return;

        if (dash != null && dash.IsDashing())
            return;

        rb.MovePosition(rb.position + movementInput * movementSpeed * Time.fixedDeltaTime);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public Vector2 GetMovementInput()
    {
        return movementInput;
    }
}
