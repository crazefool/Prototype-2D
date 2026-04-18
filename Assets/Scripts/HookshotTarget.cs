using UnityEngine;

public class HookshotTarget : MonoBehaviour
{
    public enum HookType { PullPlayer, PullObject, BreakOff, Trigger }
    public HookType hookType;

    public GameObject detachablePart;

    public void DetachPart()
    {
        if (detachablePart != null)
        {
            detachablePart.transform.SetParent(null);
            // Optional: add force or animation
        }
    }

    public void Trigger()
    {
        // For switches, levers, puzzle elements
        Debug.Log("Hookshot triggered object: " + name);
    }
}
