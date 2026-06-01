using UnityEngine;

public class GauntletState : MonoBehaviour
{
    public string uniqueID;

    public void Start()
    {
        if (WorldStateManager.Instance.IsGauntletCleared(uniqueID))
            gameObject.SetActive(false);
    }

    public void MarkCleared()
    {
        WorldStateManager.Instance.MarkGauntletCleared(uniqueID);
        gameObject.SetActive(false);
    }
}
