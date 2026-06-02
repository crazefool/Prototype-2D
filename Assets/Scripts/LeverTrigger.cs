using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public enum LeverAction
    {
        ActivateObjects,
        DeactivateObjects
    }

    [Header("Lever Behavior")]
    [SerializeField] private LeverAction action = LeverAction.ActivateObjects;

    [Header("Targets (put multiple objects here)")]
    [SerializeField] private GameObject[] targetObjects;

    [Header("Optional: One-time use")]
    [SerializeField] private bool disableAfterUse = true;

    private bool hasTriggered = false;

    private void Awake()
    {
        // ⭐ If lever was already used in saved progress → apply instantly
        if (SaveGameManager.IsLeverUsed(gameObject.name))
        {
            ApplyLeverEffect();
            if (disableAfterUse)
                gameObject.SetActive(false);
            hasTriggered = true;
        }
    }

    public void ActivateLever()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        // ⭐ Save lever used
        SaveGameManager.MarkLeverUsed(gameObject.name);

        ApplyLeverEffect();

        if (disableAfterUse)
            gameObject.SetActive(false);
    }

    private void ApplyLeverEffect()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;

            switch (action)
            {
                case LeverAction.ActivateObjects:
                    obj.SetActive(true);
                    break;

                case LeverAction.DeactivateObjects:
                    obj.SetActive(false);
                    break;
            }
        }
    }
}
