using UnityEngine;

public class ShellEnemyAI : BaseEnemyAI
{
    [Header("Shell Settings")]
    [SerializeField] private GameObject shellObject;

    private bool hasShell = true;

    protected override void Awake()
    {
        base.Awake();

        // Enemy starts invulnerable until shell is removed
        enemy.isInvulnerable = true;

        // Ensure shell collider is active and hookable
        if (shellObject != null)
        {
            Collider2D col = shellObject.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = true;
        }
    }

    /// <summary>
    /// Called by HookshotTarget.DetachPart() when the shell is hit by the hookshot.
    /// </summary>
    public void BreakShell()
    {
        if (!hasShell) return;

        hasShell = false;

        // Enemy becomes vulnerable
        enemy.isInvulnerable = false;

        // Optional: small stun for feedback
        enemy.Stun(0.4f);

        // Shell is already detached by HookshotTarget
        // We just ensure it stays hookable and physical
        if (shellObject != null)
        {
            Rigidbody2D rb = shellObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;   // ⭐ FIXED
                rb.gravityScale = 0f;
            }

            Collider2D col = shellObject.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = true;
        }
    }

    void Update()
    {
        // Enemy only moves after shell is removed
        if (!hasShell && PlayerInRange())
            MoveTowardsPlayer();
    }
}
