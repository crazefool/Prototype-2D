using UnityEngine;

public class HeartChest : MonoBehaviour
{
    [Header("Heart Reward")]
    [SerializeField] private Sprite heartSprite;

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon;

    private bool opened = false;

    private void Awake()
    {
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
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            if (interactIcon != null)
                interactIcon.SetActive(true);

            PlayerStats stats = other.GetComponentInParent<PlayerStats>();
            if (stats != null)
            {
                OpenChest(stats);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (opened) return;

        if (other.CompareTag("Player") && interactIcon != null)
            interactIcon.SetActive(false);
    }

    private void OpenChest(PlayerStats stats)
    {
        if (opened) return;
        opened = true;

        SaveGameManager.MarkChestOpened(gameObject.name);

        if (interactIcon != null)
            interactIcon.SetActive(false);

        stats.IncreaseMaxHealth(1);

        if (heartSprite != null)
        {
            GameObject go = new GameObject("HeartReward");
            go.transform.position = transform.position;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = heartSprite;
            sr.sortingOrder = 50;

            Destroy(go, 1.2f);
        }

        SaveGameManager.SaveProgressWithoutPosition(stats);

        Destroy(gameObject);
    }
}
