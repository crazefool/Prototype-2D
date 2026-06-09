using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject checkpointText;
    [SerializeField] private GameObject flashEffect;

    private bool activatedOnce = false;

    private void Awake()
    {
        if (checkpointText != null)
            checkpointText.SetActive(false);

        if (flashEffect != null)
            flashEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null)
            return;

        // Save checkpoint position on the player
        stats.SetCheckpoint(other.transform.position);

        // Save full progress (bosses, chests, levers, gauntlets, upgrades, position)
        SaveGameManager.SaveProgress(stats);

        if (!activatedOnce)
        {
            activatedOnce = true;

            stats.RestoreFullHealth();
            stats.RestoreFullMana();

            StartCoroutine(CheckpointRoutine(true));
        }
        else
        {
            StartCoroutine(CheckpointRoutine(false));
        }
    }

    private IEnumerator CheckpointRoutine(bool firstTime)
    {
        if (firstTime && flashEffect != null)
        {
            flashEffect.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            flashEffect.SetActive(false);
        }

        if (checkpointText != null)
        {
            checkpointText.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            checkpointText.SetActive(false);
        }
    }
}
