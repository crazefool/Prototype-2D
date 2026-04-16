using UnityEngine;
using UnityEngine.UI;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image[] hearts; // Assign your 4 heart images here

    private void Start()
    {
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < playerStats.CurrentHealth;
        }
    }
}
