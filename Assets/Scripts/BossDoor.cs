using UnityEngine;
using System.Collections;

public class BossDoor : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D col;

    [Header("Door Settings")]
    [SerializeField] private float fadeSpeed = 1f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);

        if (sr != null)
        {
            sr.enabled = true;
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        if (col != null)
            col.enabled = true;
    }

    public void OpenDoor()
    {
        if (col != null)
            col.enabled = false;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;

            sr.enabled = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        if (sr == null) yield break;

        Color c = sr.color;

        for (float t = 1f; t >= 0f; t -= Time.deltaTime * fadeSpeed)
        {
            c.a = t;
            sr.color = c;
            yield return null;
        }

        // ⭐ DO NOT DISABLE THE GAMEOBJECT
        // Just hide the sprite
        sr.enabled = false;

        // Reset alpha for next time
        c.a = 1f;
        sr.color = c;
    }
}
