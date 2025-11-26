using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI enemyScoreText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button returnToMenuButton;

    private void Start()
    {
        ShowMainMenu();
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        GameManager.Instance.OnPlayerScoreUpdate += GameManager_OnPlayerScoreUpdate;
        GameManager.Instance.OnEnemyScoreUpdate += GameManager_OnEnemyScoreUpdate;
        GameManager.Instance.OnPlayerOrbCollected += GameManager_OnPlayerOrbCollected;
        GameManager.Instance.OnEnemyOrbCollected += GameManager_OnEnemyOrbCollected;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
        GameManager.Instance.OnPlayerScoreUpdate -= GameManager_OnPlayerScoreUpdate;
        GameManager.Instance.OnEnemyScoreUpdate -= GameManager_OnEnemyScoreUpdate;
        GameManager.Instance.OnPlayerOrbCollected -= GameManager_OnPlayerOrbCollected;
        GameManager.Instance.OnEnemyOrbCollected -= GameManager_OnEnemyOrbCollected;
    }

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(StartGame);
        exitGameButton.onClick.AddListener(ExitGame);
        restartGameButton.onClick.AddListener(RestartGame);
        returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.RemoveAllListeners();
        restartGameButton.onClick.RemoveAllListeners();
        returnToMenuButton.onClick.RemoveAllListeners();
    }

    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        gameUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
        mainMenuUI.SetActive(false);
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
    }

    private void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }

    private void RestartGame()
    {
        GameManager.Instance.RestartGame();
        StartGame();
    }

    private void ReturnToMainMenu()
    {
        ShowMainMenu();
    }

    private void GameManager_OnPlayerScoreUpdate(object sender, int score)
    {
        if (playerScoreText != null)
        {
            playerScoreText.text = "Score: " + score;
        }
    }
    private void GameManager_OnEnemyScoreUpdate(object sender, int score)
    {
        if (enemyScoreText != null)
        {
            enemyScoreText.text = "Score: " + score;
        }
    }

    private void GameManager_OnGameOver(object sender, bool didPlayerWin)
    {
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
        if (didPlayerWin)
        {
            gameOverText.text = "You Win!!";
        }
        else
        {
            gameOverText.text = "You Lose";
        }
        finalScoreText.text = "Score : " + GameManager.Instance.PlayerScore.ToString();
    }
    private void GameManager_OnPlayerOrbCollected(object sender, EventArgs e)
    {
        playerScoreText.GetComponent<RectTransform>().DOScale(1.2f, 0.2f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            playerScoreText.GetComponent<RectTransform>().DOScale(1f, 0.2f).SetEase(Ease.InOutBounce);
        });
    }
    private void GameManager_OnEnemyOrbCollected(object sender, EventArgs e)
    {
        enemyScoreText.GetComponent<RectTransform>().DOScale(1.2f, 0.2f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            enemyScoreText.GetComponent<RectTransform>().DOScale(1f, 0.2f).SetEase(Ease.InOutBounce);
        });
    }
}
