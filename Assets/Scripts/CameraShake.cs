using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;

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
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            currentShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currentShakeDuration = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 10f);
        }
    }

    public void Shake(float duration, float magnitude)
    {
        currentShakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    public void Shake()
    {
        Shake(shakeDuration, shakeMagnitude);
    }
}