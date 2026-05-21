using UnityEngine;

public class RewardSpell : MonoBehaviour
{
    public enum SpellType { BladeBeam, MegaSlash, SlashDash }

    [Header("Reward Settings")]
    [SerializeField] private SpellType spellToUnlock;

    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float displayDuration = 2f;

    private PlayerAttack playerAttack;
    private Vector3 startPos;

    void Awake()
    {
        playerAttack = FindFirstObjectByType<PlayerAttack>();
        startPos = transform.position;
    }

    void Start()
    {
        UnlockSpell();
        StartCoroutine(FloatAndDisappear());
    }

    private void UnlockSpell()
    {
        if (playerAttack == null)
            return;

        switch (spellToUnlock)
        {
            case SpellType.BladeBeam:
                playerAttack.bladeBeamUnlocked = true;
                break;

            case SpellType.MegaSlash:
                playerAttack.megaSlashUnlocked = true;
                break;

            case SpellType.SlashDash:
                playerAttack.slashDashUnlocked = true;
                break;
        }
    }

    private System.Collections.IEnumerator FloatAndDisappear()
    {
        float timer = 0f;
        while (timer < displayDuration)
        {
            timer += Time.deltaTime;
            float offset = Mathf.Sin(timer * floatSpeed) * 0.1f;
            transform.position = startPos + Vector3.up * (offset + floatHeight * (timer / displayDuration));
            yield return null;
        }

        Destroy(gameObject);
    }
}
