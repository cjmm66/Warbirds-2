using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene transitions between Defense and Attack modes,
/// handles loading screens and data passing between scenes.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string defenseScene = "Prototype1";
    [SerializeField] private string attackScene = "ArtillaryPrototype";

    [Header("Transition")]
    [Tooltip("Optional CanvasGroup for fade-out transition.")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeDuration = 0.5f;

    /// <summary>Score carried over from previous scene.</summary>
    public int CarriedScore { get; private set; }

    /// <summary>Current mission / level index.</summary>
    public int CurrentLevel { get; private set; } = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Load the Main Menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Load a Defense mode scene.
    /// </summary>
    public void LoadDefenseLevel()
    {
        Time.timeScale = 1f;

        if (ScoreManager.Instance != null)
        {
            CarriedScore = ScoreManager.Instance.Score;
        }

        SceneManager.LoadScene(defenseScene);
    }

    /// <summary>
    /// Load an Attack mode scene.
    /// </summary>
    public void LoadAttackLevel()
    {
        Time.timeScale = 1f;

        if (ScoreManager.Instance != null)
        {
            CarriedScore = ScoreManager.Instance.Score;
        }

        SceneManager.LoadScene(attackScene);
    }

    /// <summary>
    /// Restart the current scene.
    /// </summary>
    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Advance to the next level. Alternates Defense ↔ Attack.
    /// </summary>
    public void LoadNextLevel()
    {
        CurrentLevel++;

        // Alternate between defense and attack modes
        if (CurrentLevel % 2 == 1)
        {
            LoadDefenseLevel();
        }
        else
        {
            LoadAttackLevel();
        }
    }

    /// <summary>
    /// Load a specific scene by name.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
