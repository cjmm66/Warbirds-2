using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Setup")]
    [Tooltip("Add your enemy prefabs here (for example 3 types).")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave Settings")]
    [SerializeField] private int initialEnemyCount = 3;
    [SerializeField] private int enemyIncreasePerWave = 1;

    [Header("Spawn Area")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float horizontalPadding = 0.05f;
    [SerializeField] private float upperHalfMinViewportY = 0.5f;
    [SerializeField] private float upperHalfMaxViewportY = 0.95f;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWave = 0;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Start()
    {
        StartNextWave();
    }

    private void Update()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);

        if (aliveEnemies.Count == 0)
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

        if (targetCamera == null)
        {
            Debug.LogWarning("GameManager: No camera found for spawning.");
            return;
        }

        currentWave++;
        int enemiesToSpawn = Mathf.Max(1, initialEnemyCount + (currentWave - 1) * enemyIncreasePerWave);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 spawnPosition = GetRandomUpperHalfPosition();
            GameObject spawnedEnemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
            aliveEnemies.Add(spawnedEnemy);
        }
    }

    private Vector3 GetRandomUpperHalfPosition()
    {
        float minX = Mathf.Clamp01(horizontalPadding);
        float maxX = Mathf.Clamp01(1f - horizontalPadding);
        float minY = Mathf.Clamp01(upperHalfMinViewportY);
        float maxY = Mathf.Clamp01(upperHalfMaxViewportY);

        if (maxX <= minX)
        {
            minX = 0.05f;
            maxX = 0.95f;
        }

        if (maxY <= minY)
        {
            minY = 0.5f;
            maxY = 0.95f;
        }

        Vector3 viewportPoint = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Mathf.Abs(targetCamera.transform.position.z));

        Vector3 worldPoint = targetCamera.ViewportToWorldPoint(viewportPoint);
        worldPoint.z = 0f;
        return worldPoint;
    }
}
