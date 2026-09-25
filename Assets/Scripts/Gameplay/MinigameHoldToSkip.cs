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
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private Image radialFill;
    [SerializeField, Min(0.1f)] private float holdDuration = 2f;

    private IMinigameSkipTarget skipTarget;
    private float heldTime;
    private bool skipTriggered;

    private void Start()
    {
        FindSkipTarget();
        ResetProgress();
        RefreshVisibility();
    }

    private void Update()
    {
        if (skipTarget == null)
            FindSkipTarget();

        bool targetCanSkip = skipTarget != null && skipTarget.CanSkipToSummary;
        if (!targetCanSkip)
            skipTriggered = false;

        bool canSkip = targetCanSkip && !skipTriggered;
        if (visualRoot != null && visualRoot.activeSelf != canSkip)
            visualRoot.SetActive(canSkip);

        if (!canSkip)
        {
            ResetProgress();
            return;
        }

        if (!Input.GetKey(KeyCode.T))
        {
            ResetProgress();
            return;
        }

        heldTime += Time.unscaledDeltaTime;
        if (radialFill != null)
            radialFill.fillAmount = Mathf.Clamp01(heldTime / holdDuration);

        if (heldTime < holdDuration)
            return;

        skipTriggered = true;
        skipTarget.SkipToFinalSummary();
        RefreshVisibility();
    }

    private void OnDisable()
    {
        ResetProgress();
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

    private void RefreshVisibility()
    {
        bool targetCanSkip = skipTarget != null && skipTarget.CanSkipToSummary;
        if (!targetCanSkip)
            skipTriggered = false;

        bool visible = targetCanSkip && !skipTriggered;
        if (visualRoot != null)
            visualRoot.SetActive(visible);
    }

    private void ResetProgress()
    {
        heldTime = 0f;
        if (radialFill != null)
            radialFill.fillAmount = 0f;
    }
}
