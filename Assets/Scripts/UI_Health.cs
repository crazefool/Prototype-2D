using UnityEngine;
using UnityEngine.UI;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    // Assign ALL 10 heart images here (children of TopLeftUI)
    [SerializeField] private Image[] hearts;

    private void Start()
    {
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            bool shouldShow = i < playerStats.CurrentHealth;

            if (hearts[i] != null)
            {
                // This will also turn on hearts that start disabled
                hearts[i].gameObject.SetActive(shouldShow);
            }
        }
    }
}
