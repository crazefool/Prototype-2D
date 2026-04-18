using System.Collections;
using UnityEngine;

public class HookshotController : MonoBehaviour
{
    [Header("Hookshot Settings")]
    [SerializeField] private GameObject hookProjectilePrefab;
    [SerializeField] private float hookSpeed = 20f;
    [SerializeField] private float pullSpeed = 12f;
    [SerializeField] private float maxHookDistance = 12f;

    private LineRenderer line;
    private HookshotProjectile activeHook;
    private bool isPulling = false;

    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDash dash;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        dash = GetComponent<PlayerDash>();

        // Rope renderer
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
    }

    void Update()
    {
        // Right mouse button = hookshot
        if (Input.GetMouseButtonDown(1) && !isPulling && activeHook == null)
        {
            FireHook();
        }

        // Update rope line
        if (activeHook != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, activeHook.transform.position);
        }
    }

    private void FireHook()
    {
        // ⭐ Spawn the hook IN FRONT of the player, not inside them
        Vector3 spawnPos = transform.position + transform.right * 0.5f;

        // ⭐ Use the player's facing direction for rotation
        Quaternion rot = Quaternion.Euler(0, 0, transform.eulerAngles.z);

        // Instantiate hook
        GameObject hookObj = Instantiate(hookProjectilePrefab, spawnPos, rot);
        activeHook = hookObj.GetComponent<HookshotProjectile>();

        // Initialize hook projectile
        activeHook.Initialize(this, hookSpeed, maxHookDistance);

        // Enable rope
        line.enabled = true;
    }

    public void OnHookHit(HookshotTarget target, Vector3 hitPoint)
    {
        if (target == null)
        {
            ResetHook();
            return;
        }

        switch (target.hookType)
        {
            case HookshotTarget.HookType.PullPlayer:
                StartCoroutine(PullPlayer(hitPoint));
                break;

            case HookshotTarget.HookType.PullObject:
                StartCoroutine(PullObject(target, hitPoint));
                break;

            case HookshotTarget.HookType.BreakOff:
                if (target.detachablePart != null)
                    target.DetachPart();
                ResetHook();
                break;

            case HookshotTarget.HookType.Trigger:
                target.Trigger();
                ResetHook();
                break;
        }
    }

    private IEnumerator PullPlayer(Vector3 point)
    {
        isPulling = true;

        movement.SetCanMove(false);
        attack.SetCanAttack(false);
        dash.SetCanDash(false);

        while (Vector2.Distance(transform.position, point) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, point, pullSpeed * Time.deltaTime);
            yield return null;
        }

        ResetHook();
    }

    private IEnumerator PullObject(HookshotTarget target, Vector3 point)
    {
        isPulling = true;

        movement.SetCanMove(false);
        attack.SetCanAttack(false);
        dash.SetCanDash(false);

        Transform obj = target.transform;

        // ⭐ FIX: Prevent errors if the object is destroyed mid-pull
        while (obj != null && Vector2.Distance(obj.position, transform.position) > 0.5f)
        {
            obj.position = Vector2.MoveTowards(obj.position, transform.position, pullSpeed * Time.deltaTime);
            yield return null;
        }

        ResetHook();
    }

    public void ResetHook()
    {
        isPulling = false;

        movement.SetCanMove(true);
        attack.SetCanAttack(true);
        dash.SetCanDash(true);

        if (activeHook != null)
            Destroy(activeHook.gameObject);

        activeHook = null;
        line.enabled = false;
    }
}
