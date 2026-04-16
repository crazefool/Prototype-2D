using UnityEngine;
using UnityEngine.UI;

public class UI_Mana : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image manaFill; // The blue fill image

    private void Start()
    {
        UpdateMana();
    }

    public void UpdateMana()
    {
        float fillAmount = (float)playerStats.CurrentMana / playerStats.MaxMana;
        manaFill.fillAmount = fillAmount;
    }
}
