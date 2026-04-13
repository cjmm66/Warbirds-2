using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton score manager. Tracks score, kill streaks, and mission ratings.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int pointsPerKill = 100;
    [SerializeField] private int streakBonusMultiplier = 50;

    [Header("Mission Rating Thresholds")]
    [SerializeField] private int twoStarThreshold = 500;
    [SerializeField] private int threeStarThreshold = 1000;

    [Header("Events")]
    public UnityEvent<int> onScoreChanged;

    private int score;
    private int currentStreak;
    private float lastKillTime;
    private float streakTimeWindow = 3f;

    /// <summary>Current score.</summary>
    public int Score => score;

    /// <summary>Current kill streak.</summary>
    public int CurrentStreak => currentStreak;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Register an enemy kill. Awards base points + streak bonus.
    /// </summary>
    public void RegisterKill()
    {
        // Streak tracking
        if (Time.time - lastKillTime <= streakTimeWindow)
        {
            currentStreak++;
        }
        else
        {
            currentStreak = 1;
        }

        lastKillTime = Time.time;

        int streakBonus = Mathf.Max(0, (currentStreak - 1) * streakBonusMultiplier);
        int pointsEarned = pointsPerKill + streakBonus;
        score += pointsEarned;

        onScoreChanged?.Invoke(score);

        if (currentStreak > 1)
        {
            Debug.Log($"ScoreManager: +{pointsEarned} (streak x{currentStreak}) | Total: {score}");
        }
    }

    /// <summary>
    /// Add arbitrary points (bonus objectives, accuracy bonuses, etc.).
    /// </summary>
    public void AddPoints(int points)
    {
        if (points <= 0)
        {
            return;
        }

        score += points;
        onScoreChanged?.Invoke(score);
    }

    /// <summary>
    /// Get mission star rating (1-3) based on current score.
    /// </summary>
    public int GetStarRating()
    {
        if (score >= threeStarThreshold)
        {
            return 3;
        }

        if (score >= twoStarThreshold)
        {
            return 2;
        }

        return 1;
    }

    /// <summary>
    /// Reset score for a new mission.
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        currentStreak = 0;
        lastKillTime = 0f;
        onScoreChanged?.Invoke(score);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
