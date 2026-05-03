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
    [SerializeField] private BossDoor door; // ✅ Added field for door reopening

    private bool fightStarted = false;

    // Called by BossRoomTrigger
    public void SetBoss(IBossHealth bossRef)
    {
        boss = bossRef;
    }

    public void StartBossFight()
    {
        if (fightStarted) return;
        if (boss == null)
        {
            Debug.LogError("BossManager: No boss assigned before StartBossFight was called.");
            return;
        }

        fightStarted = true;
        StartCoroutine(BossIntroRoutine());
    }

    private IEnumerator BossIntroRoutine()
    {
        if (normalMusic != null)
            normalMusic.Stop();

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
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (bossMusic != null)
            bossMusic.Stop();

        if (normalMusic != null)
            normalMusic.Play();

        // ✅ Reopen the door when boss is defeated
        if (door != null)
            door.OpenDoor();
    }
}
