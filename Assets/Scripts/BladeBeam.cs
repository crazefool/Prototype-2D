using UnityEngine;

public class BladeBeam : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 2;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float hitStopDuration = 0.05f;

    private PlayerAttack player;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerAttack>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, knockbackDir);

            if (player != null)
                player.StartCoroutine(player.HitStop(hitStopDuration));

            // pierces enemies
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
