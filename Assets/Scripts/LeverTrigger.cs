using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    [Header("Platform to Activate")]
    [SerializeField] private GameObject platformToActivate;

    [Header("Optional: One-time use")]
    [SerializeField] private bool disableAfterUse = true;

    private bool hasTriggered = false;

    public void ActivateLever()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (platformToActivate != null)
            platformToActivate.SetActive(true);

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
