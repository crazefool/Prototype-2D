using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    [Header("Object to Activate")]
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
            // If it's a door
            DoorOpener door = targetObject.GetComponent<DoorOpener>();
            if (door != null)
            {
                door.OpenDoor();
            }
            else
            {
                // Default behavior: activate object (platform)
                targetObject.SetActive(true);
            }
        }

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
