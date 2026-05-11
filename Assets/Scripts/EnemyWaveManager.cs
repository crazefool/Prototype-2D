using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public List<GameObject> enemies; // Prefabs to spawn
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerInside)
            return;

        if (other.CompareTag("Player"))
        {
            playerInside = true;
            StartCoroutine(StartGauntlet());
        }
    }

    private IEnumerator StartGauntlet()
    {
        // Close doors
        foreach (var door in doorsToClose)
            door.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // Start waves
        while (currentWave < waves.Count)
        {
            SpawnWave(currentWave);

            // Wait until all enemies are dead
            yield return new WaitUntil(() => aliveEnemies.Count == 0);

            currentWave++;
            yield return new WaitForSeconds(1f);
        }

        // Open doors
        foreach (var door in doorsToOpen)
            door.SetActive(false);
    }

    private void SpawnWave(int waveIndex)
    {
        aliveEnemies.Clear();

        foreach (var enemyPrefab in waves[waveIndex].enemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            aliveEnemies.Add(enemy);

            Enemy enemyScript = enemy.GetComponent<Enemy>();

            // ⭐ Remove enemy from list when it dies
            enemyScript.OnDeath += () =>
            {
                aliveEnemies.Remove(enemy);
            };
        }
    }
}
