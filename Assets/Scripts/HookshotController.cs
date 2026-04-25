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
    public bool IsPulling { get; private set; } = false;

    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDash dash;
    private PlayerStats stats;

    private int playerLayer;
    private int enemyLayer;

    private Enemy currentPulledEnemy = null;
    private PullableObject currentPulledObject = null;

    private Coroutine pullRoutine = null;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        dash = GetComponent<PlayerDash>();
        stats = GetComponent<PlayerStats>();

        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer == -1 || enemyLayer == -1)
            Debug.LogError("ERROR: Missing 'Player' or 'Enemy' layer in Project Settings → Tags and Layers.");

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
        if (Input.GetMouseButtonDown(1) && !IsPulling && activeHook == null)
            FireHook();

        UpdateRopePositions();
    }

    private void UpdateRopePositions()
    {
        if (!line.enabled)
            return;

        // Rope start = player
        line.SetPosition(0, transform.position);

        // Rope end depends on state
        if (IsPulling)
        {
            if (currentPulledEnemy != null)
            {
                HookshotTarget ht = currentPulledEnemy.GetComponent<HookshotTarget>();
                if (ht != null && ht.attachPoint != null)
                    line.SetPosition(1, ht.attachPoint.position);
                else
                    line.SetPosition(1, currentPulledEnemy.transform.position);
            }
            else if (currentPulledObject != null)
            {
                HookshotTarget ht = currentPulledObject.GetComponent<HookshotTarget>();
                if (ht != null && ht.attachPoint != null)
                    line.SetPosition(1, ht.attachPoint.position);
                else
                    line.SetPosition(1, currentPulledObject.transform.position);
            }
            else
            {
                line.SetPosition(1, transform.position);
            }
        }
        else if (activeHook != null)
        {
            line.SetPosition(1, activeHook.transform.position);
        }
    }

    private void FireHook()
    {
        Vector3 spawnPos = transform.position + transform.right * 0.5f;
        Quaternion rot = Quaternion.Euler(0, 0, transform.eulerAngles.z);

        GameObject hookObj = Instantiate(hookProjectilePrefab, spawnPos, rot);
        activeHook = hookObj.GetComponent<HookshotProjectile>();
        activeHook.Initialize(this, hookSpeed, maxHookDistance);

        line.enabled = true;
    }

    public void OnHookHit(HookshotTarget target, Vector3 hitPoint)
    {
        if (target == null)
        {
            ResetHook();
            return;
        }

        currentPulledEnemy = target.GetComponentInParent<Enemy>();
        currentPulledObject = target.GetComponentInParent<PullableObject>();

        switch (target.hookType)
        {
            case HookshotTarget.HookType.PullPlayer:
                pullRoutine = StartCoroutine(PullPlayer(hitPoint));
                break;

            case HookshotTarget.HookType.PullObject:
                pullRoutine = StartCoroutine(PullObject(target, hitPoint));
                break;

            case HookshotTarget.HookType.BreakOff:
                if (target.detachablePart != null)
                {
                    target.DetachPart();

                    ShellEnemyAI shellAI = target.GetComponentInParent<ShellEnemyAI>();
                    if (shellAI != null)
                        shellAI.BreakShell();

                    // still use PullObject, but it will now prefer detachablePart
                    pullRoutine = StartCoroutine(PullObject(target, hitPoint));
                }
                break;

            case HookshotTarget.HookType.Trigger:
                target.Trigger();
                ResetHook();
                break;
        }
    }

    private IEnumerator PullPlayer(Vector3 point)
    {
        IsPulling = true;

        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        stats.SetInvincible(true);

        movement.SetCanMove(false);
        dash.SetCanDash(false);

        if (currentPulledEnemy != null)
        {
            currentPulledEnemy.Stun(1f);
            currentPulledEnemy.IsBeingPulled = true;
        }

        if (currentPulledObject != null)
            currentPulledObject.IsBeingPulled = true;

        while (IsPulling && Vector2.Distance(transform.position, point) > 0.7f)
        {
            transform.position = Vector2.MoveTowards(transform.position, point, pullSpeed * Time.deltaTime);
            yield return null;
        }

        ResetHook();
    }

    private IEnumerator PullObject(HookshotTarget target, Vector3 point)
    {
        IsPulling = true;

        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        stats.SetInvincible(true);

        // ⭐ NEW: if this target has a detachablePart (like the shell), pull THAT instead of the enemy
        Transform obj;

        if (target.hookType == HookshotTarget.HookType.BreakOff && target.detachablePart != null)
        {
            obj = target.detachablePart.transform;
        }
        else
        {
            // keep existing behavior for normal PullObject targets
            obj = target.attachPoint != null ? target.attachPoint : target.transform;
        }

        if (currentPulledEnemy != null)
        {
            currentPulledEnemy.Stun(1f);
            currentPulledEnemy.IsBeingPulled = true;
        }

        if (currentPulledObject != null)
            currentPulledObject.IsBeingPulled = true;

        while (IsPulling && obj != null && Vector2.Distance(obj.position, transform.position) > 0.7f)
        {
            obj.position = Vector2.MoveTowards(obj.position, transform.position, pullSpeed * Time.deltaTime);
            yield return null;
        }

        ResetHook();
    }

    public void CancelHookshot()
    {
        if (pullRoutine != null)
        {
            StopCoroutine(pullRoutine);
            pullRoutine = null;
        }

        ResetHook();
    }

    public void ResetHook()
    {
        IsPulling = false;

        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        stats.SetInvincible(false);

        movement.SetCanMove(true);
        attack.SetCanAttack(true);
        dash.SetCanDash(true);

        if (currentPulledEnemy != null)
        {
            currentPulledEnemy.IsBeingPulled = false;

            Rigidbody2D er = currentPulledEnemy.GetComponent<Rigidbody2D>();
            if (er != null)
                er.linearVelocity = Vector2.zero;
        }

        if (currentPulledObject != null)
            currentPulledObject.IsBeingPulled = false;

        if (activeHook != null)
            Destroy(activeHook.gameObject);

        activeHook = null;
        currentPulledEnemy = null;
        currentPulledObject = null;
        line.enabled = false;
    }
}
