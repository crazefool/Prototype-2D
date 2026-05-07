using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Vector2 moveOffset = Vector2.zero;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool pingPong = true;

    private Vector2 startPos;
    private Vector2 targetPos;
    private bool movingForward = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveOffset;
    }

    void Update()
    {
        if (!isMoving)
            return;

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, movingForward ? targetPos : startPos, step);

        if (Vector2.Distance(transform.position, movingForward ? targetPos : startPos) < 0.01f)
        {
            if (pingPong)
                movingForward = !movingForward;
        }
    }

    // Optional: visualize movement path in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveOffset);
    }
}
