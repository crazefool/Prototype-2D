using UnityEngine;

public class HookshotTarget : MonoBehaviour
{
    public enum HookType { PullPlayer, PullObject, BreakOff, Trigger }
    public HookType hookType;

    public GameObject detachablePart;

    // ⭐ NEW: Visual rope attach point
    public Transform attachPoint;

    public void DetachPart()
    {
        ShellEnemyAI shellAI = GetComponentInParent<ShellEnemyAI>();
        if (shellAI != null)
            shellAI.BreakShell();

        if (detachablePart != null)
        {
            detachablePart.transform.SetParent(null);
        }
    }

    public void Trigger()
    {
        Debug.Log("Hookshot triggered object: " + name);
    }
}
