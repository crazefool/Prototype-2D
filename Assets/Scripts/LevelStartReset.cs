using UnityEngine;

public class LevelStartReset : MonoBehaviour
{
    private void Awake()
    {
        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.hasCheckpoint = false;
            WorldStateManager.Instance.checkpointUsedOnce = false;
        }
    }
}
