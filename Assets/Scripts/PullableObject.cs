using UnityEngine;

public class PullableObject : MonoBehaviour
{
    public bool IsBeingPulled { get; set; } = false;

    private Rigidbody2D rb;
    private float freezeDelay = 0.1f;
    private float freezeTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsBeingPulled)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            freezeTimer = freezeDelay;
            return;
        }

        if (freezeTimer > 0f)
        {
            freezeTimer -= Time.deltaTime;
            return;
        }

        // Only freeze if not moving from a punch
        PunchableObject punch = GetComponent<PunchableObject>();
        if (punch != null && !punch.enabled)
            return;

        if (rb.linearVelocity.sqrMagnitude < 0.01f)
            rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
