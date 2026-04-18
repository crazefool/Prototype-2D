using UnityEngine;

public class HookshotProjectile : MonoBehaviour
{
    private HookshotController controller;
    private float speed;
    private float maxDistance;
    private Vector3 startPos;

    // ⭐ Called by HookshotController
    public void Initialize(HookshotController controller, float speed, float maxDistance)
    {
        this.controller = controller;
        this.speed = speed;
        this.maxDistance = maxDistance;
        startPos = transform.position;

        Debug.Log("HOOKSHOT INIT: startPos = " + startPos + ", speed = " + speed + ", maxDist = " + maxDistance);
    }

    void Update()
    {
        // Debug: Check if controller is missing
        if (controller == null)
        {
            Debug.LogError("HOOKSHOT ERROR: controller is NULL in Update()");
            return;
        }

        // Move forward
        transform.position += transform.right * speed * Time.deltaTime;

        Debug.Log("HOOK POS: " + transform.position);

        // Check max distance
        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            Debug.Log("HOOKSHOT: Max distance reached, resetting hook.");
            controller.ResetHook();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("HOOKSHOT HIT: " + col.name);

        HookshotTarget target = col.GetComponent<HookshotTarget>();
        controller.OnHookHit(target, transform.position);
    }
}
