using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossManager bossManager;
    [SerializeField] private GameObject bossObject;
    [SerializeField] private BossDoor door; // Optional door that closes when fight starts

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // Close the door if assigned
        if (door != null)
            door.CloseDoor();

        // Activate boss object
        if (bossObject != null)
            bossObject.SetActive(true);

        // Assign boss to manager
        IBossHealth boss = bossObject.GetComponent<IBossHealth>();
        if (boss != null)
            bossManager.SetBoss(boss);

        // Start intro + music + UI
        bossManager.StartBossFight();
    }
}
