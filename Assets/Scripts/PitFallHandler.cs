using UnityEngine;

public class PitFallHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isFlying = false;
    [SerializeField] private bool destroyInstantly = true;
    [SerializeField] private float destroyDelay = 0.1f;

    private Enemy enemy;
    private PullableObject pullable;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        pullable = GetComponent<PullableObject>();
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
        if (isFlying)
            return;

        // ⭐ Do not fall while being pulled
        if (enemy != null && enemy.IsBeingPulled)
            return;

        if (pullable != null && pullable.IsBeingPulled)
            return;

        if (destroyInstantly)
            Destroy(gameObject);
        else
            Destroy(gameObject, destroyDelay);
    }
}
