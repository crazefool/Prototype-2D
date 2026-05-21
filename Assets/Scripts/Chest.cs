using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Reward Settings")]
    [SerializeField] private GameObject rewardPrefab;

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon;

    private bool isOpened = false;

    private void Awake()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;

        // Show icon when player is close
        if (other.CompareTag("Player"))
        {
            if (interactIcon != null)
                interactIcon.SetActive(true);
        }

        // Sword hitbox opens chest instantly
        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            OpenChest();
        }
    }

    // ⭐ FIX: If the sword hitbox spawns already overlapping the chest,
    // OnTriggerEnter2D will NOT fire. OnTriggerStay2D ensures instant reaction.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isOpened) return;

        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            OpenChest();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactIcon != null)
            interactIcon.SetActive(false);
    }

    private void OpenChest()
    {
        isOpened = true;

        if (interactIcon != null)
            interactIcon.SetActive(false);

        // Spawn reward above chest
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);
        }

        // Remove chest
        Destroy(gameObject);
    }
}
