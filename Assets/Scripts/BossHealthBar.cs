using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private IBossHealth boss;

    public void Initialize(IBossHealth bossRef)
    {
        boss = bossRef;
        slider.maxValue = boss.MaxHealth;
        slider.value = boss.CurrentHealth;
    }

    void Update()
    {
        if (boss == null) return;
        slider.value = boss.CurrentHealth;
    }
}
