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
        if (!other.CompareTag("Player"))
            return;

        // Use the SAME name that BossManager used when saving
        if (SaveGameManager.IsBossDefeated(bossManager.BossName))
        {
            if (door != null)
                door.OpenDoor();

            if (bossObject != null)
                bossObject.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

        if (triggered) return;
        triggered = true;

        if (door != null && !door.gameObject.activeSelf)
            door.gameObject.SetActive(true);

        door?.CloseDoor();

        if (bossObject != null)
            bossObject.SetActive(true);

        IBossHealth boss = bossObject.GetComponent<IBossHealth>(); // or GetComponentInChildren<IBossHealth>()
        if (boss != null)
            bossManager.SetBoss(boss);

        bossManager.StartBossFight();
    }
}
