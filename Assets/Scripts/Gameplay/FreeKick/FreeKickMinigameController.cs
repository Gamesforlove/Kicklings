using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class FreeKickMinigameController : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private FreeKickMinigameView interfaceView;

    [Header("Feedback Colors")]
    [SerializeField] private Color goalColor = new Color32(83, 232, 130, 255);
    [SerializeField] private Color nearMissColor = new Color32(255, 210, 58, 255);
    [SerializeField] private Color missColor = new Color32(255, 105, 105, 255);
    [SerializeField] private Color streakBestColor = new Color32(125, 145, 168, 255);
    [SerializeField] private Color gradeLabelColor = new Color32(255, 210, 58, 255);
    [SerializeField] private Color aGradeColor = new Color32(99, 230, 129, 255);
    [SerializeField] private Color bGradeColor = new Color32(85, 221, 242, 255);
    [SerializeField] private Color cGradeColor = new Color32(255, 118, 95, 255);

    [Header("Difficulty")]
    [Tooltip("Marker speeds for goals one onward. The final entry repeats if more goals are required.")]
    [SerializeField] private float[] goalSpeeds = { 0.49f, 0.76f, 1.11f, 1.42f, 1.59f };
    [Tooltip("Normalized green-window widths for goals one onward. The final entry repeats if more goals are required.")]
    [SerializeField] private float[] goalTargetWidths = { 0.106f, 0.088f, 0.07f, 0.058f, 0.058f };
    [SerializeField, Range(0f, 0.3f)] private float targetEdgePadding = 0.11f;
    [SerializeField, Range(0f, 1f)] private float minimumTargetShift = 0.16f;
    [SerializeField, Range(0f, 1f)] private float targetShiftAmount = 0.22f;

    [Header("Accuracy and Near Misses")]
    [SerializeField, Range(1f, 5f)] private float nearMissDistanceMultiplier = 2.2f;
    [SerializeField, Range(0.1f, 1f)] private float perfectWindowFraction = 0.5f;
    [SerializeField, Range(0, 100)] private int goalEdgeAccuracy = 95;

    [Header("Goal Flow")]
    [SerializeField, Min(1)] private int requiredGoals = 4;
    [SerializeField, Min(1)] private int bonusGoalCount = 1;
    [SerializeField, Min(1)] private int requiredAGradeMaximumAttempts = 6;
    [SerializeField, Min(1)] private int requiredBGradeMaximumAttempts = 12;
    [SerializeField, Min(1)] private int bonusAGradeMaximumAttempts = 4;
    [SerializeField, Min(1)] private int bonusBGradeMaximumAttempts = 6;
    [SerializeField, Min(0.1f)] private float resultHoldDuration = 1.05f;
    [Tooltip("Invoked when the player chooses Finish from either performance summary.")]
    [SerializeField] private UnityEvent onMinigameCompleted;

    private int goalsMade;
    private int requiredAttemptCount;
    private int bonusAttemptCount;
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
    private bool hasRequiredResult;
    private bool hasBonusResult;
    private PerformanceGrade bestRequiredGrade;
    private PerformanceGrade bestBonusGrade;
    private int bestRequiredAttemptCount;
    private int bestBonusAttemptCount;

    public int GoalsMade => goalsMade;
    public int RequiredAttemptCount => requiredAttemptCount;
    public int BonusAttemptCount => bonusAttemptCount;
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
        interfaceView.RetryButton.onClick.AddListener(HandleRetryButton);
        interfaceView.FinishButton.onClick.AddListener(HandleFinishButton);
        interfaceView.BonusButton.onClick.AddListener(HandleBonusButton);
        interfaceView.CompletionPopup.SetActive(false);
        BeginNextAttempt();
    }

    private void OnDestroy()
    {
        if (interfaceView == null)
            return;

        interfaceView.RetryButton?.onClick.RemoveListener(HandleRetryButton);
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

        bool beginsOnRight = GetTotalAttemptCount() % 2 == 1;
        markerPosition = beginsOnRight ? 1f : 0f;
        markerDirection = beginsOnRight ? -1f : 1f;

        interfaceView.SetTargetWindow(targetCenter, targetWidth);
        interfaceView.SetMarkerPosition(markerPosition);

        interfaceView.PrepareAttemptVisuals();
        RefreshScoreboard();
    }

    private float ChooseTargetCenter(float width)
    {
        float halfWidth = width * 0.5f;
        float minimum = targetEdgePadding + halfWidth;
        float maximum = 1f - targetEdgePadding - halfWidth;
        float candidate = Random.Range(minimum, maximum);

        // Keep consecutive targets visually distinct without making either side predictable.
        if (GetTotalAttemptCount() > 0 && Mathf.Abs(candidate - targetCenter) < minimumTargetShift)
        {
            candidate += candidate < 0.5f ? targetShiftAmount : -targetShiftAmount;
            candidate = Mathf.Clamp(candidate, minimum, maximum);
        }

        return candidate;
    }

    private void ResolveKick()
    {
        if (isBonusRound)
            bonusAttemptCount++;
        else
            requiredAttemptCount++;
        isShowingResult = true;
        resultTimer = resultHoldDuration;

        float distance = Mathf.Abs(markerPosition - targetCenter);
        float halfWidth = targetWidth * 0.5f;
        bool isGoal = distance <= halfWidth;
        bool isNearMiss = !isGoal && distance <= halfWidth * nearMissDistanceMultiplier;
        int accuracyPercent = GetAccuracyPercent(distance, halfWidth, targetCenter);

        if (isGoal)
        {
            goalsMade++;
            streak++;
            bestStreak = Mathf.Max(bestStreak, streak);
            interfaceView.ShowKickResult("GOAL!", $"{accuracyPercent}% accuracy", goalColor);
        }
        else
        {
            streak = 0;
            interfaceView.ShowKickResult(
                isNearMiss ? "SO CLOSE!" : "MISSED!",
                isNearMiss
                    ? $"{accuracyPercent}% accuracy — just outside the stripe"
                    : $"{accuracyPercent}% accuracy — try again",
                isNearMiss ? nearMissColor : missColor);
        }

        RefreshScoreboard();
    }

    private void AnimateResult()
    {
        float progress = 1f - Mathf.Clamp01(resultTimer / resultHoldDuration);
        interfaceView.AnimateKickResult(progress);
    }

    private void RefreshScoreboard()
    {
        interfaceView.GoalsText.text = goalsMade.ToString();
        interfaceView.StreakText.text =
            $"{streak}  <color=#{ColorUtility.ToHtmlStringRGB(streakBestColor)}>BEST {bestStreak}</color>";
    }

    private static bool WasKickPressed()
    {
        return Input.GetKeyDown(KeyCode.Z) ||
               Input.GetKeyDown(KeyCode.Space);
    }

    private void ShowRequiredCompletionPopup()
    {
        RecordRequiredPerformance();
        isShowingResult = false;
        isAwaitingChoice = true;
        interfaceView.CompletionPopup.SetActive(true);
        interfaceView.CompletionTitleText.text = "FREE KICK COMPLETE!";
        interfaceView.CompletionSummaryText.text =
            $"<size=130>{FormatGrade(bestRequiredGrade)}</size>\n" +
            $"<size=32><color=#{ColorUtility.ToHtmlStringRGB(gradeLabelColor)}>BEST</color></size>";
        interfaceView.CompletionMessageText.text =
            $"Good job!\n\n<size=22>4 GOALS  •  {bestRequiredAttemptCount} ATTEMPTS</size>";
        interfaceView.RetryButtonText.text = "Retry Goals";
        interfaceView.FinishButtonText.text = "Finish Minigame";
        interfaceView.BonusButtonText.text = "Play Bonus Round";
        interfaceView.RetryButton.gameObject.SetActive(true);
        interfaceView.BonusButton.gameObject.SetActive(true);
    }

    private void ShowBonusCompletionPopup()
    {
        RecordBonusPerformance();
        isShowingResult = false;
        isAwaitingChoice = true;
        interfaceView.CompletionPopup.SetActive(true);
        interfaceView.CompletionTitleText.text = "BONUS COMPLETE!";
        interfaceView.CompletionSummaryText.text =
            "<align=left>" +
            $"<size=120><pos=20%>{FormatGrade(bestRequiredGrade)}</pos>" +
            $"<pos=70%>{FormatGrade(bestBonusGrade)}</pos></size>\n" +
            $"<size=30><color=#{ColorUtility.ToHtmlStringRGB(gradeLabelColor)}><pos=20%>BEST</pos>" +
            "<pos=68%>BONUS</pos></color></size>";
        string bonusAttemptLabel = bestBonusAttemptCount == 1 ? "ATTEMPT" : "ATTEMPTS";
        interfaceView.CompletionMessageText.text =
            "Good job!\n\n" +
            $"<size=22>1 BONUS  •  {bestBonusAttemptCount} {bonusAttemptLabel}</size>";
        interfaceView.RetryButtonText.text = "Retry Goals";
        interfaceView.FinishButtonText.text = "Finish Minigame";
        interfaceView.BonusButtonText.text = "Retry Bonus";
        interfaceView.RetryButton.gameObject.SetActive(true);
        interfaceView.BonusButton.gameObject.SetActive(true);
    }

    private void HandleBonusButton()
    {
        if (!isAwaitingChoice || isComplete)
            return;
        if (isBonusRound)
            RetryBonusGoals();
        else
            StartBonusRound();
    }

    private void HandleRetryButton()
    {
        if (!isAwaitingChoice || isComplete)
            return;
        ResetFullRun();
    }

    private void HandleFinishButton()
    {
        if (!isAwaitingChoice || isComplete)
            return;

        isAwaitingChoice = false;
        isComplete = true;
        interfaceView.RetryButton.interactable = false;
        interfaceView.FinishButton.interactable = false;
        interfaceView.BonusButton.interactable = false;
        onMinigameCompleted?.Invoke();
    }

    private void RecordRequiredPerformance()
    {
        PerformanceGrade grade = CalculateRequiredGrade();
        if (!hasRequiredResult || IsBetterResult(
                grade,
                requiredAttemptCount,
                bestRequiredGrade,
                bestRequiredAttemptCount))
        {
            hasRequiredResult = true;
            bestRequiredGrade = grade;
            bestRequiredAttemptCount = requiredAttemptCount;
        }
    }

    private void RecordBonusPerformance()
    {
        PerformanceGrade grade = CalculateBonusGrade();
        if (!hasBonusResult || IsBetterResult(
                grade,
                bonusAttemptCount,
                bestBonusGrade,
                bestBonusAttemptCount))
        {
            hasBonusResult = true;
            bestBonusGrade = grade;
            bestBonusAttemptCount = bonusAttemptCount;
        }
    }

    private static bool IsBetterResult(
        PerformanceGrade candidateGrade,
        int candidateAttempts,
        PerformanceGrade bestGrade,
        int bestAttempts)
    {
        return candidateGrade < bestGrade ||
               candidateGrade == bestGrade && candidateAttempts < bestAttempts;
    }

    private void StartBonusRound()
    {
        isBonusRound = true;
        isAwaitingChoice = false;
        bonusAttemptCount = 0;
        interfaceView.CompletionPopup.SetActive(false);
        BeginNextAttempt();
    }

    private void ResetFullRun()
    {
        goalsMade = 0;
        requiredAttemptCount = 0;
        bonusAttemptCount = 0;
        streak = 0;
        bestStreak = 0;
        isBonusRound = false;
        isAwaitingChoice = false;
        isComplete = false;
        interfaceView.CompletionPopup.SetActive(false);
        BeginNextAttempt();
    }

    private void RetryBonusGoals()
    {
        goalsMade = requiredGoals;
        bonusAttemptCount = 0;
        streak = 0;
        isBonusRound = true;
        isAwaitingChoice = false;
        interfaceView.CompletionPopup.SetActive(false);
        BeginNextAttempt();
    }

    private PerformanceGrade CalculateRequiredGrade()
    {
        if (requiredAttemptCount <= requiredAGradeMaximumAttempts)
            return PerformanceGrade.A;
        return requiredAttemptCount <= requiredBGradeMaximumAttempts
            ? PerformanceGrade.B
            : PerformanceGrade.C;
    }

    private PerformanceGrade CalculateBonusGrade()
    {
        if (bonusAttemptCount <= bonusAGradeMaximumAttempts)
            return PerformanceGrade.A;
        return bonusAttemptCount <= bonusBGradeMaximumAttempts
            ? PerformanceGrade.B
            : PerformanceGrade.C;
    }

    private string FormatGrade(PerformanceGrade grade)
    {
        switch (grade)
        {
            case PerformanceGrade.A:
                return $"<color=#{ColorUtility.ToHtmlStringRGB(aGradeColor)}>A</color>";
            case PerformanceGrade.B:
                return $"<color=#{ColorUtility.ToHtmlStringRGB(bGradeColor)}>B</color>";
            default:
                return $"<color=#{ColorUtility.ToHtmlStringRGB(cGradeColor)}>C</color>";
        }
    }

    private int GetTotalAttemptCount() => requiredAttemptCount + bonusAttemptCount;

    private int GetAccuracyPercent(float distance, float halfWidth, float targetCenter)
    {
        if (halfWidth <= Mathf.Epsilon)
            return 100;

        float perfectRadius = halfWidth * perfectWindowFraction;
        if (distance <= perfectRadius)
            return 100;

        if (distance <= halfWidth)
        {
            float goalEdgeProgress = Mathf.InverseLerp(perfectRadius, halfWidth, distance);
            return Mathf.RoundToInt(Mathf.Lerp(100f, goalEdgeAccuracy, goalEdgeProgress));
        }

        float farthestBarDistance = Mathf.Max(targetCenter, 1f - targetCenter);
        if (farthestBarDistance <= halfWidth)
            return goalEdgeAccuracy;

        float outsideProgress = Mathf.InverseLerp(halfWidth, farthestBarDistance, distance);
        return Mathf.RoundToInt(Mathf.Lerp(goalEdgeAccuracy, 0f, outsideProgress));
    }

    private static float GetDifficultyValue(float[] values, int completedGoals, float fallback)
    {
        if (values == null || values.Length == 0)
            return fallback;

        return values[Mathf.Clamp(completedGoals, 0, values.Length - 1)];
    }

    private enum PerformanceGrade { A, B, C }
}
