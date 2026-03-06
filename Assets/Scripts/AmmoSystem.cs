using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages ammunition count and reload timing.
/// Attach alongside any weapon controller (AAGunController, ArtillaryController).
/// </summary>
public class AmmoSystem : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo;

    [Header("Reload")]
    [SerializeField] private float reloadTimeSeconds = 2f;
    [SerializeField] private bool autoReloadWhenEmpty = true;

    [Header("Events")]
    [Tooltip("Fires (currentAmmo, maxAmmo) whenever ammo count changes.")]
    public UnityEvent<int, int> onAmmoChanged;
    public UnityEvent onReloadStart;
    public UnityEvent onReloadComplete;

    private bool isReloading;

    /// <summary>Current ammo count.</summary>
    public int CurrentAmmo => currentAmmo;

    /// <summary>Maximum ammo count.</summary>
    public int MaxAmmo => maxAmmo;

    /// <summary>True while a reload is in progress.</summary>
    public bool IsReloading => isReloading;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    private void Start()
    {
        onAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    /// <summary>
    /// Attempt to consume one unit of ammo.
    /// Returns true if ammo was available, false if empty or reloading.
    /// </summary>
    public bool TryConsumeAmmo()
    {
        if (isReloading || currentAmmo <= 0)
        {
            return false;
        }

        currentAmmo--;
        onAmmoChanged?.Invoke(currentAmmo, maxAmmo);

        if (currentAmmo <= 0 && autoReloadWhenEmpty)
        {
            StartReload();
        }

        return true;
    }

    /// <summary>
    /// Manually trigger a reload (e.g. from a reload button).
    /// </summary>
    public void StartReload()
    {
        if (isReloading || currentAmmo >= maxAmmo)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        onReloadStart?.Invoke();

        yield return new WaitForSeconds(reloadTimeSeconds);

        currentAmmo = maxAmmo;
        isReloading = false;
        onAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        onReloadComplete?.Invoke();
    }

    /// <summary>
    /// Immediately refill ammo without reload delay (e.g. ammo pickup).
    /// </summary>
    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        StopAllCoroutines();
        onAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    /// <summary>
    /// Returns ammo as 0-1 ratio.
    /// </summary>
    public float GetAmmoPercent()
    {
        if (maxAmmo <= 0)
        {
            return 0f;
        }

        return (float)currentAmmo / maxAmmo;
    }
}
