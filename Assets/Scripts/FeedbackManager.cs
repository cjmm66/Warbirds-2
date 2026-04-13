using System.Collections;
using UnityEngine;

/// <summary>
/// Handles game-feel feedback: screen shake, damage flash, hit markers.
/// Attach to the main camera or a persistent manager GameObject.
/// </summary>
public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Screen Shake")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.2f;
    [SerializeField] private Transform cameraTransform;

    [Header("Damage Flash")]
    [Tooltip("Optional CanvasGroup for a red damage vignette overlay.")]
    [SerializeField] private CanvasGroup damageFlashOverlay;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float flashMaxAlpha = 0.5f;

    [Header("Hit Marker")]
    [Tooltip("Optional prefab to spawn at world hit position.")]
    [SerializeField] private GameObject hitMarkerPrefab;
    [SerializeField] private float hitMarkerLifetime = 0.5f;

    [Header("Kill Confirmation")]
    [Tooltip("Optional prefab for 'Enemy Down!' popup text.")]
    [SerializeField] private GameObject killConfirmPrefab;
    [SerializeField] private float killConfirmLifetime = 1f;

    private Vector3 originalCameraPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }

        if (damageFlashOverlay != null)
        {
            damageFlashOverlay.alpha = 0f;
        }
    }

    // ---------- Public API ----------

    /// <summary>
    /// Trigger a screen shake effect (e.g. on player damage or explosion).
    /// </summary>
    public void TriggerScreenShake()
    {
        TriggerScreenShake(shakeDuration, shakeMagnitude);
    }

    /// <summary>
    /// Trigger a screen shake with custom duration and magnitude.
    /// </summary>
    public void TriggerScreenShake(float duration, float magnitude)
    {
        if (cameraTransform == null)
        {
            return;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            cameraTransform.localPosition = originalCameraPosition;
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    /// <summary>
    /// Flash a red damage vignette overlay.
    /// </summary>
    public void TriggerDamageFlash()
    {
        if (damageFlashOverlay == null)
        {
            return;
        }

        StartCoroutine(DamageFlashCoroutine());
    }

    /// <summary>
    /// Spawn a hit marker at a world-space position.
    /// </summary>
    public void SpawnHitMarker(Vector3 worldPosition)
    {
        if (hitMarkerPrefab == null)
        {
            return;
        }

        GameObject marker = Instantiate(hitMarkerPrefab, worldPosition, Quaternion.identity);
        Destroy(marker, hitMarkerLifetime);
    }

    /// <summary>
    /// Show a kill confirmation popup at a world-space position.
    /// </summary>
    public void SpawnKillConfirm(Vector3 worldPosition)
    {
        if (killConfirmPrefab == null)
        {
            return;
        }

        GameObject popup = Instantiate(killConfirmPrefab, worldPosition, Quaternion.identity);
        Destroy(popup, killConfirmLifetime);
    }

    /// <summary>
    /// Play an audio clip (fire, hit, explosion, etc.) at a position.
    /// </summary>
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    // ---------- Coroutines ----------

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cameraTransform.localPosition = originalCameraPosition + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalCameraPosition;
        shakeCoroutine = null;
    }

    private IEnumerator DamageFlashCoroutine()
    {
        damageFlashOverlay.alpha = flashMaxAlpha;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            damageFlashOverlay.alpha = Mathf.Lerp(flashMaxAlpha, 0f, elapsed / flashDuration);
            yield return null;
        }

        damageFlashOverlay.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
