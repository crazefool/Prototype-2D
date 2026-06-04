using UnityEngine;
using System.Collections.Generic;

public class BossRoomTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossManager bossManager;
    [SerializeField] private GameObject bossObject;

    [Header("Doors")]
    [Tooltip("Add all doors that should close when entering and open when boss is defeated.")]
    [SerializeField] private List<BossDoor> doors = new List<BossDoor>();

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // If boss already defeated → skip fight entirely
        if (SaveGameManager.IsBossDefeated(bossManager.BossName))
        {
            foreach (BossDoor d in doors)
            {
                if (d != null)
                    d.OpenDoor();
            }

            if (bossObject != null)
                bossObject.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

        if (triggered) return;
        triggered = true;

        // Close ALL doors
        foreach (BossDoor d in doors)
        {
            if (d != null)
            {
                if (!d.gameObject.activeSelf)
                    d.gameObject.SetActive(true);

                d.CloseDoor();
            }
        }

        // Activate boss
        if (bossObject != null)
            bossObject.SetActive(true);

        // Assign boss to manager
        IBossHealth boss = bossObject.GetComponent<IBossHealth>();
        if (boss != null)
            bossManager.SetBoss(boss);
        else
            Debug.LogError("BossRoomTrigger: Could not find IBossHealth on bossObject!");

        bossManager.StartBossFight();
    }
}
