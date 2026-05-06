using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossHazardArea : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private float slowMultiplier = 0.4f;
    [SerializeField] private float lifetime = 4f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.SetMovementMultiplier(slowMultiplier);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.SetMovementMultiplier(1f);
    }
}
