using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shell types for artillery. Expand as needed.
/// </summary>
public enum ShellType
{
    Standard,
    HighExplosive,
    Smoke
}

public class ArtillaryController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider angleSlider;
    [SerializeField] private Slider powerSlider;

    [Header("Launch Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody2D projectilePrefab;

    [Header("Angle (degrees)")]
    [SerializeField] private float minAngle = 5f;
    [SerializeField] private float maxAngle = 80f;

    [Header("Power")]
    [SerializeField] private float minPower = 5f;
    [SerializeField] private float maxPower = 100f;

    [Header("Barrel Visual")]
    [SerializeField] private Transform barrelPivot;
    [SerializeField] private bool barrelForwardIsUp;
    [SerializeField] private float barrelRotateSpeedDegreesPerSecond = 120f;

    [Header("Aim Cursor")]
    [SerializeField] private Transform aimCursor;
    [SerializeField] private float aimCursorDistance = 8f;

    [Header("Delayed Shot")]
    [SerializeField] private float fireDelaySeconds = 3f;
    [SerializeField] private float constantProjectileSpeed = 15f;

    [Header("Recoil")]
    [SerializeField] private Rigidbody2D recoilBody;
    [SerializeField] private float recoilImpulse = 0.6f;

    [Header("Ammo")]
    [Tooltip("Optional. If assigned, shells are consumed per shot.")]
    [SerializeField] private AmmoSystem ammoSystem;

    [Header("Shell Selection")]
    [SerializeField] private ShellType currentShellType = ShellType.Standard;

    [Header("Prediction")]
    [SerializeField] private float predictionStep = 0.05f;
    [SerializeField] private float maxPredictionTime = 8f;
    [SerializeField] private LayerMask predictionHitMask = ~0;

    private Coroutine pendingFire;
    private float currentBarrelAngle;

    private void Start()
    {
        currentBarrelAngle = GetAngle();
    }

    private void Update()
    {
        if (barrelPivot == null)
        {
            return;
        }

        float targetAngle = GetAngle();
        currentBarrelAngle = Mathf.MoveTowardsAngle(
            currentBarrelAngle,
            targetAngle,
            barrelRotateSpeedDegreesPerSecond * Time.deltaTime);

        barrelPivot.localRotation = Quaternion.Euler(0f, 0f, currentBarrelAngle);
        UpdateAimCursor();
    }

    public void Fire()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("ArtillaryController: Assign projectilePrefab and firePoint in Inspector.");
            return;
        }

        // Block firing if game is not active
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive())
        {
            return;
        }

        // Consume ammo if AmmoSystem is assigned
        if (ammoSystem != null && !ammoSystem.TryConsumeAmmo())
        {
            Debug.Log("ArtillaryController: No ammo remaining.");
            return;
        }

        Vector2 origin = firePoint.position;
        Vector2 launchDirection = GetLaunchDirection();
        float launchSpeedFromPower = GetPower();

        List<Vector2> predictedPath = CalculatePredictedPath(origin, launchDirection * launchSpeedFromPower, out Vector2 predictedHitPoint);
        Debug.Log($"Predicted hit point: {predictedHitPoint} | Shell: {currentShellType}");

        if (pendingFire != null)
        {
            StopCoroutine(pendingFire);
        }

        pendingFire = StartCoroutine(FireAfterDelay(predictedPath, launchDirection));
    }

    /// <summary>
    /// Set the current shell type (called from UI shell selection button).
    /// </summary>
    public void SetShellType(ShellType shellType)
    {
        currentShellType = shellType;
    }

    /// <summary>
    /// Get the current shell type.
    /// </summary>
    public ShellType GetShellType()
    {
        return currentShellType;
    }

    private IEnumerator FireAfterDelay(List<Vector2> path, Vector2 launchDirection)
    {
        yield return new WaitForSeconds(fireDelaySeconds);
        pendingFire = null;

        if (projectilePrefab == null || firePoint == null || path == null || path.Count < 2)
        {
            yield break;
        }

        Rigidbody2D projectile = Instantiate(projectilePrefab, path[0], firePoint.rotation);
        projectile.gravityScale = 0f;
        projectile.linearVelocity = Vector2.zero;
        projectile.angularVelocity = 0f;
        projectile.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(MoveProjectileAlongPath(projectile, path));
        ApplyRecoil(launchDirection);
    }

    private IEnumerator MoveProjectileAlongPath(Rigidbody2D projectile, List<Vector2> path)
    {
        if (projectile == null || path == null || path.Count < 2)
        {
            yield break;
        }

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 target = path[i];

            while (projectile != null && Vector2.Distance(projectile.position, target) > 0.001f)
            {
                Vector2 current = projectile.position;
                Vector2 next = Vector2.MoveTowards(current, target, constantProjectileSpeed * Time.deltaTime);
                Vector2 moveDir = next - current;

                projectile.MovePosition(next);

                if (moveDir.sqrMagnitude > 0.000001f)
                {
                    float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                    projectile.MoveRotation(angle);
                }

                yield return null;
            }
        }
    }

    private List<Vector2> CalculatePredictedPath(Vector2 startPosition, Vector2 initialVelocity, out Vector2 hitPoint)
    {
        List<Vector2> points = new List<Vector2> { startPosition };

        Vector2 gravity = Physics2D.gravity * GetPredictionGravityScale();
        Vector2 position = startPosition;
        Vector2 velocity = initialVelocity;

        float elapsed = 0f;
        while (elapsed < maxPredictionTime)
        {
            Vector2 nextPosition = position + velocity * predictionStep + 0.5f * gravity * predictionStep * predictionStep;
            RaycastHit2D hit = Physics2D.Linecast(position, nextPosition, predictionHitMask);

            if (hit.collider != null)
            {
                points.Add(hit.point);
                hitPoint = hit.point;
                return points;
            }

            points.Add(nextPosition);
            velocity += gravity * predictionStep;
            position = nextPosition;
            elapsed += predictionStep;
        }

        hitPoint = position;
        return points;
    }

    private float GetPredictionGravityScale()
    {
        if (projectilePrefab == null)
        {
            return 1f;
        }

        return projectilePrefab.gravityScale;
    }

    private Vector2 GetLaunchDirection()
    {
        if (firePoint == null)
        {
            return Vector2.right;
        }

        return barrelForwardIsUp ? (Vector2)firePoint.up : (Vector2)firePoint.right;
    }

    private float GetAngle()
    {
        if (angleSlider == null)
        {
            return minAngle;
        }

        return Mathf.Lerp(minAngle, maxAngle, angleSlider.value);
    }

    private float GetPower()
    {
        if (powerSlider == null)
        {
            return minPower;
        }

        return Mathf.Lerp(minPower, maxPower, powerSlider.value);
    }

    private void UpdateAimCursor()
    {
        if (aimCursor == null || barrelPivot == null)
        {
            return;
        }

        Vector3 origin = firePoint != null ? firePoint.position : barrelPivot.position;
        Vector3 forward = barrelForwardIsUp ? barrelPivot.up : barrelPivot.right;
        aimCursor.position = origin + forward * aimCursorDistance;
    }

    private void ApplyRecoil(Vector2 launchDirection)
    {
        if (recoilBody == null || recoilImpulse <= 0f)
        {
            return;
        }

        Vector2 recoilDirection = -launchDirection.normalized;
        recoilBody.AddForce(recoilDirection * recoilImpulse, ForceMode2D.Impulse);
    }
}
