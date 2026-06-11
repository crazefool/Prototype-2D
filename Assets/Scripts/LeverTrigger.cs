using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public enum LeverAction
    {
        ActivateObjects,
        DeactivateObjects
    }

    [Header("Lever Behavior")]
    [SerializeField] private LeverAction action = LeverAction.ActivateObjects;

    [Header("Targets (put multiple objects here)")]
    [SerializeField] private GameObject[] targetObjects;

    [Header("Lever Visual")]
    [SerializeField] private Sprite activatedSprite;

    [Header("Optional: One-time use")]
    [SerializeField] private bool disableAfterUse = true;

    private bool hasTriggered = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If lever was already used in saved progress → apply instantly
        if (SaveGameManager.IsLeverUsed(gameObject.name))
        {
            ApplyLeverEffect();

            if (activatedSprite != null)
                spriteRenderer.sprite = activatedSprite;

            if (disableAfterUse)
            {
                Collider2D col = GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = false;
            }

            hasTriggered = true;
        }
    }

    public void ActivateLever()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        // Save lever used
        SaveGameManager.MarkLeverUsed(gameObject.name);

        ApplyLeverEffect();

        // Change lever sprite
        if (activatedSprite != null)
            spriteRenderer.sprite = activatedSprite;

        // Disable collider so it can't be activated again
        if (disableAfterUse)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }

    private void ApplyLeverEffect()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;

            switch (action)
            {
                case LeverAction.ActivateObjects:
                    obj.SetActive(true);
                    break;

                case LeverAction.DeactivateObjects:
                    obj.SetActive(false);
                    break;
            }
        }
    }
}