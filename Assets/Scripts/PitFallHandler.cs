using UnityEngine;

public class PitFallHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isFlying = false;
    [SerializeField] private bool destroyInstantly = true;
    [SerializeField] private float destroyDelay = 0.1f;

    [Header("Environment")]
    [SerializeField] private LayerMask platformMask;   // Platform trigger layer

    private Enemy enemy;
    private PullableObject pullable;
    private BaseEnemyAI ai;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        pullable = GetComponent<PullableObject>();
        ai = GetComponent<BaseEnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PitTrigger"))
            return;

        TryFallIntoPit();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PitTrigger"))
            return;

        TryFallIntoPit();
    }

    private void TryFallIntoPit()
    {
        if (ai != null && ai.IsFlying)
            return;

        if (isFlying)
            return;

        if (enemy != null && enemy.IsBeingPulled)
            return;

        if (pullable != null && pullable.IsBeingPulled)
            return;

        // ⭐ NEW: If inside a platform trigger, ignore pit
        if (IsInsidePlatform())
            return;

        if (destroyInstantly)
            Destroy(gameObject);
        else
            Destroy(gameObject, destroyDelay);
    }

    private bool IsInsidePlatform()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, platformMask);
        return hit != null;
    }
}
