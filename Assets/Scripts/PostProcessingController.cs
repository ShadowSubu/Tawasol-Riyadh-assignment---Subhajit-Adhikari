using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [Header("Post Processing")]
    public Volume postProcessVolume;

    private MotionBlur motionBlur;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    private float targetMotionBlur = 1f;
    private float currentMotionBlur = 0f;

    private void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out motionBlur);
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out lensDistortion);
        }

        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            UpdateMotionBlur();
        }
    }

    private void UpdateMotionBlur()
    {
        // Increase motion blur as speed increases
        float speedPercent = Mathf.InverseLerp(5f, 20f, GameManager.Instance.GameSpeed);
        targetMotionBlur = Mathf.Lerp(0f, 100f, speedPercent);

        currentMotionBlur = Mathf.Lerp(currentMotionBlur, targetMotionBlur, Time.deltaTime * 2f);

        if (motionBlur != null)
        {
            motionBlur.intensity.value = currentMotionBlur;
        }
    }

    public void TriggerCrashEffect()
    {
        StartCoroutine(CrashEffectCoroutine());
    }
    private void GameManager_OnGameOver(object sender, bool e)
    {
        TriggerCrashEffect();
    }

    private System.Collections.IEnumerator CrashEffectCoroutine()
    {
        // Chromatic aberration
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 1f;
        }

        // Lens distortion
        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = -50f;
        }

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(1f, 0f, t);
            }

            if (lensDistortion != null)
            {
                lensDistortion.intensity.value = Mathf.Lerp(-50f, 0f, t);
            }

            yield return null;
        }

        // Reset
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 0f;
        }

        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = 0f;
        }
    }
}