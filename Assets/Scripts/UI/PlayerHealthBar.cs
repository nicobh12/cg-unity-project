using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.maxValue = targetHealth.MaxHealth;
        slider.value = targetHealth.CurrentHealth;

        targetHealth.onHealthChanged.AddListener(UpdateBar);
    }

    private void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}