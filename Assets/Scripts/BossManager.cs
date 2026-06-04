using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private string bossName;
    [SerializeField] private BossHealthBar healthBar;

    private IBossHealth boss;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup introCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI introText;

    [Header("Music")]
    [SerializeField] private AudioSource bossMusic;
    [SerializeField] private AudioSource normalMusic;

    [Header("Door Control")]
    [SerializeField] private BossDoor door;

    private bool fightStarted = false;

    // Expose the name used for saving
    public string BossName => bossName;

    public void SetBoss(IBossHealth bossRef)
    {
        boss = bossRef;
    }

    public void StartBossFight()
    {
        if (fightStarted) return;
        if (boss == null)
        {
            Debug.LogError($"{name}: No boss assigned before StartBossFight was called.");
            return;
        }

        fightStarted = true;
        StartCoroutine(BossIntroRoutine());
    }

    private IEnumerator BossIntroRoutine()
    {
        if (normalMusic != null)
            normalMusic.Stop();

        if (introText == null || introCanvas == null)
        {
            Debug.LogError($"{name}: Missing intro UI references.");
            yield break;
        }

        introText.text = bossName;
        introCanvas.alpha = 0;
        introCanvas.gameObject.SetActive(true);

        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            introCanvas.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        for (float t = 1f; t > 0f; t -= Time.deltaTime * 2f)
        {
            introCanvas.alpha = t;
            yield return null;
        }

        introCanvas.gameObject.SetActive(false);

        if (bossMusic != null)
            bossMusic.Play();

        healthBar.gameObject.SetActive(true);
        healthBar.Initialize(boss);

        (boss as MonoBehaviour)?.SendMessage("BeginFight", SendMessageOptions.DontRequireReceiver);
    }

    public void OnBossDefeated()
    {
        // SAVE PROGRESS under bossName
        SaveGameManager.MarkBossDefeated(bossName);

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (bossMusic != null)
            bossMusic.Stop();

        if (normalMusic != null)
            normalMusic.Play();

        if (door != null)
            StartCoroutine(OpenDoorDelayed());
    }

    private IEnumerator OpenDoorDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        door.OpenDoor();
    }
}
