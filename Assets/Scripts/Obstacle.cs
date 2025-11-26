using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float despawnX = -15f;

    private Renderer obstacleRenderer;

    [Header("Dissolve")]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveDuration = 1.5f;
    [SerializeField] private bool destroyAfterDissolve = true;
    [SerializeField] private float destroyDelay = 0.2f;

    private Material instanceMaterial;
    private Material originalMaterial;
    private bool isDissolving = false;
    private float dissolveProgress = 0f;

    private void Awake()
    {
        obstacleRenderer = GetComponent<Renderer>();
        if (obstacleRenderer != null)
        {
            originalMaterial = obstacleRenderer.material;
            if (dissolveMaterial != null)
            {
                instanceMaterial = new Material(dissolveMaterial);
                instanceMaterial.SetFloat("_DissolveAmount", 0);
            }
            else
            {
                instanceMaterial = obstacleRenderer.material;
            }
        }
    }

    private void OnEnable()
    {
        ResetObstacle();
    }

    private void Update()
    {
        if (isDissolving && instanceMaterial != null)
        {
            // Increment dissolve progress
            dissolveProgress += Time.deltaTime / dissolveDuration;
            dissolveProgress = Mathf.Clamp01(dissolveProgress);

            // Update shader property
            instanceMaterial.SetFloat("_DissolveAmount", dissolveProgress);

            // Destroy object when fully dissolved
            if (dissolveProgress >= 1f && destroyAfterDissolve)
            {
                isDissolving = false;
            }
        }

        if (!GameManager.Instance.IsGameActive)
            return;

        // Move obstacle backward (relative to player moving forward)
        transform.position += Vector3.back * GameManager.Instance.GameSpeed * Time.deltaTime;

        // Deactivate when off screen
        if (transform.position.x < despawnX)
        {
            gameObject.SetActive(false);
            ResetObstacle();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            StartDissolve();
        }
    }

    private void StartDissolve()
    {
        if (isDissolving || instanceMaterial == null) return;

        if (obstacleRenderer != null)
        {
            obstacleRenderer.material = instanceMaterial;
        }

        isDissolving = true;
        dissolveProgress = 0f;
    }

    private void ResetObstacle()
    {
        isDissolving = false;
        if (dissolveMaterial != null && dissolveMaterial.HasProperty("_DissolveAmount"))
        {
            dissolveMaterial.SetFloat("_DissolveAmount", 0f);
        }
        if (obstacleRenderer != null && originalMaterial != null)
        {
            obstacleRenderer.material = originalMaterial;
        }
    }

    private void OnDisable()
    {
        ResetObstacle();
    }

    private void OnDestroy()
    {
        // Clean up material instance
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}