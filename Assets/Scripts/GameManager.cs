using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Setup")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave Settings")]
    [SerializeField] private int initialEnemyCount = 3;
    [SerializeField] private int enemyIncreasePerWave = 1;
    [SerializeField] private float spawnDelaySeconds = 0.5f;

    [Header("Spawn Points (Off Screen)")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private float randomYOffset = 1.5f;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawningWave;

    private void Start()
    {
        StartNextWave();
    }

    private void Update()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);

        if (!isSpawningWave && aliveEnemies.Count == 0)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
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
        int enemiesToSpawn = Mathf.Max(1, initialEnemyCount + (currentWave - 1) * enemyIncreasePerWave);
        StartCoroutine(SpawnWave(enemiesToSpawn));
    }

    private IEnumerator SpawnWave(int enemiesToSpawn)
    {
        isSpawningWave = true;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
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
}
