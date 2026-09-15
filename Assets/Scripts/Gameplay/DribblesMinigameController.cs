using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(DribblesMinigameWorld))]
public sealed class DribblesMinigameController : MonoBehaviour
{
    private const float PlayerRadius = 0.52f;
    private const float BallRadius = 0.42f;
    private const int BallDirectionCount = 16;
    private const float BallDirectionStep = 360f / BallDirectionCount;
    private const float DefaultATimeMaximum = 8f;
    private const float DefaultBTimeMaximum = 22f;
    private const string AScoreColor = "#63E681";
    private const string BScoreColor = "#55DDF2";
    private const string CScoreColor = "#FF765F";

    private static readonly Color FieldLineColor = new Color32(223, 239, 217, 255);
    private static readonly Color PlayerColor = new Color32(45, 126, 214, 255);
    private static readonly Color ActiveCheckpointColor = new Color32(255, 210, 58, 255);

    [Header("Course Layout")]
    [Tooltip("Bottom-left corner of the playable field.")]
    [SerializeField] private Vector2 fieldMinimum = new Vector2(-9.6f, -6.35f);
    [Tooltip("Top-right corner of the playable field.")]
    [SerializeField] private Vector2 fieldMaximum = new Vector2(9.6f, 6.35f);
    [SerializeField] private Vector2 playerStartPosition = new Vector2(0f, -5.65f);
    [SerializeField] private Vector2 ballStartPosition = new Vector2(0f, -4.85f);
    [Tooltip("Course parents in play order. Each direct child is a checkpoint, crossed in hierarchy order.")]
    [SerializeField] private Transform[] courseRoots;
    [Tooltip("Grade time limits in course order. Missing entries use the default 8-second A and 22-second B limits.")]
    [SerializeField] private CourseRatingThresholds[] courseRatingThresholds;
    [Tooltip("Courses at the start of the list that must be completed before the minigame can finish.")]
    [SerializeField, Min(1)] private int requiredCourseCount = 2;
    [Tooltip("Invoked when the player chooses Continue from the final results screen.")]
    [SerializeField] private UnityEvent onMinigameCompleted;
    [SerializeField, Min(0.5f)] private float checkpointHalfWidth = 1.3f;
    [SerializeField, Min(0.25f)] private float checkpointCrossingHalfWidth = 1.05f;

    [Header("Player")]
    [SerializeField, Min(1f)] private float playerMoveSpeed = 18f;

    [Header("Ball")]
    [SerializeField, Min(0.1f)] private float ballMaximumSpeed = 7f;
    [Tooltip("Half-angle around each of 16 directions in which ball movement snaps to that direction.")]
    [SerializeField, Range(0f, BallDirectionStep * 0.5f)]
    private float ballDirectionDeadZone = 5f;

    [Header("Components")]
    [SerializeField] private DribblesMinigameWorld minigameWorld;
    [SerializeField] private DribblesMinigameCameraController cameraController;

    [Header("Interface")]
    [SerializeField] private DribblesMinigameView interfaceView;

    private readonly List<CourseRuntime> courses = new List<CourseRuntime>();

    private Rigidbody2D playerBody;
    private Rigidbody2D ballBody;
    private Vector2 dragTarget;
    private Vector2 dragPointerOffset;
    private Vector2 previousBallPosition;
    private bool isPointerControlActive;
    private bool isTimerRunning;
    private bool isCourseComplete;
    private bool isMinigameFinished;
    private int currentCourseIndex;
    private int nextCheckpointIndex;
    private float startTime;
    private float completionTime;
    private TextMeshProUGUI timerText;
    private GameObject startPrompt;
    private GameObject restartHint;
    private GameObject completionPanel;
    private RectTransform completionPraiseRect;
    private RectTransform completionMessageRect;
    private TextMeshProUGUI completionTitleText;
    private TextMeshProUGUI completionGradeText;
    private TextMeshProUGUI completionTimeText;
    private TextMeshProUGUI completionPraiseText;
    private TextMeshProUGUI completionMessageText;
    private GameObject finalScoreboard;
    private TextMeshProUGUI finalCourseColumn;
    private TextMeshProUGUI finalScoreColumn;
    private TextMeshProUGUI finalTimeColumn;
    private Button retryButton;
    private Button continueButton;
    private Button optionalButton;
    private RectTransform retryButtonRect;
    private RectTransform continueButtonRect;
    private RectTransform optionalButtonRect;
    private TextMeshProUGUI retryButtonText;
    private TextMeshProUGUI continueButtonText;
    private TextMeshProUGUI optionalButtonText;

    private enum CourseRating
    {
        None,
        C,
        B,
        A
    }

    [System.Serializable]
    private sealed class CourseRatingThresholds
    {
        [Min(0f)] public float aTimeMaximum = DefaultATimeMaximum;
        [Min(0f)] public float bTimeMaximum = DefaultBTimeMaximum;
    }

    private sealed class CourseRuntime
    {
        public Transform Root;
        public readonly List<Transform> Checkpoints = new List<Transform>();
        public readonly List<DribblesMinigameWorld.CheckpointInstance> Visuals =
            new List<DribblesMinigameWorld.CheckpointInstance>();
        public bool HasCompletion;
        public float CompletionTime;
        public CourseRating Rating;
    }

    private void Awake()
    {
        if (minigameWorld == null)
            minigameWorld = GetComponent<DribblesMinigameWorld>();
        if (cameraController == null)
            cameraController = FindFirstObjectByType<DribblesMinigameCameraController>();

        if (minigameWorld == null || cameraController == null)
        {
            Debug.LogError("Dribbles minigame components are missing from the parent prefab.", this);
            enabled = false;
            return;
        }

        minigameWorld.CreateBoundaries(fieldMinimum, fieldMaximum);
        CreateCourses();
        playerBody = minigameWorld.SpawnPlayer(playerStartPosition);
        ballBody = minigameWorld.SpawnBall(ballStartPosition);
        if (playerBody == null || ballBody == null)
        {
            enabled = false;
            return;
        }

        cameraController.Initialize(playerBody, fieldMinimum, fieldMaximum, playerMoveSpeed);
        BindInterface();
        StartCourse(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentCourse();
            return;
        }

        HandleMouseInput();
        PulseActiveCheckpoint();
        RefreshInterface();
    }

    private void LateUpdate()
    {
        CourseRuntime currentCourse = GetCurrentCourse();
        cameraController.Follow(
            GetNextCheckpoint(currentCourse),
            nextCheckpointIndex,
            currentCourse != null ? currentCourse.Checkpoints.Count : 0);
    }

    private void FixedUpdate()
    {
        if (isCourseComplete)
            return;

        if (isPointerControlActive)
        {
            Vector2 currentPosition = playerBody.position;
            Vector2 nextPosition = Vector2.MoveTowards(
                currentPosition,
                dragTarget,
                playerMoveSpeed * Time.fixedDeltaTime);

            if (!isTimerRunning && (nextPosition - currentPosition).sqrMagnitude > 0.000001f)
            {
                isTimerRunning = true;
                startTime = Time.unscaledTime;
            }
            playerBody.MovePosition(nextPosition);
        }

        LimitBallSpeed();
        SnapBallDirectionWithinDeadZone();
        CheckForCheckpointCrossing();
    }

    private void LimitBallSpeed()
    {
        if (ballBody.linearVelocity.sqrMagnitude <= ballMaximumSpeed * ballMaximumSpeed)
            return;
        ballBody.linearVelocity = ballBody.linearVelocity.normalized * ballMaximumSpeed;
    }

    private void SnapBallDirectionWithinDeadZone()
    {
        Vector2 velocity = ballBody.linearVelocity;
        float speed = velocity.magnitude;
        if (speed <= Mathf.Epsilon)
            return;

        float directionAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(directionAngle / BallDirectionStep) * BallDirectionStep;
        if (Mathf.Abs(Mathf.DeltaAngle(directionAngle, snappedAngle)) > ballDirectionDeadZone)
            return;

        float snappedRadians = snappedAngle * Mathf.Deg2Rad;
        ballBody.linearVelocity = new Vector2(
            Mathf.Cos(snappedRadians),
            Mathf.Sin(snappedRadians)) * speed;
    }

    private void OnDestroy()
    {
        retryButton?.onClick.RemoveListener(HandleRetryButton);
        continueButton?.onClick.RemoveListener(HandleContinueButton);
        optionalButton?.onClick.RemoveListener(HandleOptionalButton);
    }

    private void OnDrawGizmos()
    {
        Vector2 bottomLeft = fieldMinimum;
        Vector2 bottomRight = new Vector2(fieldMaximum.x, fieldMinimum.y);
        Vector2 topRight = fieldMaximum;
        Vector2 topLeft = new Vector2(fieldMinimum.x, fieldMaximum.y);

        Gizmos.color = FieldLineColor;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        Gizmos.color = ActiveCheckpointColor;
        if (courseRoots != null)
        {
            for (int courseIndex = 0; courseIndex < courseRoots.Length; courseIndex++)
            {
                Transform courseRoot = courseRoots[courseIndex];
                if (courseRoot == null)
                    continue;

                for (int checkpointIndex = 0; checkpointIndex < courseRoot.childCount; checkpointIndex++)
                {
                    Transform checkpoint = courseRoot.GetChild(checkpointIndex);
                    Vector3 leftPost = checkpoint.TransformPoint(Vector3.left * checkpointHalfWidth);
                    Vector3 rightPost = checkpoint.TransformPoint(Vector3.right * checkpointHalfWidth);
                    Gizmos.DrawWireSphere(leftPost, 0.23f);
                    Gizmos.DrawWireSphere(rightPost, 0.23f);
                    Gizmos.DrawLine(leftPost, rightPost);
                }
            }
        }

        Gizmos.color = PlayerColor;
        Gizmos.DrawWireSphere(playerStartPosition, PlayerRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(ballStartPosition, BallRadius);
    }

    private void BindInterface()
    {
        if (interfaceView == null)
            interfaceView = FindFirstObjectByType<DribblesMinigameView>(FindObjectsInactive.Include);

        if (interfaceView == null)
        {
            Debug.LogError("Dribbles UI prefab is missing from the scene.", this);
            return;
        }

        timerText = interfaceView.TimerText;
        startPrompt = interfaceView.StartPrompt;
        restartHint = interfaceView.RestartHint;
        completionPanel = interfaceView.CompletionPanel;
        completionPraiseRect = interfaceView.CompletionPraiseRect;
        completionMessageRect = interfaceView.CompletionMessageRect;
        completionTitleText = interfaceView.CompletionTitleText;
        completionGradeText = interfaceView.CompletionGradeText;
        completionTimeText = interfaceView.CompletionTimeText;
        completionPraiseText = interfaceView.CompletionPraiseText;
        completionMessageText = interfaceView.CompletionMessageText;
        finalScoreboard = interfaceView.FinalScoreboard;
        finalCourseColumn = interfaceView.FinalCourseColumn;
        finalScoreColumn = interfaceView.FinalScoreColumn;
        finalTimeColumn = interfaceView.FinalTimeColumn;
        retryButton = interfaceView.RetryButton;
        continueButton = interfaceView.ContinueButton;
        optionalButton = interfaceView.OptionalButton;
        retryButtonRect = interfaceView.RetryButtonRect;
        continueButtonRect = interfaceView.ContinueButtonRect;
        optionalButtonRect = interfaceView.OptionalButtonRect;
        retryButtonText = interfaceView.RetryButtonText;
        continueButtonText = interfaceView.ContinueButtonText;
        optionalButtonText = interfaceView.OptionalButtonText;

        retryButton.onClick.AddListener(HandleRetryButton);
        continueButton.onClick.AddListener(HandleContinueButton);
        optionalButton.onClick.AddListener(HandleOptionalButton);
        startPrompt.SetActive(false);
        restartHint.SetActive(false);
        completionPanel.SetActive(false);
    }

    private void RefreshInterface()
    {
        if (timerText == null || completionPanel == null)
            return;

        float elapsed = isCourseComplete
            ? completionTime
            : isTimerRunning ? Time.unscaledTime - startTime : 0f;
        string courseValue = courses.Count > 0
            ? $"{currentCourseIndex + 1} / {courses.Count}"
            : "NONE";
        timerText.text = isMinigameFinished
            ? "<color=#FFFFFF>MINIGAME COMPLETE!</color>"
            : FormatScoreboardValue("COURSE", courseValue) +
              "    " + FormatScoreboardValue("TIME", FormatTime(elapsed));

        bool showCompletionPanel = isCourseComplete && courses.Count > 0;
        completionPanel.SetActive(showCompletionPanel);
        if (!showCompletionPanel)
            return;

        if (isMinigameFinished)
            ConfigureFinishedPanel();
        else
            ConfigureCourseCompletionPanel();
    }

    private void ConfigureCourseCompletionPanel()
    {
        bool hasNextRequiredCourse = currentCourseIndex + 1 < GetRequiredCourseCount();
        bool completedLastRequiredCourse = currentCourseIndex == GetRequiredCourseCount() - 1;
        bool isOptionalCourse = currentCourseIndex >= GetRequiredCourseCount();
        bool hasNextOptionalCourse = isOptionalCourse && currentCourseIndex < courses.Count - 1;
        bool canPlayOptionalCourse = completedLastRequiredCourse && currentCourseIndex < courses.Count - 1;
        bool showOptionalButton = canPlayOptionalCourse || hasNextOptionalCourse;

        completionTitleText.text = $"COURSE {currentCourseIndex + 1} COMPLETE!";
        completionGradeText.gameObject.SetActive(true);
        completionTimeText.gameObject.SetActive(true);
        finalScoreboard.SetActive(false);
        CourseRuntime completedCourse = GetCurrentCourse();
        completionGradeText.text = completedCourse != null
            ? GetScoreGrade(completedCourse.Rating)
            : "-";
        completionTimeText.text =
            $"<color=#FFFFFF>{FormatTime(completionTime)}</color> " +
            "<color=#FFD23A>seconds</color>";
        completionPraiseText.gameObject.SetActive(canPlayOptionalCourse);
        completionMessageText.gameObject.SetActive(canPlayOptionalCourse);

        if (canPlayOptionalCourse)
        {
            SetRectLayout(completionPraiseRect, new Vector2(780f, 64f), new Vector2(0f, -160f));
            SetRectLayout(completionMessageRect, new Vector2(720f, 72f), new Vector2(0f, -225f));
            completionPraiseText.text = "Good job!";
            completionMessageText.text = "There's one BONUS course for more points!";
        }

        retryButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
        optionalButton.gameObject.SetActive(showOptionalButton);
        retryButtonText.text = "Retry Course";
        continueButtonText.text = hasNextRequiredCourse ? "Next Course" : "Finish Minigame";
        optionalButtonText.text = "Play Bonus Course";

        float primaryRowY = showOptionalButton ? -350f : -475f;
        SetRectLayout(retryButtonRect, new Vector2(390f, 94f), new Vector2(-210f, primaryRowY));
        SetRectLayout(continueButtonRect, new Vector2(390f, 94f), new Vector2(210f, primaryRowY));
        if (showOptionalButton)
        {
            SetRectLayout(optionalButtonRect, new Vector2(810f, 94f), new Vector2(0f, -475f));
        }
    }

    private void ConfigureFinishedPanel()
    {
        completionTitleText.text = "DRIBBLES COMPLETE!";
        completionGradeText.gameObject.SetActive(false);
        completionTimeText.gameObject.SetActive(false);
        finalScoreboard.SetActive(true);
        PopulateFinalScoreboard();
        completionPraiseText.gameObject.SetActive(true);
        completionMessageText.gameObject.SetActive(false);
        completionPraiseText.text = "Good job!";
        SetRectLayout(completionPraiseRect, new Vector2(780f, 70f), new Vector2(0f, 300f));

        retryButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
        optionalButton.gameObject.SetActive(false);
        retryButtonText.text = "Replay Courses";
        continueButtonText.text = "Continue";
        SetRectLayout(retryButtonRect, new Vector2(390f, 94f), new Vector2(-210f, -475f));
        SetRectLayout(continueButtonRect, new Vector2(390f, 94f), new Vector2(210f, -475f));
    }

    private static void SetRectLayout(RectTransform rectTransform, Vector2 size, Vector2 position)
    {
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
    }

    private void HandleRetryButton()
    {
        if (isMinigameFinished)
        {
            StartCourse(0);
            return;
        }

        RestartCurrentCourse();
    }

    private void HandleContinueButton()
    {
        if (isMinigameFinished)
        {
            onMinigameCompleted?.Invoke();
            return;
        }

        if (currentCourseIndex + 1 < GetRequiredCourseCount())
        {
            StartCourse(currentCourseIndex + 1);
            return;
        }

        FinishMinigame();
    }

    private void HandleOptionalButton()
    {
        int optionalCourseIndex = isMinigameFinished
            ? GetRequiredCourseCount()
            : currentCourseIndex + 1;
        StartCourse(optionalCourseIndex);
    }

    private void CreateCourses()
    {
        if (courseRoots == null)
            return;

        for (int courseIndex = 0; courseIndex < courseRoots.Length; courseIndex++)
        {
            Transform courseRoot = courseRoots[courseIndex];
            if (courseRoot == null)
                continue;

            CourseRuntime course = new CourseRuntime
            {
                Root = courseRoot
            };
            courses.Add(course);

            int checkpointCount = courseRoot.childCount;
            for (int checkpointIndex = 0; checkpointIndex < checkpointCount; checkpointIndex++)
            {
                Transform checkpoint = courseRoot.GetChild(checkpointIndex);
                course.Checkpoints.Add(checkpoint);

                Transform nextCheckpoint = checkpointIndex + 1 < checkpointCount
                    ? courseRoot.GetChild(checkpointIndex + 1)
                    : null;
                course.Visuals.Add(minigameWorld.SpawnCheckpoint(
                    checkpoint,
                    nextCheckpoint,
                    checkpointHalfWidth));
            }
        }
    }

    private void HandleMouseInput()
    {
        Camera gameplayCamera = cameraController.GameplayCamera;
        if (isCourseComplete || gameplayCamera == null)
            return;

        Vector3 pointerScreenPosition = Input.mousePosition;
        pointerScreenPosition.z = -gameplayCamera.transform.position.z;
        Vector2 pointerWorldPosition = gameplayCamera.ScreenToWorldPoint(pointerScreenPosition);

        if (Input.GetMouseButtonDown(0))
        {
            float selectionRadius = PlayerRadius * 1.25f;
            if ((pointerWorldPosition - playerBody.position).sqrMagnitude <= selectionRadius * selectionRadius)
            {
                isPointerControlActive = true;
                dragTarget = playerBody.position;
                dragPointerOffset = playerBody.position - pointerWorldPosition;
                startPrompt?.SetActive(false);
                restartHint?.SetActive(true);
            }
        }

        if (isPointerControlActive)
        {
            pointerScreenPosition = Input.mousePosition;
            pointerScreenPosition.z = -gameplayCamera.transform.position.z;
            pointerWorldPosition = gameplayCamera.ScreenToWorldPoint(pointerScreenPosition);
            Vector2 desiredPosition = pointerWorldPosition + dragPointerOffset;

            dragTarget = new Vector2(
                Mathf.Clamp(desiredPosition.x, fieldMinimum.x + PlayerRadius, fieldMaximum.x - PlayerRadius),
                Mathf.Clamp(desiredPosition.y, fieldMinimum.y + PlayerRadius, fieldMaximum.y - PlayerRadius));
        }
    }

    private void CheckForCheckpointCrossing()
    {
        Vector2 currentBallPosition = ballBody.position;
        CourseRuntime currentCourse = GetCurrentCourse();
        if (currentCourse != null && nextCheckpointIndex < currentCourse.Checkpoints.Count)
        {
            Transform checkpoint = currentCourse.Checkpoints[nextCheckpointIndex];
            Vector2 previousLocalPosition = checkpoint.InverseTransformPoint(previousBallPosition);
            Vector2 currentLocalPosition = checkpoint.InverseTransformPoint(currentBallPosition);
            bool crossedUpward = previousLocalPosition.y <= 0f && currentLocalPosition.y > 0f;

            if (crossedUpward)
            {
                float verticalTravel = currentLocalPosition.y - previousLocalPosition.y;
                float crossingProgress = -previousLocalPosition.y / verticalTravel;
                float crossingX = Mathf.Lerp(previousLocalPosition.x, currentLocalPosition.x, crossingProgress);

                if (Mathf.Abs(crossingX) <= checkpointCrossingHalfWidth)
                {
                    bool isFinalCheckpoint = nextCheckpointIndex == currentCourse.Checkpoints.Count - 1;
                    minigameWorld.PlayCheckpointBurst(checkpoint.position, isFinalCheckpoint);
                    nextCheckpointIndex++;
                    RefreshCourseState();

                    if (nextCheckpointIndex >= currentCourse.Checkpoints.Count)
                    {
                        CompleteCourse();
                    }
                }
            }
        }

        previousBallPosition = currentBallPosition;
    }

    private void RefreshCourseState()
    {
        for (int courseIndex = 0; courseIndex < courses.Count; courseIndex++)
        {
            CourseRuntime course = courses[courseIndex];
            bool isCurrentCourse = courseIndex == currentCourseIndex;
            course.Root.gameObject.SetActive(isCurrentCourse);

            for (int checkpointIndex = 0; checkpointIndex < course.Visuals.Count; checkpointIndex++)
            {
                DribblesMinigameWorld.CheckpointInstance visual = course.Visuals[checkpointIndex];
                bool isActiveCheckpoint = isCurrentCourse &&
                    !isCourseComplete && checkpointIndex == nextCheckpointIndex;
                minigameWorld.SetCheckpointActive(visual, isActiveCheckpoint);
            }
        }
    }

    private void PulseActiveCheckpoint()
    {
        CourseRuntime currentCourse = GetCurrentCourse();
        if (isCourseComplete || currentCourse == null || nextCheckpointIndex >= currentCourse.Visuals.Count)
            return;

        float pulse = (Mathf.Sin(Time.unscaledTime * 5f) + 1f) * 0.08f;
        minigameWorld.PulseCheckpoint(currentCourse.Visuals[nextCheckpointIndex], pulse);
    }

    private void CompleteCourse()
    {
        isCourseComplete = true;
        isPointerControlActive = false;
        startPrompt?.SetActive(false);
        restartHint?.SetActive(false);
        completionTime = isTimerRunning ? Time.unscaledTime - startTime : 0f;

        CourseRuntime completedCourse = GetCurrentCourse();
        if (completedCourse != null)
        {
            completedCourse.HasCompletion = true;
            completedCourse.CompletionTime = completionTime;
            completedCourse.Rating = CalculateCourseRating(completionTime, currentCourseIndex);
        }

        playerBody.linearVelocity = Vector2.zero;
        ballBody.linearVelocity = Vector2.zero;
        ballBody.angularVelocity = 0f;
        ballBody.bodyType = RigidbodyType2D.Kinematic;
        RefreshCourseState();
    }

    private void FinishMinigame()
    {
        if (currentCourseIndex + 1 < GetRequiredCourseCount())
            return;

        isMinigameFinished = true;
    }

    private void RestartCurrentCourse()
    {
        StartCourse(currentCourseIndex);
    }

    private void StartCourse(int courseIndex)
    {
        if (courses.Count == 0)
        {
            isCourseComplete = true;
            return;
        }

        currentCourseIndex = Mathf.Clamp(courseIndex, 0, courses.Count - 1);
        ClearCourseResultsFrom(currentCourseIndex);
        isPointerControlActive = false;
        isTimerRunning = false;
        isCourseComplete = false;
        isMinigameFinished = false;
        nextCheckpointIndex = 0;
        completionTime = 0f;

        playerBody.position = playerStartPosition;
        playerBody.rotation = 0f;
        playerBody.linearVelocity = Vector2.zero;
        playerBody.angularVelocity = 0f;

        ballBody.bodyType = RigidbodyType2D.Dynamic;
        ballBody.position = ballStartPosition;
        ballBody.rotation = 0f;
        ballBody.linearVelocity = Vector2.zero;
        ballBody.angularVelocity = 0f;
        ballBody.WakeUp();

        dragTarget = playerStartPosition;
        previousBallPosition = ballStartPosition;
        dragPointerOffset = Vector2.zero;
        startTime = 0f;
        startPrompt?.SetActive(true);
        restartHint?.SetActive(false);

        minigameWorld.ClearCheckpointEffects();
        RefreshCourseState();

        CourseRuntime currentCourse = GetCurrentCourse();
        cameraController.Snap(
            GetNextCheckpoint(currentCourse),
            nextCheckpointIndex,
            currentCourse != null ? currentCourse.Checkpoints.Count : 0);
        if (currentCourse == null || currentCourse.Checkpoints.Count == 0)
            CompleteCourse();
    }

    private CourseRuntime GetCurrentCourse()
    {
        if (currentCourseIndex < 0 || currentCourseIndex >= courses.Count)
            return null;

        return courses[currentCourseIndex];
    }

    private Transform GetNextCheckpoint(CourseRuntime course)
    {
        if (isCourseComplete || course == null ||
            nextCheckpointIndex < 0 || nextCheckpointIndex >= course.Checkpoints.Count)
            return null;

        return course.Checkpoints[nextCheckpointIndex];
    }

    private void ClearCourseResultsFrom(int firstCourseIndex)
    {
        for (int courseIndex = firstCourseIndex; courseIndex < courses.Count; courseIndex++)
        {
            CourseRuntime course = courses[courseIndex];
            course.HasCompletion = false;
            course.CompletionTime = 0f;
            course.Rating = CourseRating.None;
        }
    }

    private CourseRating CalculateCourseRating(float courseTime, int courseIndex)
    {
        float aTimeMaximum = DefaultATimeMaximum;
        float bTimeMaximum = DefaultBTimeMaximum;
        if (courseRatingThresholds != null &&
            courseIndex >= 0 && courseIndex < courseRatingThresholds.Length &&
            courseRatingThresholds[courseIndex] != null)
        {
            CourseRatingThresholds thresholds = courseRatingThresholds[courseIndex];
            aTimeMaximum = Mathf.Max(0f, thresholds.aTimeMaximum);
            bTimeMaximum = Mathf.Max(aTimeMaximum, thresholds.bTimeMaximum);
        }

        if (courseTime <= aTimeMaximum)
            return CourseRating.A;

        return courseTime <= bTimeMaximum ? CourseRating.B : CourseRating.C;
    }

    private void PopulateFinalScoreboard()
    {
        string courseColumn = "<color=#FFD23A>COURSE:</color>";
        string scoreColumn = "<color=#FFD23A>SCORE:</color>";
        string timeColumn = "<color=#FFD23A>TIME:</color>";
        for (int courseIndex = 0; courseIndex < courses.Count; courseIndex++)
        {
            CourseRuntime course = courses[courseIndex];
            if (!course.HasCompletion)
                continue;

            courseColumn += "\n" + (courseIndex + 1);
            scoreColumn += "\n" + GetScoreGrade(course.Rating);
            timeColumn += "\n" + FormatTime(course.CompletionTime);
        }

        finalCourseColumn.text = courseColumn;
        finalScoreColumn.text = scoreColumn;
        finalTimeColumn.text = timeColumn;
    }

    private static string GetScoreGrade(CourseRating rating)
    {
        switch (rating)
        {
            case CourseRating.A:
                return $"<color={AScoreColor}>A</color>";
            case CourseRating.B:
                return $"<color={BScoreColor}>B</color>";
            case CourseRating.C:
                return $"<color={CScoreColor}>C</color>";
            default:
                return "-";
        }
    }

    private static string FormatScoreboardValue(string label, string value) => $"<color=#FFD23A>{label}:</color> <color=#FFFFFF>{value}</color>";

    private int GetRequiredCourseCount()
    {
        return courses.Count == 0
            ? 0
            : Mathf.Clamp(requiredCourseCount, 1, courses.Count);
    }

    private static string FormatTime(float totalSeconds) => $"{Mathf.Max(0f, totalSeconds):00.0}";
}
