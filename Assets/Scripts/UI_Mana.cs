using UnityEngine;
using UnityEngine.UI;

public class UI_Mana : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image manaFill;

    [Header("Visuals")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowManaColor = new Color(1f, 1f, 1f, 0.4f);

    private void Start()
    {
        UpdateMana();
    }

    public void UpdateMana()
    {
        // Total mana including partial progress
        float partialMana =
            playerStats.CurrentMana +
            (float)playerStats.HitCounter / playerStats.HitsPerMana;

        float fillAmount = partialMana / playerStats.MaxMana;
        manaFill.fillAmount = fillAmount;

        // Visual clarity: less than 1 MP looks weaker
        if (partialMana < 1f)
            manaFill.color = lowManaColor;
        else
            manaFill.color = normalColor;
    }
}
