using UnityEngine;
using UnityEngine.UI;

public class SpellbarUI : MonoBehaviour
{
    [Header("Spell Icons")]
    [SerializeField] private Image bladeBeamIcon;
    [SerializeField] private Image megaSlashIcon;
    [SerializeField] private Image slashDashIcon;

    private void Awake()
    {
        // Start hidden
        SetBladeBeamVisible(false);
        SetMegaSlashVisible(false);
        SetSlashDashVisible(false);

        // Optional: if you want them to reflect saved unlocks on load:
        if (SaveGameManager.bladeBeamUnlocked)
            SetBladeBeamVisible(true);

        if (SaveGameManager.megaSlashUnlocked)
            SetMegaSlashVisible(true);

        if (SaveGameManager.slashDashUnlocked)
            SetSlashDashVisible(true);
    }

    public void SetBladeBeamVisible(bool visible)
    {
        if (bladeBeamIcon != null)
            bladeBeamIcon.gameObject.SetActive(visible);
    }

    public void SetMegaSlashVisible(bool visible)
    {
        if (megaSlashIcon != null)
            megaSlashIcon.gameObject.SetActive(visible);
    }

    public void SetSlashDashVisible(bool visible)
    {
        if (slashDashIcon != null)
            slashDashIcon.gameObject.SetActive(visible);
    }
}
