using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class FreeKickMinigameView : MonoBehaviour
{
    [Header("Curved Timing Bar")]
    [SerializeField] private RectTransform targetZone;
    [SerializeField] private RectTransform targetGlow;
    [SerializeField] private RectTransform movingMarker;
    [SerializeField] private Vector2 arcCenter = new Vector2(0f, -720f);
    [SerializeField, Min(1f)] private float arcRadius = 800f;
    [SerializeField] private float arcStartAngle = 30f;
    [SerializeField] private float arcEndAngle = -30f;

    [Header("Scoreboard")]
    [FormerlySerializedAs("roundText")]
    [SerializeField] private TextMeshProUGUI goalsText;
    [SerializeField] private TextMeshProUGUI streakText;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI resultDetailText;
    [SerializeField] private Image resultFlash;
    [SerializeField] private RectTransform ballIcon;
    [SerializeField] private string readyTitle = "TIME YOUR KICK";
    [SerializeField] private Color readyTitleColor = Color.white;
    [SerializeField] private Vector2 ballRestPosition = new Vector2(0f, -12f);
    [SerializeField, Min(0f)] private float ballKickHeight = 62f;
    [SerializeField, Min(0f)] private float ballScalePunch = 0.16f;
    [SerializeField, Range(0f, 1f)] private float resultFlashAlpha = 0.04f;
    [Tooltip("Fraction of the result pause used by the screen-flash fade.")]
    [SerializeField, Range(0.1f, 1f)] private float resultFlashFadePortion = 0.75f;

    [Header("Completion Popup")]
    [SerializeField] private GameObject completionPopup;
    [SerializeField] private TextMeshProUGUI completionTitleText;
    [SerializeField] private TextMeshProUGUI completionSummaryText;
    [SerializeField] private TextMeshProUGUI completionMessageText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private Button bonusButton;
    [SerializeField] private TextMeshProUGUI retryButtonText;
    [SerializeField] private TextMeshProUGUI finishButtonText;
    [SerializeField] private TextMeshProUGUI bonusButtonText;

    public TextMeshProUGUI GoalsText => goalsText;
    public TextMeshProUGUI StreakText => streakText;
    public GameObject CompletionPopup { get { EnsurePopupReferences(); return completionPopup; } }
    public TextMeshProUGUI CompletionTitleText { get { EnsurePopupReferences(); return completionTitleText; } }
    public TextMeshProUGUI CompletionSummaryText { get { EnsurePopupReferences(); return completionSummaryText; } }
    public TextMeshProUGUI CompletionMessageText { get { EnsurePopupReferences(); return completionMessageText; } }
    public Button RetryButton { get { EnsurePopupReferences(); return retryButton; } }
    public Button FinishButton { get { EnsurePopupReferences(); return finishButton; } }
    public Button BonusButton { get { EnsurePopupReferences(); return bonusButton; } }
    public TextMeshProUGUI RetryButtonText { get { EnsurePopupReferences(); return retryButtonText; } }
    public TextMeshProUGUI FinishButtonText { get { EnsurePopupReferences(); return finishButtonText; } }
    public TextMeshProUGUI BonusButtonText { get { EnsurePopupReferences(); return bonusButtonText; } }

    public void PrepareCompletionPopup()
    {
        EnsurePopupReferences();
        SetChildActive("Grade", false);
        SetChildActive("Praise", false);
        SetChildActive("Final Scoreboard", false);
        completionTitleText.gameObject.SetActive(true);
        completionSummaryText.gameObject.SetActive(true);
        completionMessageText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        finishButton.gameObject.SetActive(true);
        bonusButton.gameObject.SetActive(true);
        completionPopup.SetActive(false);
    }

    public void PrepareAttemptVisuals()
    {
        resultText.text = readyTitle;
        resultText.color = readyTitleColor;
        resultDetailText.text = string.Empty;
        resultFlash.color = new Color(1f, 1f, 1f, 0f);
        ballIcon.anchoredPosition = ballRestPosition;
        ballIcon.localScale = Vector3.one;
    }

    public void ShowKickResult(string title, string detail, Color color)
    {
        resultText.text = title;
        resultText.color = color;
        resultDetailText.text = detail;
        resultFlash.color = new Color(color.r, color.g, color.b, resultFlashAlpha);
    }

    public void AnimateKickResult(float normalizedProgress)
    {
        float kickArc = Mathf.Sin(Mathf.Clamp01(normalizedProgress) * Mathf.PI);
        ballIcon.anchoredPosition = ballRestPosition + Vector2.up * (kickArc * ballKickHeight);
        ballIcon.localScale = Vector3.one * (1f + kickArc * ballScalePunch);

        float flashProgress = Mathf.InverseLerp(
            0f,
            Mathf.Max(0.1f, resultFlashFadePortion),
            Mathf.Clamp01(normalizedProgress));
        Color flash = resultFlash.color;
        flash.a = Mathf.Lerp(resultFlashAlpha, 0f, flashProgress);
        resultFlash.color = flash;
    }

    public void SetMarkerPosition(float normalizedPosition)
    {
        PositionOnArc(movingMarker, normalizedPosition, arcRadius);
    }

    public void SetTargetWindow(float normalizedCenter, float normalizedWidth)
    {
        PositionOnArc(targetGlow, normalizedCenter, arcRadius);
        PositionOnArc(targetZone, normalizedCenter, arcRadius);

        float arcLength = Mathf.Deg2Rad * Mathf.Abs(arcEndAngle - arcStartAngle) * arcRadius;
        SetWindowSize(targetGlow, arcLength * normalizedWidth * 1.35f, 90f);
        SetWindowSize(targetZone, arcLength * normalizedWidth, 76f);
    }

    private void PositionOnArc(RectTransform element, float normalizedPosition, float radius)
    {
        if (element == null)
            return;

        float angle = Mathf.Lerp(arcStartAngle, arcEndAngle, Mathf.Clamp01(normalizedPosition));
        Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        element.anchoredPosition = arcCenter + direction * radius;
        element.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private static void SetWindowSize(RectTransform element, float width, float height)
    {
        if (element != null)
            element.sizeDelta = new Vector2(Mathf.Max(24f, width), height);
    }

    private void EnsurePopupReferences()
    {
        if (completionPopup == null)
        {
            Transform popup = FindDescendant(transform, "Free Kick Completion Popup");
            if (popup == null)
                popup = FindDescendant(transform, "Completion Backdrop");
            completionPopup = popup != null ? popup.gameObject : null;
        }

        if (completionPopup == null)
            return;

        completionTitleText ??= FindDescendant(completionPopup.transform, "Title")?.GetComponent<TextMeshProUGUI>();
        completionSummaryText ??= FindDescendant(completionPopup.transform, "Course Time")?.GetComponent<TextMeshProUGUI>();
        completionMessageText ??= FindDescendant(completionPopup.transform, "Bonus Message")?.GetComponent<TextMeshProUGUI>();
        retryButton ??= FindDescendant(completionPopup.transform, "Retry Button")?.GetComponent<Button>();
        finishButton ??= FindDescendant(completionPopup.transform, "Continue Button")?.GetComponent<Button>();
        bonusButton ??= FindDescendant(completionPopup.transform, "Optional Button")?.GetComponent<Button>();
        retryButtonText ??= retryButton != null ? retryButton.GetComponentInChildren<TextMeshProUGUI>(true) : null;
        finishButtonText ??= finishButton != null ? finishButton.GetComponentInChildren<TextMeshProUGUI>(true) : null;
        bonusButtonText ??= bonusButton != null ? bonusButton.GetComponentInChildren<TextMeshProUGUI>(true) : null;
    }

    private void SetChildActive(string childName, bool isActive)
    {
        Transform child = FindDescendant(completionPopup.transform, childName);
        if (child != null)
            child.gameObject.SetActive(isActive);
    }

    private static Transform FindDescendant(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
                return child;
        }
        return null;
    }
}
