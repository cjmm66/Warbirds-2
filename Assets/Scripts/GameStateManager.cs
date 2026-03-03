using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Possible game states for the Warbirds2 state machine.
/// </summary>
public enum GameState
{
    MainMenu,
    DefenseGameplay,
    AttackGameplay,
    Paused,
    Victory,
    Defeat
}

/// <summary>
/// Singleton state machine governing overall game flow.
/// Lives across scenes; place on a persistent root GameObject.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Initial State")]
    [SerializeField] private GameState initialState = GameState.DefenseGameplay;

    [Header("Events")]
    public UnityEvent<GameState> onStateChanged;

    private GameState currentState;
    private GameState stateBeforePause;

    /// <summary>The current game state.</summary>
    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetState(initialState);
    }

    /// <summary>
    /// Transition to a new game state.
    /// </summary>
    public void SetState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        GameState previousState = currentState;
        currentState = newState;

        HandleStateTransition(previousState, newState);
        onStateChanged?.Invoke(currentState);

        Debug.Log($"GameState: {previousState} → {newState}");
    }

    /// <summary>
    /// Toggle pause on/off. Saves and restores the previous state.
    /// </summary>
    public void TogglePause()
    {
        if (currentState == GameState.Paused)
        {
            Unpause();
        }
        else if (currentState == GameState.DefenseGameplay || currentState == GameState.AttackGameplay)
        {
            Pause();
        }
    }

    /// <summary>
    /// Pause the game (sets timeScale to 0).
    /// </summary>
    public void Pause()
    {
        if (currentState == GameState.Paused)
        {
            return;
        }

        stateBeforePause = currentState;
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    /// <summary>
    /// Unpause the game (restores timeScale to 1).
    /// </summary>
    public void Unpause()
    {
        if (currentState != GameState.Paused)
        {
            return;
        }

        Time.timeScale = 1f;
        SetState(stateBeforePause);
    }

    /// <summary>
    /// Convenience: check if gameplay is active (not paused, not menu, not end screen).
    /// </summary>
    public bool IsGameplayActive()
    {
        return currentState == GameState.DefenseGameplay || currentState == GameState.AttackGameplay;
    }

    private void HandleStateTransition(GameState from, GameState to)
    {
        switch (to)
        {
            case GameState.Victory:
            case GameState.Defeat:
                Time.timeScale = 0f;
                break;

            case GameState.DefenseGameplay:
            case GameState.AttackGameplay:
                Time.timeScale = 1f;
                break;
        }
    }

    private void OnDestroy()
    {
        // Ensure timeScale is restored if this object is destroyed
        Time.timeScale = 1f;

        if (Instance == this)
        {
            Instance = null;
        }
    }
}
