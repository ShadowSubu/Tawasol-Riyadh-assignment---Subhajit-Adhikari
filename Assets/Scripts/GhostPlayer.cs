using System;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    [Header("References")]
    private StateSync stateSync;

    [Header("Interpolation")]
    [SerializeField] private float interpolationSpeed = 10f;

    [Header("Ground")]
    [SerializeField] private float groundY = 0.5f;

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float lastProcessedTime = 0f;
    private Renderer ghostRenderer;

    private void Awake()
    {
        stateSync = FindAnyObjectByType<StateSync>();
    }

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
        ghostRenderer = GetComponent<Renderer>();
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
    }

    private void GameManager_OnGameStart(object sender, EventArgs e)
    {
        ResetGhost();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= GameManager_OnGameStart;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive)
            return;

        ProcessSyncedActions();
        InterpolatePosition();
    }

    private void ProcessSyncedActions()
    {
        // Get delayed action to simulate network lag
        PlayerAction delayedAction = stateSync.GetDelayedAction();

        if (delayedAction != null)
        {
            switch (delayedAction.actionType)
            {
                case ActionType.Position:
                    targetPosition = delayedAction.position;
                    break;

                case ActionType.Jump:
                    // Ghost mimics jump
                    targetPosition = delayedAction.position;
                    break;
            }
        }
    }

    private void InterpolatePosition()
    {
        // Smooth interpolation to prevent jittery movement
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(transform.position.x, targetPosition.y, targetPosition.z),
            interpolationSpeed * Time.deltaTime
        );

        // Ensure ghost doesn't go below ground
        Vector3 pos = transform.position;
        if (pos.y < groundY)
        {
            pos.y = groundY;
            transform.position = pos;
        }
    }

    public void ResetGhost()
    {
        transform.position = startPosition;
        targetPosition = startPosition;
        lastProcessedTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            OnHitObstacle();
        }
        else if (other.CompareTag("Collectible"))
        {
            OnCollectOrb(other.gameObject);
        }


        if (other.CompareTag("Collectible"))
        {
            GameManager.Instance.EnemyCollectOrbPoints(10);
            GameManager.Instance.EmitParticles(other.transform.position);
            other.gameObject.SetActive(false);
        }
        else if (true)
        {

        }
    }

    private void OnHitObstacle()
    {
        CameraShake.Instance?.Shake(0.3f, 0.5f);
        GameManager.Instance.GameOver(true);
    }

    private void OnCollectOrb(GameObject orb)
    {
        GameManager.Instance.EnemyCollectOrbPoints(10);
        GameManager.Instance.EmitParticles(orb.transform.position);
        orb.SetActive(false);
    }
}