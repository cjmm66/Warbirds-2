using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Displays Victory or Defeat screen with score summary.
/// Listens to GameStateManager.onStateChanged.
/// Attach to a UI panel that starts disabled.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text ratingText;
    [SerializeField] private TMP_Text detailsText;

    [Header("Display Text")]
    [SerializeField] private string victoryTitle = "VICTORY";
    [SerializeField] private string defeatTitle = "DEFEATED";

    [Header("Star Characters")]
    [SerializeField] private string filledStar = "★";
    [SerializeField] private string emptyStar = "☆";

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.onStateChanged.AddListener(OnGameStateChanged);
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Victory)
        {
            ShowGameOver(true);
        }
        else if (state == GameState.Defeat)
        {
            ShowGameOver(false);
        }
    }

    private void ShowGameOver(bool isVictory)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = isVictory ? victoryTitle : defeatTitle;
            titleText.color = isVictory ? Color.yellow : Color.red;
        }

        int score = 0;
        int stars = 1;

        if (ScoreManager.Instance != null)
        {
            score = ScoreManager.Instance.Score;
            stars = ScoreManager.Instance.GetStarRating();
        }

        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score:N0}";
        }

        if (ratingText != null)
        {
            string rating = "";
            for (int i = 0; i < 3; i++)
            {
                rating += i < stars ? filledStar : emptyStar;
            }
            ratingText.text = rating;
        }

        // Show wave/kill details from GameManager
        if (detailsText != null)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                detailsText.text = $"WAVE {gm.CurrentWave}  |  KILLS {gm.TotalKills}";
            }
        }
    }

    /// <summary>
    /// Restart the current scene. Wire to a UI Button's onClick.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Load the Main Menu scene. Wire to a UI Button's onClick.
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
