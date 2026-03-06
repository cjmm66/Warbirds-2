using UnityEngine;

/// <summary>
/// Controls pause menu visibility and pause input.
/// Listens to GameStateManager.onStateChanged.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject pauseButton;

    

    private void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.onStateChanged.AddListener(OnGameStateChanged);
            OnGameStateChanged(GameStateManager.Instance.CurrentState);
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.onStateChanged.RemoveListener(OnGameStateChanged);
        }
    }

    private void Update()
    {
       
    }

    private void OnGameStateChanged(GameState state)
    {
        if (pauseMenuPanel != null)
        {
            pauseButton.SetActive(state == GameState.DefenseGameplay || state == GameState.AttackGameplay);
            pauseMenuPanel.SetActive(state == GameState.Paused);
        }
    }

    /// <summary>
    /// Resume gameplay from pause. Wire to Resume button.
    /// </summary>

public void PauseGame()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.Pause();
        }
    }
    public void ResumeGame()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.Unpause();
        }
    }

    /// <summary>
    /// Return to Main Menu from pause. Wire to Main Menu button.
    /// </summary>
    public void QuitToMainMenu()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(GameState.MainMenu);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadMainMenu();
        }
    }
}
