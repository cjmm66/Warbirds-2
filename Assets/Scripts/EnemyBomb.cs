using UnityEngine;

/// <summary>
/// Projectile dropped by enemy aircraft. Falls with gravity,
/// deals damage to the player on collision via HealthSystem.
/// Three rotation behaviours (merged from BombLogic):
///   1. Torque spin on spawn — gives a tumbling effect.
///   2. Spawn-time flip    — corrects orientation when bomber flies left.
///   3. Ground-proximity nose-dive via raycast in FixedUpdate.
/// Spawns an explosion prefab on collision.
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
    [Tooltip("Initial torque spin applied on drop (tumbling effect).")]
    [SerializeField] private float torqueAmount = 0.15f;
    [Tooltip("How aggressively the bomb noses down near the ground.")]
    [SerializeField] private float noseDiveStrength = 0.015f;
    [Tooltip("Rotation applied at spawn when the bomber is flying left (-110 keeps nose forward).")]
    [SerializeField] private float leftFacingSpawnRotation = -110f;

    [Header("Explosion")]
    [Tooltip("Prefab to spawn at impact position.")]
    [SerializeField] private GameObject explosionPrefab;
    [Tooltip("Seconds before the explosion object destroys itself (if no ExplosionDestroyer on it).")]
    [SerializeField] private float explosionLifetime = 4f;

    [Header("Visual")]
    [Tooltip("Optional trail renderer to enable on spawn.")]
    [SerializeField] private TrailRenderer trail;

    [Header("Ground Detection")]
    [SerializeField] private ContactFilter2D groundContactFilter;

    /// <summary>Assign from EnemyBomber.DropBomb() right after Instantiate.</summary>
    [HideInInspector] public EnemyBomber parentBomber;

    private Rigidbody2D rb;

    // ──────────────────────────────────────────────────────────────────────

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

            // 1. Tumbling spin on release
            rb.AddTorque(torqueAmount);

            // 2. Flip orientation when parent bomber is flying left
            if (parentBomber != null && parentBomber.EstimatedVelocity.x < 0f)
            {
                rb.SetRotation(leftFacingSpawnRotation);
            }
        }

        if (trail != null)
        {
            trail.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        // 3. Ground-proximity nose-dive: the closer to ground, the faster it straightens.
        if (rb == null) return;

        RaycastHit2D[] results = new RaycastHit2D[10];
        int hitCount = Physics2D.Raycast(transform.position, Vector2.down, groundContactFilter, results);

        for (int i = 0; i < hitCount; i++)
        {
            if (results[i].collider != null && results[i].transform.gameObject.isStatic)
            {
                float distance = results[i].distance;
                float alpha = noseDiveStrength / Mathf.Min(distance, 1f);
                rb.SetRotation(rb.rotation * (1f - alpha));
                break;
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Explode();
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Auto-destroy if no ExplosionDestroyer script is on the prefab
          //  if (explosion.GetComponent<ExplosionDestroyer>() == null)
            //{
              //  Destroy(explosion, explosionLifetime);
            //}
        }

        Destroy(gameObject);
    }
}
