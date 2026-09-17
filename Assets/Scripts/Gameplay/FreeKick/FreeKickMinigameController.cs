using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class FreeKickMinigameController : MonoBehaviour
{
    private static readonly Color GoalColor = new Color32(83, 232, 130, 255);
    private static readonly Color NearMissColor = new Color32(255, 210, 58, 255);
    private static readonly Color MissColor = new Color32(255, 105, 105, 255);

    [Header("Interface")]
    [SerializeField] private FreeKickMinigameView interfaceView;

    [Header("Difficulty")]
    [Tooltip("Marker speeds for goals one onward. The final entry repeats if more goals are required.")]
    [SerializeField] private float[] goalSpeeds = { 0.58f, 0.76f, 0.98f, 1.24f, 1.42f, 1.58f, 1.68f };
    [Tooltip("Normalized green-window widths for goals one onward. The final entry repeats if more goals are required.")]
    [SerializeField] private float[] goalTargetWidths = { 0.1f, 0.088f, 0.076f, 0.064f, 0.058f, 0.052f, 0.048f };
    [SerializeField, Range(0f, 0.3f)] private float targetEdgePadding = 0.11f;

    [Header("Goal Flow")]
    [SerializeField, Min(1)] private int requiredGoals = 5;
    [SerializeField, Min(1)] private int bonusGoalCount = 2;
    [SerializeField, Min(0.1f)] private float resultHoldDuration = 1.05f;
    [SerializeField, Min(1)] private int baseGoalScore = 1000;
    [SerializeField, Min(0)] private int streakBonus = 150;
    [Tooltip("Invoked after the required goal count's final feedback finishes.")]
    [SerializeField] private UnityEvent onMinigameCompleted;

    private int goalsMade;
    private int attemptCount;
    private int score;
    private int streak;
    private int bestStreak;
    private float markerPosition;
    private float markerDirection;
    private float targetCenter;
    private float targetWidth;
    private float currentSpeed;
    private float resultTimer;
    private bool isShowingResult;
    private bool isAwaitingChoice;
    private bool isBonusRound;
    private bool isComplete;

    public int GoalsMade => goalsMade;
    public int AttemptCount => attemptCount;
    public int Score => score;
    public int Streak => streak;
    public int BestStreak => bestStreak;
    public float MarkerPosition => markerPosition;
    public float TargetCenter => targetCenter;
    public float TargetWidth => targetWidth;
    public float CurrentSpeed => currentSpeed;

    private void Awake()
    {
        if (interfaceView == null)
            interfaceView = FindFirstObjectByType<FreeKickMinigameView>(FindObjectsInactive.Include);

        if (interfaceView == null)
        {
            Debug.LogError("Free Kick UI prefab is missing from the scene.", this);
            enabled = false;
            return;
        }

        interfaceView.PrepareCompletionPopup();
        interfaceView.FinishButton.onClick.AddListener(HandleFinishButton);
        interfaceView.BonusButton.onClick.AddListener(HandleBonusButton);
        interfaceView.CompletionPopup.SetActive(false);
        BeginNextAttempt();
    }

    private void OnDestroy()
    {
        if (interfaceView == null)
            return;

        interfaceView.FinishButton?.onClick.RemoveListener(HandleFinishButton);
        interfaceView.BonusButton?.onClick.RemoveListener(HandleBonusButton);
    }

    private void Update()
    {
        if (isComplete || isAwaitingChoice)
            return;

        if (isShowingResult)
        {
            AnimateResult();
            resultTimer -= Time.unscaledDeltaTime;
            if (resultTimer <= 0f)
            {
                if (!isBonusRound && goalsMade >= requiredGoals)
                    ShowRequiredCompletionPopup();
                else if (isBonusRound && goalsMade >= requiredGoals + bonusGoalCount)
                    ShowBonusCompletionPopup();
                else
                    BeginNextAttempt();
            }
            return;
        }

        AdvanceMarker(Time.unscaledDeltaTime);
        if (WasKickPressed())
            ResolveKick();
    }

    private void AdvanceMarker(float deltaTime)
    {
        markerPosition += markerDirection * currentSpeed * deltaTime;

        while (markerPosition < 0f || markerPosition > 1f)
        {
            if (markerPosition > 1f)
            {
                markerPosition = 2f - markerPosition;
                markerDirection = -1f;
            }
            else
            {
                markerPosition = -markerPosition;
                markerDirection = 1f;
            }
        }

        interfaceView.SetMarkerPosition(markerPosition);
    }

    private void BeginNextAttempt()
    {
        isShowingResult = false;
        resultTimer = 0f;

        currentSpeed = GetDifficultyValue(goalSpeeds, goalsMade, 0.58f);
        targetWidth = GetDifficultyValue(goalTargetWidths, goalsMade, 0.1f);
        targetCenter = ChooseTargetCenter(targetWidth);

        bool beginsOnRight = attemptCount % 2 == 1;
        markerPosition = beginsOnRight ? 1f : 0f;
        markerDirection = beginsOnRight ? -1f : 1f;

        interfaceView.SetTargetWindow(targetCenter, targetWidth);
        interfaceView.SetMarkerPosition(markerPosition);

        interfaceView.ResultText.text = "TIME YOUR KICK";
        interfaceView.ResultText.color = Color.white;
        interfaceView.ResultDetailText.text = string.Empty;
        interfaceView.ResultFlash.color = new Color(1f, 1f, 1f, 0f);
        interfaceView.BallIcon.anchoredPosition = new Vector2(0f, -12f);
        interfaceView.BallIcon.localScale = Vector3.one;
        RefreshScoreboard();
    }

    private float ChooseTargetCenter(float width)
    {
        float halfWidth = width * 0.5f;
        float minimum = targetEdgePadding + halfWidth;
        float maximum = 1f - targetEdgePadding - halfWidth;
        float candidate = Random.Range(minimum, maximum);

        // Keep consecutive targets visually distinct without making either side predictable.
        if (attemptCount > 0 && Mathf.Abs(candidate - targetCenter) < 0.16f)
        {
            candidate += candidate < 0.5f ? 0.22f : -0.22f;
            candidate = Mathf.Clamp(candidate, minimum, maximum);
        }

        return candidate;
    }

    private void ResolveKick()
    {
        attemptCount++;
        isShowingResult = true;
        resultTimer = resultHoldDuration;

        float distance = Mathf.Abs(markerPosition - targetCenter);
        float halfWidth = targetWidth * 0.5f;
        bool isGoal = distance <= halfWidth;
        bool isNearMiss = !isGoal && distance <= halfWidth * 2.2f;

        if (isGoal)
        {
            goalsMade++;
            streak++;
            bestStreak = Mathf.Max(bestStreak, streak);
            int speedBonus = Mathf.RoundToInt(currentSpeed * 200f);
            int points = baseGoalScore + speedBonus + Mathf.Max(0, streak - 1) * streakBonus;
            score += points;
            ShowResult("GOAL!", $"+{points:N0}  •  {GetAccuracyPercent(distance, halfWidth)}% accuracy", GoalColor);
        }
        else
        {
            streak = 0;
            ShowResult(
                isNearMiss ? "SO CLOSE!" : "MISSED!",
                isNearMiss ? "Just outside the stripe — try again" : "Same difficulty — try again",
                isNearMiss ? NearMissColor : MissColor);
        }

        RefreshScoreboard();
    }

    private void ShowResult(string title, string detail, Color color)
    {
        interfaceView.ResultText.text = title;
        interfaceView.ResultText.color = color;
        interfaceView.ResultDetailText.text = detail;
        interfaceView.ResultFlash.color = new Color(color.r, color.g, color.b, 0.2f);
    }

    private void AnimateResult()
    {
        float progress = 1f - Mathf.Clamp01(resultTimer / resultHoldDuration);
        float kickArc = Mathf.Sin(progress * Mathf.PI);
        interfaceView.BallIcon.anchoredPosition = new Vector2(0f, -12f + kickArc * 62f);
        interfaceView.BallIcon.localScale = Vector3.one * (1f + kickArc * 0.16f);

        Color flash = interfaceView.ResultFlash.color;
        flash.a = Mathf.Lerp(0.2f, 0f, progress);
        interfaceView.ResultFlash.color = flash;
    }

    private void RefreshScoreboard()
    {
        interfaceView.GoalsText.text = goalsMade.ToString();
        interfaceView.ScoreText.text = score.ToString("N0");
        interfaceView.StreakText.text = $"{streak}  <color=#7D91A8>BEST {bestStreak}</color>";
    }

    private static bool WasKickPressed()
    {
        return Input.GetKeyDown(KeyCode.Z) ||
               Input.GetKeyDown(KeyCode.Space) ||
               Input.GetMouseButtonDown(0);
    }

    private void ShowRequiredCompletionPopup()
    {
        isShowingResult = false;
        isAwaitingChoice = true;
        interfaceView.CompletionPopup.SetActive(true);
        interfaceView.CompletionTitleText.text = "FREE KICK COMPLETE!";
        interfaceView.CompletionSummaryText.text = FormatCompletionSummary();
        interfaceView.CompletionMessageText.text = "Play two bonus goals for more points?";
        interfaceView.FinishButtonText.text = "Finish Minigame";
        interfaceView.BonusButtonText.text = "Play Bonus Round";
        interfaceView.BonusButton.gameObject.SetActive(true);
        SetPopupButtonPositions(true);
    }

    private void ShowBonusCompletionPopup()
    {
        isShowingResult = false;
        isAwaitingChoice = true;
        interfaceView.CompletionPopup.SetActive(true);
        interfaceView.CompletionTitleText.text = "BONUS COMPLETE!";
        interfaceView.CompletionSummaryText.text = FormatCompletionSummary();
        interfaceView.CompletionMessageText.text = "Two extra goals scored!";
        interfaceView.FinishButtonText.text = "Continue";
        interfaceView.BonusButton.gameObject.SetActive(false);
        SetPopupButtonPositions(false);
    }

    private void HandleBonusButton()
    {
        if (!isAwaitingChoice || isBonusRound || isComplete)
            return;

        isBonusRound = true;
        isAwaitingChoice = false;
        interfaceView.CompletionPopup.SetActive(false);
        BeginNextAttempt();
    }

    private void HandleFinishButton()
    {
        if (!isAwaitingChoice || isComplete)
            return;

        isAwaitingChoice = false;
        isComplete = true;
        interfaceView.FinishButton.interactable = false;
        interfaceView.BonusButton.interactable = false;
        onMinigameCompleted?.Invoke();
    }

    private string FormatCompletionSummary()
    {
        string attemptLabel = attemptCount == 1 ? "ATTEMPT" : "ATTEMPTS";
        return $"{goalsMade} GOALS  •  {attemptCount} {attemptLabel}";
    }

    private void SetPopupButtonPositions(bool showBonusButton)
    {
        RectTransform finishRect = interfaceView.FinishButtonRect;
        finishRect.anchoredPosition = new Vector2(showBonusButton ? -210f : 0f, -350f);
        RectTransform bonusRect = interfaceView.BonusButtonRect;
        bonusRect.anchoredPosition = new Vector2(210f, -350f);
    }

    private static int GetAccuracyPercent(float distance, float halfWidth)
    {
        if (halfWidth <= Mathf.Epsilon)
            return 100;

        return Mathf.RoundToInt(Mathf.Lerp(100f, 80f, Mathf.Clamp01(distance / halfWidth)));
    }

    private static float GetDifficultyValue(float[] values, int completedGoals, float fallback)
    {
        if (values == null || values.Length == 0)
            return fallback;

        return values[Mathf.Clamp(completedGoals, 0, values.Length - 1)];
    }

}
