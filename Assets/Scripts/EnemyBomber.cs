using UnityEngine;

/// <summary>
/// Enemy aircraft bombing behaviour. Detects when the player is
/// within drop range and releases EnemyBomb projectiles at intervals.
/// Attach alongside EnemyMovement and HealthSystem on enemy prefab.
/// </summary>
public class EnemyBomber : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the bomb prefab (must have Rigidbody2D and EnemyBomb).")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombDropPoint;

    [Header("Targeting")]
    [Tooltip("Auto-finds Player tag if empty.")]
    [SerializeField] private Transform playerTarget;
    [Tooltip("Horizontal distance within which the bomber will drop bombs.")]
    [SerializeField] private float bombDropRange = 3f;

    [Header("Timing")]
    [SerializeField] private float bombDropInterval = 2f;
    [Tooltip("Random variance added to interval. Final = interval ± variance.")]
    [SerializeField] private float intervalVariance = 0.5f;

    [Header("Bomb Physics")]
    [Tooltip("Optional initial downward speed for the bomb.")]
    [SerializeField] private float bombInitialDownSpeed = 0.35f;
    [Tooltip("How much of the bomber velocity is inherited by the bomb on release.")]
    [SerializeField] private float inheritedVelocityMultiplier = 1f;

    private float nextDropTime;
    private HealthSystem healthSystem;
    private Vector3 previousPosition;
    private Vector2 estimatedVelocity;

    private void Awake()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        healthSystem = GetComponent<HealthSystem>();

        if (bombDropPoint == null)
        {
            bombDropPoint = transform;
        }

        // Stagger initial drop so all bombers don't fire simultaneously
        nextDropTime = Time.time + Random.Range(0.5f, bombDropInterval);
        previousPosition = transform.position;
    }

    private void Update()
    {
        UpdateEstimatedVelocity();

        if (playerTarget == null || bombPrefab == null)
        {
            return;
        }

        // Don't bomb if dead
        if (healthSystem != null && healthSystem.IsDead)
        {
            return;
        }

        if (Time.time < nextDropTime)
        {
            return;
        }

        // Check if player is within horizontal drop range
        float horizontalDistance = Mathf.Abs(transform.position.x - playerTarget.position.x);
        if (horizontalDistance > bombDropRange)
        {
            return;
        }

        // Only drop if above the player
        if (transform.position.y <= playerTarget.position.y)
        {
            return;
        }

        DropBomb();
        float variance = Random.Range(-intervalVariance, intervalVariance);
        nextDropTime = Time.time + bombDropInterval + variance;
    }

    private void UpdateEstimatedVelocity()
    {
        float dt = Time.deltaTime;
        if (dt > 0f)
        {
            Vector3 delta = transform.position - previousPosition;
            estimatedVelocity = new Vector2(delta.x / dt, delta.y / dt);
        }

        previousPosition = transform.position;
    }

    private void DropBomb()
    {
        Vector3 spawnPos = bombDropPoint.position;
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        // Add downward velocity + inherit horizontal movement from parent
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        if (bombRb != null)
        {
            Rigidbody2D parentRb = GetComponent<Rigidbody2D>();
            Vector2 inheritedVelocity = estimatedVelocity;
            if (parentRb != null && parentRb.linearVelocity.sqrMagnitude > 0.001f)
            {
                inheritedVelocity = parentRb.linearVelocity;
            }

            bombRb.linearVelocity = (inheritedVelocity * inheritedVelocityMultiplier) + Vector2.down * bombInitialDownSpeed;
        }

        Debug.Log("EnemyBomber: Bomb dropped!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = bombDropPoint != null ? bombDropPoint.position : transform.position;
        Gizmos.DrawWireCube(pos, new Vector3(bombDropRange * 2f, 0.2f, 0f));
    }
}
