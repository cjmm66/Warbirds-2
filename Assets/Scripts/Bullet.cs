using UnityEngine;

/// <summary>
/// Projectile behaviour. Deals damage via HealthSystem on collision.
/// Self-destructs on collision or after a lifetime timeout.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Lifetime")]
    [Tooltip("Seconds before the bullet self-destructs if it hasn't hit anything.")]
    [SerializeField] private float lifetimeSeconds = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetimeSeconds);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to damage the target via HealthSystem
        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Legacy fallback: if enemy has no HealthSystem, destroy directly
        if (health == null && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }

        // Destroy the bullet itself
        Destroy(gameObject);
    }
}
