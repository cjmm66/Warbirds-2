using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages enemy wave spawning and evaluates win/loss conditions.
/// Integrates with GameStateManager and player HealthSystem.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Enemy Setup")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave Settings")]
    [SerializeField] private int initialEnemyCount = 3;
    [SerializeField] private int enemyIncreasePerWave = 1;
    [SerializeField] private float spawnDelaySeconds = 0.5f;

    [Header("Win Condition")]
    [Tooltip("Total waves to survive for victory. 0 = endless.")]
    [SerializeField] private int maxWaves = 5;

    [Header("Spawn Points (Off Screen)")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private float randomYOffset = 1.5f;

    [Header("Player Reference")]
    [Tooltip("Assign the player GameObject to listen for death. Auto-finds by 'Player' tag if empty.")]
    [SerializeField] private HealthSystem playerHealth;

    [Header("Events")]
    public UnityEvent<int> onWaveChanged;
    public UnityEvent<int> onEnemyKilled;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWave = 0;
    private int totalKills = 0;
    private bool isSpawningWave;
    private bool gameEnded;

    private void Awake()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
            }
        }
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.onDeath.AddListener(HandlePlayerDeath);
        }

        StartNextWave();
    }

    private void Update()
    {
        if (gameEnded)
        {
            return;
        }

        // Clean up destroyed enemies and count kills
        int previousCount = aliveEnemies.Count;
        aliveEnemies.RemoveAll(enemy => enemy == null);
        int killed = previousCount - aliveEnemies.Count;

        if (killed > 0)
        {
            totalKills += killed;
            onEnemyKilled?.Invoke(totalKills);
        }

        if (!isSpawningWave && aliveEnemies.Count == 0)
        {
            // Check win condition
            if (maxWaves > 0 && currentWave >= maxWaves)
            {
                HandleVictory();
                return;
            }

            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        if (gameEnded)
        {
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("GameManager: Assign at least one enemy prefab.");
            return;
        }

        if (leftSpawnPoint == null || rightSpawnPoint == null)
        {
            Debug.LogWarning("GameManager: Assign both leftSpawnPoint and rightSpawnPoint.");
            return;
        }

        currentWave++;
        onWaveChanged?.Invoke(currentWave);

        int enemiesToSpawn = Mathf.Max(1, initialEnemyCount + (currentWave - 1) * enemyIncreasePerWave);
        StartCoroutine(SpawnWave(enemiesToSpawn));

        Debug.Log($"GameManager: Wave {currentWave} started ({enemiesToSpawn} enemies)");
    }

    private IEnumerator SpawnWave(int enemiesToSpawn)
    {
        isSpawningWave = true;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (gameEnded)
            {
                break;
            }

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject spawnedEnemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
            aliveEnemies.Add(spawnedEnemy);

            if (spawnDelaySeconds > 0f)
            {
                yield return new WaitForSeconds(spawnDelaySeconds);
            }
        }

        isSpawningWave = false;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Transform baseSpawn = Random.value < 0.5f ? leftSpawnPoint : rightSpawnPoint;
        Vector3 pos = baseSpawn.position;
        pos.y += Random.Range(-Mathf.Abs(randomYOffset), Mathf.Abs(randomYOffset));
        return pos;
    }

    private void HandleVictory()
    {
        gameEnded = true;
        Debug.Log($"GameManager: VICTORY! Waves cleared: {currentWave}, Total kills: {totalKills}");

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(GameState.Victory);
        }
    }

    private void HandlePlayerDeath()
    {
        gameEnded = true;
        Debug.Log($"GameManager: DEFEAT. Died on wave {currentWave}, Total kills: {totalKills}");

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(GameState.Defeat);
        }
    }

    /// <summary>Current wave number.</summary>
    public int CurrentWave => currentWave;

    /// <summary>Total enemies killed this mission.</summary>
    public int TotalKills => totalKills;
}
