using UnityEngine;
using System.Collections.Generic;
using System;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.8f;
    [SerializeField] private float intervalDecreaseRate = 0.05f;

    [Header("Object Pooling")]
    [SerializeField] private int poolSize = 20;

    private List<GameObject> obstaclePool = new List<GameObject>();
    private float spawnTimer = 0f;
    private float currentSpawnInterval;
    private bool isSpawning = false;

    private void Start()
    {
        spawnParent = transform;
        InitializePool();
        currentSpawnInterval = spawnInterval;
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= GameManager_OnGameStart;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab, spawnParent);
            obj.SetActive(false);
            obstaclePool.Add(obj);
        }
    }

    private void Update()
    {
        if (!isSpawning || !GameManager.Instance.IsGameActive)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;

            // Decrease spawn interval over time
            if (currentSpawnInterval > minSpawnInterval)
            {
                currentSpawnInterval -= intervalDecreaseRate;
            }
        }
    }

    private void GameManager_OnGameStart(object sender, EventArgs e)
    {
        StartSpawning();
    }

    private void GameManager_OnGameOver(object sender, bool e)
    {
        StopSpawning();
    }

    private void SpawnObstacle()
    {
        GameObject obstacle = GetPooledObstacle();

        if (obstacle != null)
        {
            obstacle.transform.position = spawnPoint.position;
            obstacle.SetActive(true);
        }
    }

    private GameObject GetPooledObstacle()
    {
        foreach (GameObject obj in obstaclePool)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }

        // If no inactive objects, expand pool
        GameObject newObj = Instantiate(obstaclePrefab, spawnParent);
        newObj.SetActive(false);
        obstaclePool.Add(newObj);
        return newObj;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnTimer = 0f;
        currentSpawnInterval = spawnInterval;
        DeactivateAll();
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private void DeactivateAll()
    {
        foreach (GameObject obj in obstaclePool)
        {
            obj.SetActive(false);
        }
    }
}