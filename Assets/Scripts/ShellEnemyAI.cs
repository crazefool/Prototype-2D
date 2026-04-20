using UnityEngine;

public class ShellEnemyAI : BaseEnemyAI
{
    [Header("Shell Settings")]
    [SerializeField] private GameObject shellObject;

    private bool hasShell = true;

    private Rigidbody2D shellRB;
    private Collider2D shellCol;

    protected override void Awake()
    {
        base.Awake();

        // Enemy starts invulnerable until shell is removed
        enemy.isInvulnerable = true;

        if (shellObject != null)
        {
            shellRB = shellObject.GetComponent<Rigidbody2D>();
            shellCol = shellObject.GetComponent<Collider2D>();

            // ⭐ SHELL ATTACHED STATE
            // Shell should NOT collide with player or enemies
            // Shell should NOT be pushed by physics
            if (shellRB != null)
            {
                shellRB.bodyType = RigidbodyType2D.Kinematic;
                shellRB.gravityScale = 0f;
            }

            if (shellCol != null)
            {
                shellCol.isTrigger = true; // hookshot can still detect it
                shellCol.enabled = true;
            }
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
            shellCol.isTrigger = false; // now it becomes a real physics object
            shellCol.enabled = true;
        }
    }

    void Update()
    {
        if (!PlayerInRange())
            return;

        MoveTowardsPlayer();
    }
}
