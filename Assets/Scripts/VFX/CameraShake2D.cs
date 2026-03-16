using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    public static CameraShake2D Instance { get; private set; }

    [Header("Defaults")]
    [SerializeField] private float defaultAmplitude = 0.2f;
    [SerializeField] private float defaultFrequency = 25f;
    [SerializeField] private float defaultDuration = 0.35f;

    private float shakeTimeLeft;
    private float currentAmplitude;
    private float currentFrequency;

    private Vector3 currentOffset;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

    }

    private void LateUpdate()
    {
        if(currentOffset != Vector3.zero)
        {
            transform.localPosition -= currentOffset;
            currentOffset = Vector3.zero;
        }

        if (shakeTimeLeft <= 0f)
            return;

        shakeTimeLeft -= Time.unscaledDeltaTime;

        float t = Time.unscaledDeltaTime * currentFrequency;

        float offsetX = (Mathf.PerlinNoise(t, 0.123f) - 0.5f) * 2f;
        float offsetY = (Mathf.PerlinNoise(0.456f, t) - 0.5f) * 2f;

        currentOffset = new Vector3(offsetX, offsetY, 0f) * currentAmplitude;
        transform.localPosition += currentOffset;

    }

    public void TriggerShake()
    {
        TriggerShake(defaultAmplitude, defaultFrequency, defaultDuration);
    }

    public void TriggerShake(float amplitude, float frequency, float duration)
    {
        currentAmplitude = amplitude;
        currentFrequency = frequency;
        shakeTimeLeft = duration;
    }
}
