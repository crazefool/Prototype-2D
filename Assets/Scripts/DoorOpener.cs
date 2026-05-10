using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void OpenDoor()
    {
        // Play animation if assigned
        if (animator != null)
            animator.SetTrigger("Open");

        // Disable collider so player can walk through
        if (doorCollider != null)
            doorCollider.enabled = false;

        // Hide the visual door
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }
}
