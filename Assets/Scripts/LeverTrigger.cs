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

    public void ActivateLever()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

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

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
