using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float flashIntensity = 4f; // Higher = whiter silhouette

    private SpriteRenderer sr;
    private MaterialPropertyBlock block;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        block = new MaterialPropertyBlock();
        sr.GetPropertyBlock(block);

        originalColor = sr.color;
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Overbrighten the sprite to force a white silhouette
        block.SetColor("_Color", originalColor * flashIntensity);
        sr.SetPropertyBlock(block);

        yield return new WaitForSeconds(flashDuration);

        // Restore original color
        block.SetColor("_Color", originalColor);
        sr.SetPropertyBlock(block);
    }
}
