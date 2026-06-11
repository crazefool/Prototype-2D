using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum SpellReward
    {
        BladeBeam,
        MegaSlash,
        SlashDash
    }

    [Header("Reward Settings")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private SpellReward spellReward;   // select which spell this chest unlocks

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon;

    private bool isOpened = false;

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
        if (isOpened) return;

        if (other.CompareTag("Player"))
        {
            if (interactIcon != null)
                interactIcon.SetActive(true);
        }

        PlayerAttack pa = other.GetComponentInParent<PlayerAttack>();
        PlayerStats stats = other.GetComponentInParent<PlayerStats>();
        if (pa != null && stats != null)
        {
            OpenChest(pa, stats);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isOpened) return;

        PlayerAttack pa = other.GetComponentInParent<PlayerAttack>();
        PlayerStats stats = other.GetComponentInParent<PlayerStats>();
        if (pa != null && stats != null)
        {
            OpenChest(pa, stats);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactIcon != null)
            interactIcon.SetActive(false);
    }

    private void OpenChest(PlayerAttack attack, PlayerStats stats)
    {
        if (isOpened) return;
        isOpened = true;

        SaveGameManager.MarkChestOpened(gameObject.name);

        if (interactIcon != null)
            interactIcon.SetActive(false);

        // Unlock the correct spell + update save + UI
        SpellbarUI ui = FindFirstObjectByType<SpellbarUI>();

        switch (spellReward)
        {
            case SpellReward.BladeBeam:
                attack.bladeBeamUnlocked = true;
                SaveGameManager.bladeBeamUnlocked = true;
                if (ui != null) ui.SetBladeBeamVisible(true);
                break;

            case SpellReward.MegaSlash:
                attack.megaSlashUnlocked = true;
                SaveGameManager.megaSlashUnlocked = true;
                if (ui != null) ui.SetMegaSlashVisible(true);
                break;

            case SpellReward.SlashDash:
                attack.slashDashUnlocked = true;
                SaveGameManager.slashDashUnlocked = true;
                if (ui != null) ui.SetSlashDashVisible(true);
                break;
        }

        // Visual reward still spawned
        if (rewardPrefab != null)
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);

        // Save AFTER unlock
        SaveGameManager.SaveProgressWithoutPosition(stats);

        Destroy(gameObject);
    }
}
