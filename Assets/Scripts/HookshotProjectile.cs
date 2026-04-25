using UnityEngine;

public class HookshotProjectile : MonoBehaviour
{
    private HookshotController controller;
    private float speed;
    private float maxDistance;
    private Vector3 startPos;

    private bool hasHit = false;

    public void Initialize(HookshotController controller, float speed, float maxDistance)
    {
        this.controller = controller;
        this.speed = speed;
        this.maxDistance = maxDistance;
        startPos = transform.position;
    }

    void Update()
    {
        if (hasHit)
            return;

        transform.position += transform.right * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            if (controller != null)
                controller.ResetHook();

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit)
            return;

        hasHit = true;

        if (controller == null)
        {
            Destroy(gameObject);
            return;
        }

        // ⭐ Always go to the root of what we hit, then search all children
        Transform root = col.transform.root;
        HookshotTarget target = root.GetComponentInChildren<HookshotTarget>();

        if (target != null)
        {
            controller.OnHookHit(target, transform.position);
        }
        else
        {
            controller.ResetHook();
        }

        Destroy(gameObject);
    }
}
