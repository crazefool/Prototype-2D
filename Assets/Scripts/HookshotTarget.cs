using UnityEngine;

public class HookshotTarget : MonoBehaviour
{
    public enum HookType { PullPlayer, PullObject, BreakOff, Trigger }
    public HookType hookType;

    public GameObject detachablePart;

    public void DetachPart()
    {
        // ⭐ FIX: Notify the shell enemy BEFORE detaching the shell
        ShellEnemyAI shellAI = GetComponentInParent<ShellEnemyAI>();
        if (shellAI != null)
            shellAI.BreakShell();

        // Now detach the shell safely
        if (detachablePart != null)
        {
            detachablePart.transform.SetParent(null);
            // Optional: add force or animation
        }
    }

    public void Trigger()
    {
        Debug.Log("Hookshot triggered object: " + name);
    }
}
