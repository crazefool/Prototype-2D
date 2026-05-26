using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject checkpointText;   // TMP text object
    [SerializeField] private GameObject flashEffect;      // Optional screen flash

    private bool activated = false;

    private void Awake()
    {
        if (checkpointText != null)
            checkpointText.SetActive(false);

        if (flashEffect != null)
            flashEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.RestoreFullHealth();
            stats.RestoreFullMana();
        }

        SaveCheckpointPosition(other.transform.position);

        StartCoroutine(CheckpointRoutine());
    }

    private void SaveCheckpointPosition(Vector3 pos)
    {
        PlayerPrefs.SetFloat("CheckpointX", pos.x);
        PlayerPrefs.SetFloat("CheckpointY", pos.y);
        PlayerPrefs.Save();
    }

    private IEnumerator CheckpointRoutine()
    {
        if (flashEffect != null)
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
