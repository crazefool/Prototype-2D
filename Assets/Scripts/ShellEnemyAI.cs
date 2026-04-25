using UnityEngine;

public class ShellEnemyAI : BaseEnemyAI
{
    [Header("Shell Settings")]
    [SerializeField] private GameObject shellObject;

    private bool hasShell = true;

    private Rigidbody2D shellRB;
    private Collider2D shellCol;
    private PunchableObject punchable;

    protected override void Awake()
    {
        base.Awake();

        // Enemy starts invulnerable until shell is removed
        enemy.isInvulnerable = true;

        if (shellObject != null)
        {
            shellRB = shellObject.GetComponent<Rigidbody2D>();
            shellCol = shellObject.GetComponent<Collider2D>();
            punchable = shellObject.GetComponent<PunchableObject>();

            // ⭐ SHELL ATTACHED STATE
            if (shellRB != null)
            {
                shellRB.bodyType = RigidbodyType2D.Kinematic;
                shellRB.gravityScale = 0f;
            }

            if (shellCol != null)
            {
                shellCol.isTrigger = true;
                shellCol.enabled = true;
            }

            // ⭐ NEW: Mark shell as attached so it cannot be punched
            if (punchable != null)
                punchable.IsAttached = true;
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

        // Optional stun for feedback
        enemy.Stun(0.4f);

        // ⭐ SHELL DETACHED STATE
        if (shellRB != null)
        {
            shellRB.bodyType = RigidbodyType2D.Dynamic;
            shellRB.gravityScale = 0f;
        }

        if (shellCol != null)
        {
            shellCol.isTrigger = false;
            shellCol.enabled = true;
        }

        // ⭐ NEW: Shell is now punchable
        if (punchable != null)
            punchable.IsAttached = false;
    }

    void Update()
    {
        if (!PlayerInRange())
            return;

        MoveTowardsPlayer();
    }
}
