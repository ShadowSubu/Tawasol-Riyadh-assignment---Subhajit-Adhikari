using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float despawnX = -15f;

    [Header("Bob Effect")]
    [SerializeField] private float bobSpeed = 2f;
    public float bobAmount = 0.3f;
    private float bobTimer = 0f;
    private Vector3 startPosition;

    private void OnEnable()
    {
        startPosition = transform.position;
        bobTimer = 0f;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive)
            return;

        // Move collectible backward
        transform.position += Vector3.back * GameManager.Instance.GameSpeed * Time.deltaTime;

        // Bob up and down
        bobTimer += Time.deltaTime * bobSpeed;
        Vector3 pos = transform.position;
        pos.y = startPosition.y + Mathf.Sin(bobTimer) * bobAmount;
        transform.position = pos;

        // Deactivate when off screen
        if (transform.position.x < despawnX)
        {
            gameObject.SetActive(false);
        }
    }
}