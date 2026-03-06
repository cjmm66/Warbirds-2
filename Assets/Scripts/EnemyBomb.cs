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

    [Header("Fall Physics")]
    [Tooltip("Lower values make the bomb fall more slowly.")]
    [SerializeField] private float gravityScale = 0.35f;

    [Header("Rotation")]
    [Tooltip("Degrees per second while rotating to match movement direction.")]
    [SerializeField] private float rotationSpeed = 120f;
    [Tooltip("Angle offset so the bomb tip points into the movement direction.")]
    [SerializeField] private float tipAngleOffset = -90f;

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

        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }

        if (trail != null)
        {
            trail.enabled = true;
        }
    }

    private void Update()
    {
        // Rotate bomb gradually so it falls tip-first without snapping instantly.
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg + tipAngleOffset;
            float currentAngle = transform.eulerAngles.z;
            float nextAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, nextAngle);
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
