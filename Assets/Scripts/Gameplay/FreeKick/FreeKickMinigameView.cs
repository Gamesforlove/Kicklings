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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI resultDetailText;
    [SerializeField] private Image resultFlash;
    [SerializeField] private RectTransform ballIcon;

    public TextMeshProUGUI GoalsText => goalsText;
    public TextMeshProUGUI ScoreText => scoreText;
    public TextMeshProUGUI StreakText => streakText;
    public TextMeshProUGUI SpeedText => speedText;
    public TextMeshProUGUI ResultText => resultText;
    public TextMeshProUGUI ResultDetailText => resultDetailText;
    public Image ResultFlash => resultFlash;
    public RectTransform BallIcon => ballIcon;

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
}
