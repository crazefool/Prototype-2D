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

        // ⭐ If controller was destroyed or missing, just destroy projectile
        if (controller == null)
        {
            Destroy(gameObject);
            return;
        }

        // ⭐ Try to get a hookshot target
        HookshotTarget target = col.GetComponent<HookshotTarget>();

        if (target != null)
        {
            controller.OnHookHit(target, transform.position);
        }
        else
        {
            // ⭐ Hit something that is NOT hookable → reset hook
            controller.ResetHook();
        }

        Destroy(gameObject);
    }
}
