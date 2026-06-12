using UnityEngine;

public class HeartChest : MonoBehaviour
{
    [Header("Heart Reward")]
    [SerializeField] private Sprite heartSprite;

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon;

    [Header("Sound")]
    [SerializeField] private AudioClip chestSound;
    private AudioSource audioSource;

    [Header("Destroy Delay")]
    [SerializeField] private float destroyDelay = 0.5f;   // ADDED

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
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

        if (chestSound != null)
            audioSource.PlayOneShot(chestSound);

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

        // ADDED: adjustable delay
        Destroy(gameObject, destroyDelay);
    }
}
