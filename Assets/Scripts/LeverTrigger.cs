using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public enum LeverAction
    {
        ActivateObject,
        DeactivateObject
    }

    [Header("Lever Behavior")]
    [SerializeField] private LeverAction action = LeverAction.ActivateObject;

    [Header("Target Object (Bridge or Door)")]
    [SerializeField] private GameObject targetObject;

    [Header("Optional: One-time use")]
    [SerializeField] private bool disableAfterUse = true;

    private bool hasTriggered = false;

    public void ActivateLever()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (targetObject != null)
        {
            switch (action)
            {
                case LeverAction.ActivateObject:
                    targetObject.SetActive(true);
                    break;

                case LeverAction.DeactivateObject:
                    targetObject.SetActive(false);
                    break;
            }
        }

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
