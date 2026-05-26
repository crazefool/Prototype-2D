using UnityEngine;
using System.Collections;

public class CastleGate : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Vector2 teleportDestination;

    [Header("References")]
    [SerializeField] private FadeController fadeController;
    [SerializeField] private float fadeDuration = 0.6f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TeleportRoutine(other.transform));
    }

    private IEnumerator TeleportRoutine(Transform player)
    {
        isTeleporting = true;

        // Fade to black
        yield return StartCoroutine(fadeController.FadeOut(fadeDuration));

        // Teleport player
        player.position = teleportDestination;

        // Fade back in
        yield return StartCoroutine(fadeController.FadeIn(fadeDuration));

        isTeleporting = false;
    }
}
