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
    public TextMeshProUGUI ResultText => resultText;
    public TextMeshProUGUI ResultDetailText => resultDetailText;
    public Image ResultFlash => resultFlash;
    public RectTransform BallIcon => ballIcon;
    public GameObject CompletionPopup { get { EnsurePopupReferences(); return completionPopup; } }
    public TextMeshProUGUI CompletionTitleText { get { EnsurePopupReferences(); return completionTitleText; } }
    public TextMeshProUGUI CompletionSummaryText { get { EnsurePopupReferences(); return completionSummaryText; } }
    public TextMeshProUGUI CompletionMessageText { get { EnsurePopupReferences(); return completionMessageText; } }
    public Button RetryButton { get { EnsurePopupReferences(); return retryButton; } }
    public Button FinishButton { get { EnsurePopupReferences(); return finishButton; } }
    public Button BonusButton { get { EnsurePopupReferences(); return bonusButton; } }
    public RectTransform RetryButtonRect { get { EnsurePopupReferences(); return retryButton.GetComponent<RectTransform>(); } }
    public RectTransform FinishButtonRect { get { EnsurePopupReferences(); return finishButton.GetComponent<RectTransform>(); } }
    public RectTransform BonusButtonRect { get { EnsurePopupReferences(); return bonusButton.GetComponent<RectTransform>(); } }
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
