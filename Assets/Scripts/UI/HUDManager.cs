using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime HUD controller. Subscribes to HealthSystem, AmmoSystem,
/// GameManager, and ScoreManager events to update UI elements.
/// Attach to a UI Canvas in the scene.
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Assign the player's HealthSystem.")]
    [SerializeField] private HealthSystem playerHealth;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Ammo")]
    [Tooltip("Assign the player's weapon AmmoSystem.")]
    [SerializeField] private AmmoSystem playerAmmo;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Slider reloadBarSlider;

    [Header("Waves")]
    [Tooltip("Assign the GameManager in the scene.")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text waveText;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        // Subscribe to events
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        if (playerAmmo != null)
        {
            playerAmmo.onAmmoChanged.AddListener(UpdateAmmoUI);
            playerAmmo.onReloadStart.AddListener(OnReloadStart);
            playerAmmo.onReloadComplete.AddListener(OnReloadComplete);
            UpdateAmmoUI(playerAmmo.CurrentAmmo, playerAmmo.MaxAmmo);
        }

        if (gameManager != null)
        {
            gameManager.onWaveChanged.AddListener(UpdateWaveUI);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.onScoreChanged.AddListener(UpdateScoreUI);
            UpdateScoreUI(ScoreManager.Instance.Score);
        }

        // Hide reload bar initially
        if (reloadBarSlider != null)
        {
            reloadBarSlider.gameObject.SetActive(false);
        }
    }

    // ---------- UI Update Methods ----------

    private void UpdateHealthUI(int current, int max)
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = max;
            healthBarSlider.value = current;
        }

        if (healthText != null)
        {
            healthText.text = $"{current}/{max}";
        }
    }

    private void UpdateAmmoUI(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{current}/{max}";
        }
    }

    private void OnReloadStart()
    {
        if (ammoText != null)
        {
            ammoText.text = "RELOADING...";
        }

        if (reloadBarSlider != null)
        {
            reloadBarSlider.gameObject.SetActive(true);
            reloadBarSlider.value = 0f;
        }
    }

    private void OnReloadComplete()
    {
        if (reloadBarSlider != null)
        {
            reloadBarSlider.gameObject.SetActive(false);
        }
    }

    private void UpdateWaveUI(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"WAVE {wave}";
        }
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("N0");
        }
    }

    private void Update()
    {
        // Animate reload bar if reloading
        if (playerAmmo != null && playerAmmo.IsReloading && reloadBarSlider != null)
        {
            reloadBarSlider.value += Time.deltaTime;
        }
    }
}
