using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent onDied;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void SetMaxHealth(float value, bool refill = true)
    {
        maxHealth = Mathf.Max(1f, value);
        currentHealth = refill ? maxHealth : Mathf.Clamp(currentHealth, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool ApplyDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return false;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            onDied?.Invoke();
        }

        return true;
    }

    public bool Heal(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return false;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        return true;
    }

    public void Deplete()
    {
        if (IsDead)
        {
            return;
        }

        currentHealth = 0f;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onDied?.Invoke();
    }
}
