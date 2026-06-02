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
        // ⭐ If chest was already opened in saved progress → remove it
        if (SaveGameManager.IsChestOpened(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;

        if (other.CompareTag("Player"))
        {
            if (interactIcon != null)
                interactIcon.SetActive(true);
        }

        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            OpenChest();
        }
    }

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
        if (isOpened) return;
        isOpened = true;

        // ⭐ Save chest opened
        SaveGameManager.MarkChestOpened(gameObject.name);

        if (interactIcon != null)
            interactIcon.SetActive(false);

        if (rewardPrefab != null)
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
