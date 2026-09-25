using UnityEngine;
using UnityEngine.UI;

public interface IMinigameSkipTarget
{
    bool CanSkipToSummary { get; }
    void SkipToFinalSummary();
}

[DisallowMultipleComponent]
public sealed class MinigameHoldToSkip : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup visualCanvasGroup;
    [SerializeField] private Image radialFill;

    [Header("Timing")]
    [SerializeField, Min(0.1f)] private float holdDuration = 2f;
    [SerializeField, Min(0f)] private float fadeInDuration = 0.15f;
    [SerializeField, Min(0f)] private float fadeOutDelay = 0.5f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 0.45f;

    private IMinigameSkipTarget skipTarget;
    private float heldTime;
    private float timeSinceHoldReleased;
    private bool skipTriggered;

    private void Start()
    {
        FindSkipTarget();
        ResetProgress();
        HideImmediately();
    }

    private void Update()
    {
        if (skipTarget == null)
            FindSkipTarget();

        bool targetCanSkip = skipTarget != null && skipTarget.CanSkipToSummary;
        if (!targetCanSkip)
            skipTriggered = false;

        if (!targetCanSkip || skipTriggered)
        {
            ResetProgress();
            HideImmediately();
            return;
        }

        if (Input.GetKey(KeyCode.T))
        {
            timeSinceHoldReleased = 0f;
            FadeTowards(1f, fadeInDuration);

            heldTime += Time.unscaledDeltaTime;
            if (radialFill != null)
                radialFill.fillAmount = Mathf.Clamp01(heldTime / holdDuration);

            if (heldTime < holdDuration)
                return;

            skipTriggered = true;
            skipTarget.SkipToFinalSummary();
            HideImmediately();
            return;
        }

        ResetProgress();
        timeSinceHoldReleased += Time.unscaledDeltaTime;
        if (timeSinceHoldReleased >= fadeOutDelay)
            FadeTowards(0f, fadeOutDuration);
    }

    private void OnDisable()
    {
        ResetProgress();
        HideImmediately();
    }

    private void FindSkipTarget()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IMinigameSkipTarget target)
            {
                skipTarget = target;
                return;
            }
        }
    }

    private void FadeTowards(float targetAlpha, float duration)
    {
        if (visualCanvasGroup == null)
            return;

        if (duration <= Mathf.Epsilon)
        {
            visualCanvasGroup.alpha = targetAlpha;
            return;
        }

        visualCanvasGroup.alpha = Mathf.MoveTowards(
            visualCanvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime / duration);
    }

    private void HideImmediately()
    {
        timeSinceHoldReleased = 0f;
        if (visualCanvasGroup != null)
            visualCanvasGroup.alpha = 0f;
    }

    private void ResetProgress()
    {
        heldTime = 0f;
        if (radialFill != null)
            radialFill.fillAmount = 0f;
    }
}
