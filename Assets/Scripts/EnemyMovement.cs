using UnityEngine;

/// <summary>
/// Flight pattern types for enemy aircraft.
/// </summary>
public enum FlightPattern
{
    Horizontal,
    DiveBomb,
    Zigzag
}

/// <summary>
/// Controls enemy aircraft movement with multiple flight patterns.
/// Integrates with HealthSystem for death state.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool startMovingRight = true;
    [Tooltip("If enabled, initial direction is chosen from spawn side: left spawns move right, right spawns move left.")]
    [SerializeField] private bool autoDirectionFromSpawnSide = true;
    [Tooltip("Set true if your sprite naturally faces right when localScale.x is positive.")]
    [SerializeField] private bool spriteFacesRightAtPositiveScale = false;

    [Header("Flight Pattern")]
    [SerializeField] private FlightPattern flightPattern = FlightPattern.Horizontal;

    [Header("Zigzag Settings")]
    [Tooltip("Vertical amplitude of the zigzag oscillation.")]
    [SerializeField] private float zigzagAmplitude = 1.5f;
    [Tooltip("Speed of the zigzag oscillation.")]
    [SerializeField] private float zigzagFrequency = 2f;

    [Header("Dive Bomb Settings")]
    [Tooltip("How far the aircraft descends during a dive.")]
    [SerializeField] private float diveDepth = 3f;
    [Tooltip("Duration of one dive cycle in seconds.")]
    [SerializeField] private float diveCycleDuration = 3f;
    [Tooltip("Auto-finds Player tag if empty.")]
    [SerializeField] private Transform diveTarget;

    [Header("Turn Around Limits")]
    [Tooltip("How far past the camera edge the plane goes before flipping direction.")]
    [SerializeField] private float extraOffscreenDistance = 2f;
    [Tooltip("Optional fixed world X limit. If > 0, this is used instead of camera bounds.")]
    [SerializeField] private float fixedWorldXLimit = 0f;
    [SerializeField] private GameObject targetCamera;

    private float direction = 1f;
    private float baseYPosition;
    private float zigzagTimer;
    private float diveTimer;
    private bool isDiving;
    private HealthSystem healthSystem;

    private void Awake()
    {
        if (targetCamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                targetCamera = mainCam.gameObject;
            }
        }

        direction = startMovingRight ? 1f : -1f;
        if (autoDirectionFromSpawnSide)
        {
            float centerX = 0f;
            if (targetCamera != null)
            {
                centerX = targetCamera.transform.position.x;
            }

            direction = transform.position.x <= centerX ? 1f : -1f;
        }

        if (diveTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                diveTarget = player.transform;
            }
        }

        healthSystem = GetComponent<HealthSystem>();
        baseYPosition = transform.position.y;

        ApplyFacing();
    }

    private void Start()
    {
        // If HealthSystem is attached, listen for death to trigger destruction
        if (healthSystem != null)
        {
            healthSystem.onDeath.AddListener(HandleDeath);
        }
    }

    private void Update()
    {
        // Stop moving if dead
        if (healthSystem != null && healthSystem.IsDead)
        {
            return;
        }

        switch (flightPattern)
        {
            case FlightPattern.Horizontal:
                MoveHorizontal();
                break;
            case FlightPattern.Zigzag:
                MoveZigzag();
                break;
            case FlightPattern.DiveBomb:
                MoveDiveBomb();
                break;
        }

        CheckTurnAround();
    }

    // ---------- Movement Patterns ----------

    private void MoveHorizontal()
    {
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime), Space.World);
    }

    private void MoveZigzag()
    {
        // Horizontal movement
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime), Space.World);

        // Sinusoidal vertical oscillation
        zigzagTimer += Time.deltaTime * zigzagFrequency;
        float yOffset = Mathf.Sin(zigzagTimer) * zigzagAmplitude;

        Vector3 pos = transform.position;
        pos.y = baseYPosition + yOffset;
        transform.position = pos;
    }

    private void MoveDiveBomb()
    {
        // Horizontal movement
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime), Space.World);

        // Periodic dive towards the ground and pull-up
        diveTimer += Time.deltaTime;
        float cycleProgress = (diveTimer % diveCycleDuration) / diveCycleDuration;

        // Smooth dive: down in first half, up in second half (sine curve)
        float diveOffset = Mathf.Sin(cycleProgress * Mathf.PI) * diveDepth;

        Vector3 pos = transform.position;
        pos.y = baseYPosition - diveOffset;
        transform.position = pos;
    }

    // ---------- Turn Around ----------

    private void CheckTurnAround()
    {
        float xLimit = GetTurnAroundXLimit();
        if (xLimit <= 0f)
        {
            return;
        }

        if (direction > 0f && transform.position.x >= xLimit)
        {
            FlipDirection();
        }
        else if (direction < 0f && transform.position.x <= -xLimit)
        {
            FlipDirection();
        }
    }

    private float GetTurnAroundXLimit()
    {
        if (fixedWorldXLimit > 0f)
        {
            return fixedWorldXLimit;
        }

        if (targetCamera == null)
        {
            return 0f;
        }

        Vector3 rightEdgeWorld = targetCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1f, 0.5f, Mathf.Abs(targetCamera.transform.position.z)));
        return Mathf.Abs(rightEdgeWorld.x) + Mathf.Max(0f, extraOffscreenDistance);
    }

    private void FlipDirection()
    {
        direction *= -1f;
        ApplyFacing();
    }

    private void ApplyFacing()
    {
        Vector3 scale = transform.localScale;
        float facingSign = spriteFacesRightAtPositiveScale ? Mathf.Sign(direction) : -Mathf.Sign(direction);
        scale.x = Mathf.Abs(scale.x) * facingSign;
        transform.localScale = scale;
    }

    // ---------- Death ----------

    private void HandleDeath()
    {
        // Could trigger explosion effect here in the future
        Debug.Log($"EnemyMovement: {gameObject.name} destroyed!");
        Destroy(gameObject);
    }

    /// <summary>
    /// Set the flight pattern at runtime (e.g., GameManager sets this based on wave).
    /// </summary>
    public void SetFlightPattern(FlightPattern pattern)
    {
        flightPattern = pattern;
    }
}
