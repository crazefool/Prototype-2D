using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public List<GameObject> enemies;
    }

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    [Header("Doors")]
    public GameObject[] doorsToClose;
    public GameObject[] doorsToOpen;

    private int currentWave = 0;
    private bool playerInside = false;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    private void Awake()
    {
        // ⭐ If gauntlet already cleared → open doors and disable trigger
        if (SaveGameManager.IsGauntletCleared(gameObject.name))
        {
            foreach (var door in doorsToOpen)
                door.SetActive(false);

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerInside)
            return;

        if (!other.CompareTag("Player"))
            return;

        // ⭐ If gauntlet already cleared → skip entirely
        if (SaveGameManager.IsGauntletCleared(gameObject.name))
        {
            foreach (var door in doorsToOpen)
                door.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

        playerInside = true;
        StartCoroutine(StartGauntlet());
    }

    private IEnumerator StartGauntlet()
    {
        foreach (var door in doorsToClose)
            door.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        while (currentWave < waves.Count)
        {
            SpawnWave(currentWave);

            yield return new WaitUntil(() => aliveEnemies.Count == 0);

            currentWave++;
            yield return new WaitForSeconds(1f);
        }

        foreach (var door in doorsToOpen)
            door.SetActive(false);

        // ⭐ Save gauntlet cleared
        SaveGameManager.MarkGauntletCleared(gameObject.name);

        gameObject.SetActive(false);
    }

    private void SpawnWave(int waveIndex)
    {
        aliveEnemies.Clear();

        foreach (var enemyPrefab in waves[waveIndex].enemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Object.Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            aliveEnemies.Add(enemy);

            Enemy enemyScript = enemy.GetComponent<Enemy>();

            enemyScript.OnDeath += () =>
            {
                aliveEnemies.Remove(enemy);
            };
        }
    }
}
