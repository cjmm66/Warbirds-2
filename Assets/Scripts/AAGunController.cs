using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AAGunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform barrelPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody2D projectilePrefab;

    [Header("Ammo")]
    [Tooltip("Optional. If assigned, ammo is consumed per shot.")]
    [SerializeField] private AmmoSystem ammoSystem;

    [Header("Firing")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private bool barrelForwardIsUp;
    [SerializeField] private float shotDelaySeconds = 0.25f;

    [Header("Touch Dead Zone")]
    [Tooltip("Bottom area of the screen that will ignore firing input. 0.5 means lower half.")]
    [SerializeField] private float lowerDeadZoneHeightPercent = 0.5f;

    private float nextAllowedShotTime;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (!TryGetPointerDownPosition(out Vector2 touchScreenPosition))
        {
            return;
        }

        if (targetCamera == null || barrelPivot == null || firePoint == null || projectilePrefab == null)
        {
            Debug.LogWarning("AAGunController: Assign camera, barrelPivot, firePoint, and projectilePrefab in Inspector.");
            return;
        }

        if (IsTouchOverUi())
        {
            return;
        }

        if (IsInLowerDeadZone(touchScreenPosition))
        {
            return;
        }

        if (Time.time < nextAllowedShotTime)
        {
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
            return;
        }

        Vector2 touchWorldPosition = targetCamera.ScreenToWorldPoint(touchScreenPosition);
        RotateBarrelToPoint(touchWorldPosition);
        FireTowards(touchWorldPosition);
        nextAllowedShotTime = Time.time + shotDelaySeconds;
    }

    private bool TryGetPointerDownPosition(out Vector2 touchPosition)
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                touchPosition = touch.position.ReadValue();
                return true;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            touchPosition = Mouse.current.position.ReadValue();
            return true;
        }
#endif

        touchPosition = default;
        return false;
    }

    private bool IsTouchOverUi()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        if (Touchscreen.current != null)
        {
            int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            return EventSystem.current.IsPointerOverGameObject(touchId);
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsInLowerDeadZone(Vector2 screenPosition)
    {
        float clampedPercent = Mathf.Clamp01(lowerDeadZoneHeightPercent);
        float deadZoneTopY = Screen.height * clampedPercent;
        return screenPosition.y <= deadZoneTopY;
    }

    private void RotateBarrelToPoint(Vector2 targetPoint)
    {
        Vector2 from = barrelPivot.position;
        Vector2 direction = targetPoint - from;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (barrelForwardIsUp)
        {
            angle -= 90f;
        }

        barrelPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FireTowards(Vector2 targetPoint)
    {
        Vector2 from = firePoint.position;
        Vector2 direction = (targetPoint - from).normalized;
        Rigidbody2D projectile = Instantiate(projectilePrefab, from, Quaternion.identity);
        projectile.linearVelocity = direction * projectileSpeed;
    }
}
