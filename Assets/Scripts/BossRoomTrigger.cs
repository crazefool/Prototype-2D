using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossManager bossManager;
    [SerializeField] private GameObject bossObject;
    [SerializeField] private BossDoor door;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // Activate the door object if it’s disabled
        if (door != null && !door.gameObject.activeSelf)
            door.gameObject.SetActive(true);

        door?.CloseDoor();

        if (bossObject != null)
            bossObject.SetActive(true);

        IBossHealth boss = bossObject.GetComponent<IBossHealth>();
        if (boss != null)
            bossManager.SetBoss(boss);

        bossManager.StartBossFight();
    }
}
