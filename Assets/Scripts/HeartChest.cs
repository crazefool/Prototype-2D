using UnityEngine;

public class HeartChest : MonoBehaviour
{
    [Header("Heart Reward")]
    [SerializeField] private Sprite heartSprite;   // Drop your heart sprite here

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon;

    private bool opened = false;

    private void Awake()
    {
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

            PlayerStats stats = other.GetComponent<PlayerStats>();
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

        if (interactIcon != null)
            interactIcon.SetActive(false);

        // Give heart upgrade
        stats.IncreaseMaxHealth(1);

        // Spawn heart sprite above chest
        if (heartSprite != null)
        {
            GameObject go = new GameObject("HeartReward");
            go.transform.position = transform.position;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = heartSprite;
            sr.sortingOrder = 50;

            // ⭐ Remove after 1.2 seconds
            Destroy(go, 1.2f);
        }

        Destroy(gameObject);
    }
}
