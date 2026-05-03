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
        gameObject.SetActive(true);
        if (col != null) col.enabled = true;
        if (sr != null) sr.enabled = true;
    }

    public void OpenDoor()
    {
        if (col != null) col.enabled = false;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
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
    }
}
