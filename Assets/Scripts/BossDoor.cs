using UnityEngine;
using System.Collections;

public class BossDoor : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    public void CloseDoor()
    {
        // Door becomes visible and solid
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
        // Door stops blocking, fades out visually
        if (col != null)
            col.enabled = false;

        if (sr != null)
        {
            sr.enabled = true;
            StartCoroutine(FadeOutAndDisable());
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        if (sr == null) yield break;

        Color c = sr.color;
        for (float t = 1f; t >= 0f; t -= Time.deltaTime * 2f)
        {
            c.a = t;
            sr.color = c;
            yield return null;
        }

        sr.enabled = false;
        c.a = 1f;
        sr.color = c;

        // Disable the door object completely after fade
        gameObject.SetActive(false);
    }
}
