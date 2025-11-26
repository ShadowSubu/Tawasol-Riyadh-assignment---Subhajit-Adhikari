using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -30f;
    private float verticalVelocity = 0f;
    private bool isGrounded = true;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Boundaries")]
    [SerializeField] private float groundY = 0.5f;

    [Header("State Sync")]
    private StateSync stateSync;

    private Vector3 startPosition;
    private Renderer playerRenderer;

    private void Awake()
    {
        stateSync = FindAnyObjectByType<StateSync>();
    }

    private void Start()
    {
        startPosition = transform.position;
        playerRenderer = GetComponent<Renderer>();
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
    }

    private void GameManager_OnGameStart(object sender, EventArgs e)
    {
        ResetPlayer();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= GameManager_OnGameStart;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive)
            return;

        HandleInput();
        ApplyGravity();
        MovePlayer();
        CheckGround();

        // Send state to sync system
        SendStateUpdate();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isGrounded && GameManager.Instance.IsGameActive)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        verticalVelocity = jumpForce;
        isGrounded = false;

        // Record jump action
        stateSync.RecordAction(new PlayerAction
        {
            timestamp = Time.time,
            actionType = ActionType.Jump,
            position = transform.position,
            velocity = verticalVelocity
        });
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity = 0f;
        }
    }

    private void MovePlayer()
    {
        Vector3 pos = transform.position;
        pos.y += verticalVelocity * Time.deltaTime;

        // Clamp to ground
        if (pos.y < groundY)
        {
            pos.y = groundY;
            isGrounded = true;
            verticalVelocity = 0f;
        }

        transform.position = pos;
    }

    private void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = transform.position.y <= groundY + 0.1f;
        }
    }

    private void SendStateUpdate()
    {
        // Continuously send position updates
        stateSync.RecordAction(new PlayerAction
        {
            timestamp = Time.time,
            actionType = ActionType.Position,
            position = transform.position,
            velocity = verticalVelocity
        });
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
    }

    private void OnHitObstacle()
    {
        CameraShake.Instance?.Shake(0.3f, 0.5f);
        GameManager.Instance.GameOver(false);
    }

    private void OnCollectOrb(GameObject orb)
    {
        GameManager.Instance.PlayerCollectOrbPoints(10);
        GameManager.Instance.EmitParticles(orb.transform.position);

        orb.SetActive(false);
    }

    public void ResetPlayer()
    {
        transform.position = startPosition;
        verticalVelocity = 0f;
        isGrounded = true;
        stateSync.ClearActions();
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}