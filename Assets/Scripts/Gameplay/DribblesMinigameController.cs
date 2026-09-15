using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class DribblesMinigameController : MonoBehaviour
{
    private const float PlayerRadius = 0.52f;
    private const float BallRadius = 0.42f;
    private const int BallDirectionCount = 16;
    private const float BallDirectionStep = 360f / BallDirectionCount;
    private const float CheckpointArrowDirectionInfluence = 0.65f;
    private const float MaximumCheckpointArrowAngle = 32f;
    private const float MaximumCourseProgressLookAheadMultiplier = 1.1f;
    private const float MinimumCameraLookAheadRatio = 0.75f;
    private const float MinimumCameraFollowRatio = 0.25f;
    private const float CameraForwardFollowRatio = 0.55f;
    private const float MinimumCameraFollowSpeed = 0.75f;
    private const float CameraEdgeFollowStart = 0.45f;
    private const float CameraEdgeFollowEnd = 0.78f;
    private const float DefaultATimeMaximum = 8f;
    private const float DefaultBTimeMaximum = 22f;
    private const string AScoreColor = "#63E681";
    private const string BScoreColor = "#55DDF2";
    private const string CScoreColor = "#FF765F";

    private static readonly Color FieldColor = new Color32(25, 111, 62, 255);
    private static readonly Color FieldLineColor = new Color32(223, 239, 217, 255);
    private static readonly Color PlayerColor = new Color32(45, 126, 214, 255);
    private static readonly Color ActiveCheckpointColor = new Color32(255, 210, 58, 255);
    private static readonly Color InactiveCheckpointColor = new Color32(218, 233, 240, 235);

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
    [SerializeField] private Sprite ballSprite;
    [SerializeField, Min(0.1f)] private float ballMass = 1.35f;
    [SerializeField, Min(0f)] private float ballLinearDamping = 2.15f;
    [SerializeField, Min(0.1f)] private float ballMaximumSpeed = 7f;
    [Tooltip("Half-angle around each of 16 directions in which ball movement snaps to that direction.")]
    [SerializeField, Range(0f, BallDirectionStep * 0.5f)]
    private float ballDirectionDeadZone = 5f;

    [Header("Camera")]
    [SerializeField, Min(1f)] private float cameraOrthographicSize = 6f;
    [SerializeField, Min(0.01f)] private float cameraFollowSmoothTime = 0.28f;
    [SerializeField, Range(0f, 0.5f)] private float cameraCheckpointLookAhead = 0.35f;

    [Header("Interface")]
    [SerializeField] private DribblesMinigameView interfaceView;

    private readonly List<CourseRuntime> courses = new List<CourseRuntime>();
    private readonly List<GameObject> checkpointEffects = new List<GameObject>();
    private readonly List<Object> generatedAssets = new List<Object>();

    private Camera gameplayCamera;
    private Sprite squareSprite;
    private Sprite circleSprite;
    private Material particleMaterial;
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
    private Vector3 cameraFollowVelocity;
    private Vector2 previousCameraPlayerPosition;
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
        public readonly List<CheckpointVisual> Visuals = new List<CheckpointVisual>();
        public bool HasCompletion;
        public float CompletionTime;
        public CourseRating Rating;
    }

    private sealed class CheckpointVisual
    {
        public GameObject Root;
        public readonly List<SpriteRenderer> Renderers = new List<SpriteRenderer>();
        public readonly List<Collider2D> Colliders = new List<Collider2D>();
    }

    private void Awake()
    {
        SetupCamera();
        CreateShapeSprites();
        CreateParticleMaterial();
        CreateBoundaries();
        CreateCourses();
        CreatePlayer();
        CreateBall();
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
        FollowPlayerWithCamera();
    }

    private void FixedUpdate()
    {
        if (isCourseComplete)
        {
            return;
        }

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
        {
            return;
        }

        ballBody.linearVelocity = ballBody.linearVelocity.normalized * ballMaximumSpeed;
    }

    private void SnapBallDirectionWithinDeadZone()
    {
        Vector2 velocity = ballBody.linearVelocity;
        float speed = velocity.magnitude;
        if (speed <= Mathf.Epsilon)
        {
            return;
        }

        float directionAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(directionAngle / BallDirectionStep) * BallDirectionStep;
        if (Mathf.Abs(Mathf.DeltaAngle(directionAngle, snappedAngle)) > ballDirectionDeadZone)
        {
            return;
        }

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

        for (int i = 0; i < generatedAssets.Count; i++)
        {
            Destroy(generatedAssets[i]);
        }
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
                {
                    continue;
                }

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
        {
            interfaceView = FindFirstObjectByType<DribblesMinigameView>(FindObjectsInactive.Include);
        }

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

        Material gradeMaterial = CreateGradeFontMaterial();
        if (completionGradeText != null && gradeMaterial != null)
        {
            completionGradeText.fontSharedMaterial = gradeMaterial;
        }

        retryButton.onClick.AddListener(HandleRetryButton);
        continueButton.onClick.AddListener(HandleContinueButton);
        optionalButton.onClick.AddListener(HandleOptionalButton);
        startPrompt.SetActive(false);
        restartHint.SetActive(false);
        completionPanel.SetActive(false);
    }

    private Material CreateGradeFontMaterial()
    {
        TMP_FontAsset gradeFont = completionGradeText != null ? completionGradeText.font : null;
        if (gradeFont == null || gradeFont.material == null)
        {
            return null;
        }

        Material gradeMaterial = new Material(gradeFont.material)
        {
            name = "Dribbles Grade Font Material"
        };
        if (gradeMaterial.HasProperty("_FaceDilate"))
        {
            gradeMaterial.SetFloat("_FaceDilate", -0.1f);
        }

        if (gradeMaterial.HasProperty("_OutlineWidth"))
        {
            gradeMaterial.SetFloat("_OutlineWidth", 0.12f);
        }

        if (gradeMaterial.HasProperty("_OutlineColor"))
        {
            gradeMaterial.SetColor("_OutlineColor", Color.black);
        }

        gradeMaterial.EnableKeyword("OUTLINE_ON");
        gradeMaterial.DisableKeyword("UNDERLAY_ON");

        generatedAssets.Add(gradeMaterial);
        return gradeMaterial;
    }

    private void RefreshInterface()
    {
        if (timerText == null || completionPanel == null)
        {
            return;
        }

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
        {
            return;
        }

        if (isMinigameFinished)
        {
            ConfigureFinishedPanel();
        }
        else
        {
            ConfigureCourseCompletionPanel();
        }
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

    private void SetupCamera()
    {
        gameplayCamera = Camera.main;
        if (gameplayCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            gameplayCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        gameplayCamera.transform.position = new Vector3(0f, 0f, -10f);
        gameplayCamera.orthographic = true;
        gameplayCamera.orthographicSize = cameraOrthographicSize;
        gameplayCamera.clearFlags = CameraClearFlags.SolidColor;
        gameplayCamera.backgroundColor = FieldColor;
    }

    private void SnapCameraToPlayer()
    {
        if (gameplayCamera == null || playerBody == null)
        {
            return;
        }

        Vector2 playerPosition = playerBody.position;
        playerBody.transform.position = playerPosition;
        gameplayCamera.transform.position = GetClampedCameraPosition(playerPosition, 0f);
        cameraFollowVelocity = Vector3.zero;
        previousCameraPlayerPosition = playerPosition;
    }

    private void FollowPlayerWithCamera()
    {
        if (gameplayCamera == null || playerBody == null)
        {
            return;
        }

        float deltaTime = Time.unscaledDeltaTime;
        if (deltaTime <= 0f)
        {
            return;
        }

        Vector2 playerPosition = playerBody.transform.position;
        Vector2 playerTravel = playerPosition - previousCameraPlayerPosition;
        float playerTravelDistance = playerTravel.magnitude;
        previousCameraPlayerPosition = playerPosition;

        float forwardTravelDistance = 0f;
        CourseRuntime currentCourse = GetCurrentCourse();
        if (!isCourseComplete && currentCourse != null &&
            nextCheckpointIndex < currentCourse.Checkpoints.Count)
        {
            Vector2 checkpointDirection =
                (Vector2)currentCourse.Checkpoints[nextCheckpointIndex].position - playerPosition;
            if (checkpointDirection.sqrMagnitude > Mathf.Epsilon)
            {
                forwardTravelDistance = Mathf.Max(
                    0f,
                    Vector2.Dot(playerTravel, checkpointDirection.normalized));
            }
        }

        float maximumPlayerTravel = playerMoveSpeed * deltaTime;
        float movementAmount = Mathf.Clamp01(
            playerTravelDistance / Mathf.Max(maximumPlayerTravel, Mathf.Epsilon));
        Vector3 targetPosition = GetClampedCameraPosition(playerPosition, movementAmount);
        Vector3 currentCameraPosition = gameplayCamera.transform.position;
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            currentCameraPosition,
            targetPosition,
            ref cameraFollowVelocity,
            cameraFollowSmoothTime,
            Mathf.Infinity,
            deltaTime);

        float halfHeight = gameplayCamera.orthographicSize;
        float halfWidth = halfHeight * gameplayCamera.aspect;
        Vector2 playerOffset = playerPosition - (Vector2)currentCameraPosition;
        float normalizedScreenOffset = Mathf.Max(
            Mathf.Abs(playerOffset.x) / Mathf.Max(halfWidth, Mathf.Epsilon),
            Mathf.Abs(playerOffset.y) / Mathf.Max(halfHeight, Mathf.Epsilon));
        float edgeFollowAmount = Mathf.SmoothStep(
            0f,
            1f,
            Mathf.InverseLerp(
                CameraEdgeFollowStart,
                CameraEdgeFollowEnd,
                normalizedScreenOffset));
        float cameraFollowRatio = Mathf.Lerp(
            MinimumCameraFollowRatio,
            1f,
            edgeFollowAmount);
        float forwardFollowRatio = CameraForwardFollowRatio * (1f - edgeFollowAmount);
        float maximumCameraTravel =
            (MinimumCameraFollowSpeed * deltaTime) +
            (playerTravelDistance * cameraFollowRatio) +
            (forwardTravelDistance * forwardFollowRatio);
        Vector3 cameraTravel = Vector3.ClampMagnitude(
            smoothedPosition - currentCameraPosition,
            maximumCameraTravel);
        gameplayCamera.transform.position = currentCameraPosition + cameraTravel;
    }

    private Vector3 GetClampedCameraPosition(Vector2 playerPosition, float movementAmount)
    {
        Vector2 followTarget = playerPosition;
        Vector2 lookAheadOffset = Vector2.zero;
        CourseRuntime currentCourse = GetCurrentCourse();
        if (!isCourseComplete && currentCourse != null &&
            nextCheckpointIndex < currentCourse.Checkpoints.Count)
        {
            Vector2 checkpointOffset =
                (Vector2)currentCourse.Checkpoints[nextCheckpointIndex].position - playerPosition;
            float verticalCheckpointDistance = Mathf.Abs(checkpointOffset.y);
            if (verticalCheckpointDistance > Mathf.Epsilon)
            {
                float courseProgress = currentCourse.Checkpoints.Count > 1
                    ? (float)nextCheckpointIndex / (currentCourse.Checkpoints.Count - 1)
                    : 0f;
                float progressLookAheadMultiplier = Mathf.Lerp(
                    1f,
                    MaximumCourseProgressLookAheadMultiplier,
                    courseProgress);
                float lookAheadSourceDistance = Mathf.Min(
                    verticalCheckpointDistance,
                    gameplayCamera.orthographicSize);
                lookAheadOffset = Vector2.up * Mathf.Sign(checkpointOffset.y) *
                    lookAheadSourceDistance *
                    cameraCheckpointLookAhead *
                    progressLookAheadMultiplier *
                    Mathf.Lerp(
                        MinimumCameraLookAheadRatio,
                        1f,
                        Mathf.Clamp01(movementAmount));
                followTarget += lookAheadOffset;
            }
        }

        float halfHeight = gameplayCamera.orthographicSize;
        float halfWidth = halfHeight * gameplayCamera.aspect;
        float fieldWidth = fieldMaximum.x - fieldMinimum.x;
        if (halfWidth * 2f >= fieldWidth)
        {
            followTarget.x = ((fieldMinimum.x + fieldMaximum.x) * 0.5f) + lookAheadOffset.x;
        }

        float cameraX = ClampCameraAxis(followTarget.x, fieldMinimum.x, fieldMaximum.x, halfWidth);
        float cameraY = ClampCameraAxis(followTarget.y, fieldMinimum.y, fieldMaximum.y, halfHeight);
        return new Vector3(cameraX, cameraY, -10f);
    }

    private static float ClampCameraAxis(float target, float minimum, float maximum, float cameraExtent)
    {
        float clampedMinimum = minimum + cameraExtent;
        float clampedMaximum = maximum - cameraExtent;

        if (clampedMinimum >= clampedMaximum)
        {
            return Mathf.Clamp(target, clampedMaximum, clampedMinimum);
        }

        return Mathf.Clamp(target, clampedMinimum, clampedMaximum);
    }

    private void CreateShapeSprites()
    {
        squareSprite = CreateSquareSprite();
        circleSprite = CreateCircleSprite();
    }

    private void CreateParticleMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Particles/Standard Unlit");
        }

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            return;
        }

        particleMaterial = new Material(shader)
        {
            name = "Dribbles Particle Material"
        };
        generatedAssets.Add(particleMaterial);
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            name = "Dribbles Square",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        sprite.name = "Dribbles Square";
        generatedAssets.Add(sprite);
        generatedAssets.Add(texture);
        return sprite;
    }

    private Sprite CreateCircleSprite()
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Dribbles Circle",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.48f;
        float feather = 1.25f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01((radius - distance) / feather);
                pixels[(y * size) + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            size);
        sprite.name = "Dribbles Circle";
        generatedAssets.Add(sprite);
        generatedAssets.Add(texture);
        return sprite;
    }

    private void CreateBoundaries()
    {
        float width = fieldMaximum.x - fieldMinimum.x;
        float height = fieldMaximum.y - fieldMinimum.y;
        Vector2 center = (fieldMinimum + fieldMaximum) * 0.5f;

        CreateWall("Bottom Boundary", new Vector2(center.x, fieldMinimum.y), new Vector2(width, 0.12f));
        CreateWall("Top Boundary", new Vector2(center.x, fieldMaximum.y), new Vector2(width, 0.12f));
        CreateWall("Left Boundary", new Vector2(fieldMinimum.x, center.y), new Vector2(0.12f, height));
        CreateWall("Right Boundary", new Vector2(fieldMaximum.x, center.y), new Vector2(0.12f, height));
    }

    private void CreateWall(string objectName, Vector2 position, Vector2 size)
    {
        SpriteRenderer renderer = CreateSpriteObject(
            objectName,
            position,
            size,
            FieldLineColor,
            squareSprite,
            -5,
            transform);

        BoxCollider2D collider = renderer.gameObject.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
    }

    private void CreateCourses()
    {
        if (courseRoots == null)
        {
            return;
        }

        for (int courseIndex = 0; courseIndex < courseRoots.Length; courseIndex++)
        {
            Transform courseRoot = courseRoots[courseIndex];
            if (courseRoot == null)
            {
                continue;
            }

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

                CheckpointVisual visual = new CheckpointVisual();
                course.Visuals.Add(visual);
                Transform nextCheckpoint = checkpointIndex + 1 < checkpointCount
                    ? courseRoot.GetChild(checkpointIndex + 1)
                    : null;
                CreateCheckpointVisual(checkpoint, nextCheckpoint, visual);
            }
        }
    }

    private void CreateCheckpointVisual(
        Transform checkpoint,
        Transform nextCheckpoint,
        CheckpointVisual visual)
    {
        Transform gateRoot = new GameObject("Runtime Marker").transform;
        gateRoot.SetParent(checkpoint, false);
        visual.Root = gateRoot.gameObject;

        CreateCheckpointPost(gateRoot, new Vector2(-checkpointHalfWidth, 0f), visual);
        CreateCheckpointPost(gateRoot, new Vector2(checkpointHalfWidth, 0f), visual);

        for (int dash = -2; dash <= 2; dash++)
        {
            SpriteRenderer renderer = CreateSpriteObject(
                "Gate Dash",
                new Vector2(dash * 0.38f, 0f),
                new Vector2(0.22f, 0.07f),
                ActiveCheckpointColor,
                squareSprite,
                2,
                gateRoot,
                true);
            visual.Renderers.Add(renderer);
        }

        CreateCheckpointArrow(gateRoot, nextCheckpoint, visual);
    }

    private void CreateCheckpointPost(Transform parent, Vector2 localPosition, CheckpointVisual visual)
    {
        SpriteRenderer renderer = CreateSpriteObject(
            "Checkpoint Post",
            localPosition,
            Vector2.one * 0.46f,
            ActiveCheckpointColor,
            circleSprite,
            3,
            parent,
            true);

        CircleCollider2D collider = renderer.gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        visual.Renderers.Add(renderer);
        visual.Colliders.Add(collider);
    }

    private void CreateCheckpointArrow(
        Transform parent,
        Transform nextCheckpoint,
        CheckpointVisual visual)
    {
        Transform arrowRoot = new GameObject("Direction Arrow").transform;
        arrowRoot.SetParent(parent, false);

        if (nextCheckpoint != null)
        {
            Vector2 localCheckpointDirection = parent.InverseTransformDirection(
                nextCheckpoint.position - parent.position);
            if (localCheckpointDirection.sqrMagnitude > Mathf.Epsilon)
            {
                Vector2 arrowDirection = Vector2.Lerp(
                    Vector2.up,
                    localCheckpointDirection.normalized,
                    CheckpointArrowDirectionInfluence).normalized;
                float arrowAngle = Mathf.Clamp(
                    Vector2.SignedAngle(Vector2.up, arrowDirection),
                    -MaximumCheckpointArrowAngle,
                    MaximumCheckpointArrowAngle);
                arrowRoot.localRotation = Quaternion.Euler(0f, 0f, arrowAngle);
            }
        }

        SpriteRenderer stem = CreateSpriteObject(
            "Direction Arrow Stem",
            new Vector2(0f, 0.43f),
            new Vector2(0.075f, 0.28f),
            ActiveCheckpointColor,
            squareSprite,
            2,
            arrowRoot,
            true);
        visual.Renderers.Add(stem);

        SpriteRenderer leftHead = CreateSpriteObject(
            "Direction Arrow Left",
            new Vector2(-0.085f, 0.57f),
            new Vector2(0.075f, 0.23f),
            ActiveCheckpointColor,
            squareSprite,
            2,
            arrowRoot,
            true);
        leftHead.transform.localRotation = Quaternion.Euler(0f, 0f, -45f);
        visual.Renderers.Add(leftHead);

        SpriteRenderer rightHead = CreateSpriteObject(
            "Direction Arrow Right",
            new Vector2(0.085f, 0.57f),
            new Vector2(0.075f, 0.23f),
            ActiveCheckpointColor,
            squareSprite,
            2,
            arrowRoot,
            true);
        rightHead.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        visual.Renderers.Add(rightHead);
    }

    private void CreatePlayer()
    {
        SpriteRenderer renderer = CreateSpriteObject(
            "Player",
            playerStartPosition,
            Vector2.one * (PlayerRadius * 2f),
            PlayerColor,
            circleSprite,
            10,
            transform);

        CircleCollider2D collider = renderer.gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        playerBody = renderer.gameObject.AddComponent<Rigidbody2D>();
        playerBody.bodyType = RigidbodyType2D.Kinematic;
        playerBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        playerBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void CreateBall()
    {
        GameObject ballObject = new GameObject("Ball");
        ballObject.transform.SetParent(transform, false);
        ballObject.transform.position = ballStartPosition;

        GameObject visualObject = new GameObject("Existing Ball Sprite");
        visualObject.transform.SetParent(ballObject.transform, false);
        SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ballSprite != null ? ballSprite : circleSprite;
        renderer.color = ballSprite != null ? Color.white : new Color32(32, 32, 32, 255);
        renderer.sortingOrder = 11;

        float spriteSize = Mathf.Max(renderer.sprite.bounds.size.x, renderer.sprite.bounds.size.y);
        float scale = (BallRadius * 2f) / Mathf.Max(spriteSize, 0.001f);
        visualObject.transform.localScale = Vector3.one * scale;
        visualObject.transform.localPosition = -renderer.sprite.bounds.center * scale;

        CircleCollider2D collider = ballObject.AddComponent<CircleCollider2D>();
        collider.radius = BallRadius;

        PhysicsMaterial2D material = new PhysicsMaterial2D("Dribbles Ball Material")
        {
            bounciness = 0f,
            friction = 0.32f
        };
        generatedAssets.Add(material);
        collider.sharedMaterial = material;

        ballBody = ballObject.AddComponent<Rigidbody2D>();
        ballBody.mass = ballMass;
        ballBody.gravityScale = 0f;
        ballBody.linearDamping = ballLinearDamping;
        ballBody.angularDamping = 1.6f;
        ballBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        ballBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private SpriteRenderer CreateSpriteObject(
        string objectName,
        Vector2 position,
        Vector2 size,
        Color color,
        Sprite sprite,
        int sortingOrder,
        Transform parent,
        bool useLocalPosition = false)
    {
        GameObject gameObject = new GameObject(objectName);
        gameObject.transform.SetParent(parent, false);

        if (useLocalPosition)
        {
            gameObject.transform.localPosition = position;
        }
        else
        {
            gameObject.transform.position = position;
        }

        gameObject.transform.localScale = new Vector3(size.x, size.y, 1f);
        SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private void HandleMouseInput()
    {
        if (isCourseComplete || gameplayCamera == null)
        {
            return;
        }

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
                    PlayCheckpointBurst(checkpoint.position, isFinalCheckpoint);
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

    private void PlayCheckpointBurst(Vector2 position, bool isFinalCheckpoint)
    {
        GameObject effectObject = new GameObject(
            isFinalCheckpoint ? "Final Checkpoint Burst" : "Checkpoint Burst");
        effectObject.transform.SetParent(transform, false);
        effectObject.transform.position = position;
        checkpointEffects.Add(effectObject);

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = isFinalCheckpoint ? 1.1f : 0.65f;
        main.startLifetime = isFinalCheckpoint
            ? new ParticleSystem.MinMaxCurve(0.7f, 1.15f)
            : new ParticleSystem.MinMaxCurve(0.35f, 0.65f);
        main.startSpeed = isFinalCheckpoint
            ? new ParticleSystem.MinMaxCurve(2.5f, 5.25f)
            : new ParticleSystem.MinMaxCurve(1.4f, 3f);
        main.startSize = isFinalCheckpoint
            ? new ParticleSystem.MinMaxCurve(0.12f, 0.28f)
            : new ParticleSystem.MinMaxCurve(0.08f, 0.17f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor = isFinalCheckpoint
            ? new ParticleSystem.MinMaxGradient(ActiveCheckpointColor, Color.white)
            : new ParticleSystem.MinMaxGradient(ActiveCheckpointColor, new Color32(255, 132, 32, 255));
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = isFinalCheckpoint ? 64 : 24;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = isFinalCheckpoint ? 0.45f : 0.22f;
        shape.radiusThickness = 1f;

        ParticleSystemRenderer particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.sortingOrder = 30;
        if (particleMaterial != null)
        {
            particleRenderer.sharedMaterial = particleMaterial;
        }

        particles.Emit(isFinalCheckpoint ? 48 : 16);
        particles.Play();
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
                CheckpointVisual visual = course.Visuals[checkpointIndex];
                bool isActiveCheckpoint = isCurrentCourse &&
                    !isCourseComplete && checkpointIndex == nextCheckpointIndex;
                if (visual.Root != null)
                {
                    visual.Root.SetActive(true);
                }

                Color color = isActiveCheckpoint
                    ? ActiveCheckpointColor
                    : InactiveCheckpointColor;
                SetCheckpointColor(visual, color);

                for (int colliderIndex = 0; colliderIndex < visual.Colliders.Count; colliderIndex++)
                {
                    visual.Colliders[colliderIndex].enabled = isActiveCheckpoint;
                }
            }
        }
    }

    private void PulseActiveCheckpoint()
    {
        CourseRuntime currentCourse = GetCurrentCourse();
        if (isCourseComplete || currentCourse == null ||
            nextCheckpointIndex >= currentCourse.Visuals.Count)
        {
            return;
        }

        float pulse = (Mathf.Sin(Time.unscaledTime * 5f) + 1f) * 0.08f;
        Color pulseColor = Color.Lerp(ActiveCheckpointColor, Color.white, pulse);
        SetCheckpointColor(currentCourse.Visuals[nextCheckpointIndex], pulseColor);
    }

    private static void SetCheckpointColor(CheckpointVisual checkpoint, Color color)
    {
        for (int i = 0; i < checkpoint.Renderers.Count; i++)
        {
            checkpoint.Renderers[i].color = color;
        }
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
        {
            return;
        }

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

        ClearCheckpointEffects();
        RefreshCourseState();
        SnapCameraToPlayer();

        CourseRuntime currentCourse = GetCurrentCourse();
        if (currentCourse == null || currentCourse.Checkpoints.Count == 0)
        {
            CompleteCourse();
        }
    }

    private void ClearCheckpointEffects()
    {
        for (int i = 0; i < checkpointEffects.Count; i++)
        {
            if (checkpointEffects[i] != null)
            {
                checkpointEffects[i].SetActive(false);
                Destroy(checkpointEffects[i]);
            }
        }

        checkpointEffects.Clear();
    }

    private CourseRuntime GetCurrentCourse()
    {
        if (currentCourseIndex < 0 || currentCourseIndex >= courses.Count)
        {
            return null;
        }

        return courses[currentCourseIndex];
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
        {
            return CourseRating.A;
        }

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
            {
                continue;
            }

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

    private static string FormatScoreboardValue(string label, string value)
    {
        return $"<color=#FFD23A>{label}:</color> <color=#FFFFFF>{value}</color>";
    }

    private int GetRequiredCourseCount()
    {
        return courses.Count == 0
            ? 0
            : Mathf.Clamp(requiredCourseCount, 1, courses.Count);
    }

    private static string FormatTime(float totalSeconds)
    {
        return $"{Mathf.Max(0f, totalSeconds):00.0}";
    }
}
