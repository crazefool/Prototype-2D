using UnityEngine;

public class BossState : MonoBehaviour
{
    public string uniqueID;

    private void Start()
    {
        if (WorldStateManager.Instance.IsBossDead(uniqueID))
            gameObject.SetActive(false);
    }

    public void MarkDead()
    {
        WorldStateManager.Instance.MarkBossDead(uniqueID);
        gameObject.SetActive(false);
    }
}
