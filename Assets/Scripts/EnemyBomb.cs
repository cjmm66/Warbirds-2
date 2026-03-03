using UnityEngine;

/// <summary>
/// Projectile dropped by enemy aircraft. Falls with gravity,
/// deals damage to the player on collision via HealthSystem.
/// </summary>
public class EnemyBomb : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Lifetime")]
    [Tooltip("Seconds before self-destruct if no collision occurs.")]
    [SerializeField] private float lifetimeSeconds = 6f;

    [Header("Visual")]
    [Tooltip("Optional trail renderer to enable on spawn.")]
    [SerializeField] private TrailRenderer trail;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetimeSeconds);

        if (trail != null)
        {
            trail.enabled = true;
        }
    }

    private void Update()
    {
        // Rotate bomb to face its velocity direction
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to damage via HealthSystem
        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Also support trigger colliders (e.g., ground zone)
        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
