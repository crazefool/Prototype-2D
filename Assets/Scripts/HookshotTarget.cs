using UnityEngine;

public class HookshotTarget : MonoBehaviour
{
    public enum HookType { PullPlayer, PullObject, BreakOff, Trigger }
    public HookType hookType;

    public GameObject detachablePart;
    public Transform attachPoint;

    public void DetachPart()
    {
        ShellEnemyAI shellAI = GetComponentInParent<ShellEnemyAI>();
        if (shellAI != null)
            shellAI.BreakShell();

        if (detachablePart != null)
            detachablePart.transform.SetParent(null);
    }

    public void Trigger()
    {
        Debug.Log("Hookshot triggered object: " + name);

        LeverTrigger lever = GetComponent<LeverTrigger>();
        if (lever != null)
        {
            lever.ActivateLever();
        }
        else
        {
            Debug.LogWarning("HookshotTarget.Trigger() called, but no LeverTrigger found on " + name);
        }
    }
}
