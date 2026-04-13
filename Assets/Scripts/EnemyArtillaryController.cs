using UnityEngine;
using UnityEngine.Events;

public class EnemyArtillaryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform barrelPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody2D projectilePrefab;
    
    [Header("Firing")]
    [SerializeField] private float shotIntervalSeconds = 2f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float barrelRotateSpeedDegreesPerSecond = 90f;
    [SerializeField] private bool barrelForwardIsUp;

    [Header("Accuracy")]
    [Tooltip("Starts inaccurate. Higher value means wider miss angle.")]
    [SerializeField] private float startInaccuracyDegrees = 20f;
    [SerializeField] private float minInaccuracyDegrees = 2f;
    [SerializeField] private float accuracyIncreasePerShotDegrees = 1.5f;

    [Header("Fight Timer")]
    [SerializeField] private float fightDurationSeconds = 40f;
    [SerializeField] private bool stopFiringWhenTimeExpires = true;
    [SerializeField] private UnityEvent onTimeExpired;

    private float nextShotTime;
    private float currentInaccuracyDegrees;
    private float currentBarrelAngle;
    private float remainingTime;
    private bool timeExpired;

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

        currentInaccuracyDegrees = Mathf.Max(minInaccuracyDegrees, startInaccuracyDegrees);
        remainingTime = Mathf.Max(0f, fightDurationSeconds);

        if (barrelPivot != null)
        {
            currentBarrelAngle = barrelPivot.eulerAngles.z;
        }
    }

    private void Update()
    {
        if (timeExpired)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            HandleTimeExpired();
            if (stopFiringWhenTimeExpires)
            {
                return;
            }
        }

        if (playerTarget == null || barrelPivot == null || firePoint == null || projectilePrefab == null)
        {
            return;
        }

        Vector2 toPlayer = (playerTarget.position - barrelPivot.position);
        float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        if (barrelForwardIsUp)
        {
            targetAngle -= 90f;
        }

        currentBarrelAngle = Mathf.MoveTowardsAngle(
            currentBarrelAngle,
            targetAngle,
            barrelRotateSpeedDegreesPerSecond * Time.deltaTime);
        barrelPivot.rotation = Quaternion.Euler(0f, 0f, currentBarrelAngle);

        if (Time.time >= nextShotTime)
        {
            FireAtPlayer();
            nextShotTime = Time.time + shotIntervalSeconds;
        }
    }

    private void FireAtPlayer()
    {
        Vector2 origin = firePoint.position;
        Vector2 toPlayer = (playerTarget.position - firePoint.position);
        if (toPlayer.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float randomError = Random.Range(-currentInaccuracyDegrees, currentInaccuracyDegrees);
        Vector2 shotDirection = Quaternion.Euler(0f, 0f, randomError) * toPlayer.normalized;

        Rigidbody2D projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.linearVelocity = shotDirection * projectileSpeed;

        currentInaccuracyDegrees = Mathf.Max(
            minInaccuracyDegrees,
            currentInaccuracyDegrees - accuracyIncreasePerShotDegrees);
    }

    private void HandleTimeExpired()
    {
        timeExpired = true;
        onTimeExpired?.Invoke();
        Debug.Log("EnemyArtillaryController: Timer expired.");
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
