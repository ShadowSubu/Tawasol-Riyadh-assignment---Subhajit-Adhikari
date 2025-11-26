using UnityEngine;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnHeightMin = 1f;
    [SerializeField] private float spawnHeightMax = 3f;

    [Header("Object Pooling")]
    [SerializeField] private int poolSize = 30;

    private List<GameObject> collectiblePool = new List<GameObject>();
    private float spawnTimer = 0f;
    private bool isSpawning = false;

    private void Start()
    {
        spawnParent = transform;
        InitializePool();
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= GameManager_OnGameStart;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(object sender, bool e)
    {
        StopSpawning();
    }

    private void GameManager_OnGameStart(object sender, System.EventArgs e)
    {
        StartSpawning();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(collectiblePrefab, spawnParent);
            obj.SetActive(false);
            collectiblePool.Add(obj);
        }
    }

    private void Update()
    {
        if (!isSpawning || !GameManager.Instance.IsGameActive)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnCollectible();
            spawnTimer = 0f;
        }
    }

    private void SpawnCollectible()
    {
        GameObject collectible = GetPooledCollectible();

        if (collectible != null)
        {
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.y = Random.Range(spawnHeightMin, spawnHeightMax);

            collectible.transform.position = spawnPos;
            collectible.SetActive(true);
        }
    }

    private GameObject GetPooledCollectible()
    {
        foreach (GameObject obj in collectiblePool)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }

        // If no inactive objects, expand pool
        GameObject newObj = Instantiate(collectiblePrefab, spawnParent);
        newObj.SetActive(false);
        collectiblePool.Add(newObj);
        return newObj;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnTimer = 0f;
        DeactivateAll();
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private void DeactivateAll()
    {
        foreach (GameObject obj in collectiblePool)
        {
            obj.SetActive(false);
        }
    }
}