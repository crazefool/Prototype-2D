using System.Collections;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [Header("Ghost Settings")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float spawnInterval = 0.05f;
    [SerializeField] private float ghostLifetime = 0.2f;

    private bool isActive = false;

    public void StartTrail()
    {
        if (!isActive)
            StartCoroutine(SpawnGhosts());
    }

    public void StopTrail()
    {
        isActive = false;
    }

    private IEnumerator SpawnGhosts()
    {
        isActive = true;

        while (isActive)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
            SpriteRenderer playerSR = GetComponentInChildren<SpriteRenderer>();


            ghostSR.sprite = playerSR.sprite;
            ghostSR.flipX = playerSR.flipX;
            ghostSR.flipY = playerSR.flipY;
            ghostSR.color = new Color(1f, 1f, 1f, 0.6f);

            Destroy(ghost, ghostLifetime);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
