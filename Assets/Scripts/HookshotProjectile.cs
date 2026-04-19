using UnityEngine;

public class HookshotProjectile : MonoBehaviour
{
    private HookshotController controller;
    private float speed;
    private float maxDistance;
    private Vector3 startPos;

    private bool hasHit = false;   // ⭐ NEW

    public void Initialize(HookshotController controller, float speed, float maxDistance)
    {
        this.controller = controller;
        this.speed = speed;
        this.maxDistance = maxDistance;
        startPos = transform.position;
    }

    void Update()
    {
        if (hasHit) return;   // ⭐ Stop moving after first hit

        transform.position += transform.right * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            controller.ResetHook();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;   // ⭐ Ignore all further collisions
        hasHit = true;

        HookshotTarget target = col.GetComponent<HookshotTarget>();
        controller.OnHookHit(target, transform.position);
    }
}
