using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float flashIntensity = 4f;

    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock[] blocks;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();

        blocks = new MaterialPropertyBlock[renderers.Length];
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            blocks[i] = new MaterialPropertyBlock();
            renderers[i].GetPropertyBlock(blocks[i]);

            // Force unique material instance
            renderers[i].material = new Material(renderers[i].material);

            originalColors[i] = renderers[i].color;
        }
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            blocks[i].SetColor("_Color", originalColors[i] * flashIntensity);
            renderers[i].SetPropertyBlock(blocks[i]);
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            blocks[i].SetColor("_Color", originalColors[i]);
            renderers[i].SetPropertyBlock(blocks[i]);
        }
    }
}
