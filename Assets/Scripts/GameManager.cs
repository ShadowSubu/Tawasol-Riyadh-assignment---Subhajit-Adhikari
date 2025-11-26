using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private float gameSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.5f;
    [SerializeField] private float maxSpeed = 20f;

    [Header("Score")]
    private int playerScore = 0;
    private int enemyScore = 0;
    private int distanceScore = 0;
    private float distanceCounter = 0f;

    public event EventHandler OnGameStart;
    public event EventHandler<bool> OnGameOver;
    public event EventHandler<int> OnPlayerScoreUpdate;
    public event EventHandler<int> OnEnemyScoreUpdate;
    public event EventHandler OnPlayerOrbCollected;
    public event EventHandler OnEnemyOrbCollected;

    public bool IsGameActive => isGameActive;
    public float GameSpeed => gameSpeed;
    public int PlayerScore => playerScore;

    private ParticleEffect particleEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isGameActive = false;
        particleEffect = FindAnyObjectByType<ParticleEffect>();
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (isGameActive)
        {
            // Increase speed over time
            if (gameSpeed < maxSpeed)
            {
                gameSpeed += speedIncreaseRate * Time.deltaTime;
            }

            // Update distance score
            distanceCounter += gameSpeed * Time.deltaTime;
            if (distanceCounter >= 1f)
            {
                distanceScore++;
                distanceCounter = 0f;
                AddScore(1);
            }

            OnPlayerScoreUpdate?.Invoke(this, playerScore);
            OnEnemyScoreUpdate?.Invoke(this, enemyScore);
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        gameSpeed = 5f;
        playerScore = 0;
        enemyScore = 0;
        distanceScore = 0;
        distanceCounter = 0f;

        OnGameStart?.Invoke(this, EventArgs.Empty);
        OnPlayerScoreUpdate?.Invoke(this, playerScore);
        OnEnemyScoreUpdate?.Invoke(this, playerScore);
    }

    public void GameOver(bool didPlayerWin)
    {
        isGameActive = false;
        OnGameOver?.Invoke(this, didPlayerWin);
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AddScore(int points)
    {
        playerScore += points;
        enemyScore += points;
    }

    public void PlayerCollectOrbPoints(int points)
    {
        playerScore += points;
        OnPlayerOrbCollected?.Invoke(this, EventArgs.Empty);
    }

    public void EnemyCollectOrbPoints(int points)
    {
        enemyScore += points;
        OnEnemyOrbCollected?.Invoke(this, EventArgs.Empty);
    }

    public void EmitParticles(Vector3 position)
    {
        particleEffect.EmitAtPosition(position);
    }
}