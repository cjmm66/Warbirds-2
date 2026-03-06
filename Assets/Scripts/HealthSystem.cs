using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Reusable health component. Attach to any damageable entity
/// (player truck, soldier, enemy aircraft, artillery).
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Invincibility")]
    [Tooltip("Seconds of invincibility after taking damage. 0 = none.")]
    [SerializeField] private float invincibilityDuration = 0.15f;

    [Header("Events")]
    [Tooltip("Fires (currentHealth, maxHealth) whenever health changes.")]
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent onDeath;
    public UnityEvent onDamageTaken;

    private float invincibilityEndTime;

    /// <summary>Current health value.</summary>
    public int CurrentHealth => currentHealth;

    /// <summary>Maximum health value.</summary>
    public int MaxHealth => maxHealth;

    /// <summary>True when the entity has zero health.</summary>
    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Apply damage. Respects invincibility frames.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        if (Time.time < invincibilityEndTime)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onDamageTaken?.Invoke();

        if (invincibilityDuration > 0f)
        {
            invincibilityEndTime = Time.time + invincibilityDuration;
        }

        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
        }
    }

    /// <summary>
    /// Restore health, clamped to maxHealth.
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Reset health to maximum (e.g. on respawn / new wave).
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        invincibilityEndTime = 0f;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Returns health as 0-1 ratio.
    /// </summary>
    public float GetHealthPercent()
    {
        if (maxHealth <= 0)
        {
            return 0f;
        }

        return (float)currentHealth / maxHealth;
    }
}
