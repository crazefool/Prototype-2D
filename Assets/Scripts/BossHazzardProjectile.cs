using UnityEngine;

public class BossHazardProjectile : MonoBehaviour
{
    private Vector2 targetPos;
    private float speed;

    [Header("Hazard Settings")]
    [SerializeField] private GameObject hazardAreaPrefab;
    [SerializeField] private float stopDistance = 0.1f;

    public void Initialize(Vector2 target, float spd)
    {
        targetPos = target;
        speed = spd;
    }

    void Update()
    {
        // Move toward target
        Vector2 pos = transform.position;
        Vector2 dir = (targetPos - pos).normalized;

        transform.position = pos + dir * speed * Time.deltaTime;

        // If close enough → explode
        if (Vector2.Distance(transform.position, targetPos) <= stopDistance)
        {
            SpawnHazard();
            Destroy(gameObject);
        }
    }

    private void SpawnHazard()
    {
        if (hazardAreaPrefab != null)
            Instantiate(hazardAreaPrefab, transform.position, Quaternion.identity);
    }
}
