using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class DribblesMinigameController : MonoBehaviour
{
    private const float PlayerRadius = 0.52f;
    private const float BallRadius = 0.42f;

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
    [Tooltip("Courses at the start of the list that must be completed before the minigame can finish.")]
    [SerializeField, Min(1)] private int requiredCourseCount = 2;
    [Tooltip("Invoked when the player chooses Finish Minigame after completing every required course.")]
    [SerializeField] private UnityEvent onMinigameCompleted;
    [Tooltip("Per-course time limits in seconds. Times at or below A earn A; times at or below B earn B.")]
    [SerializeField] private CourseRatingThresholds[] courseRatingThresholds;
    [SerializeField, Min(0.5f)] private float checkpointHalfWidth = 1.3f;
    [SerializeField, Min(0.25f)] private float checkpointCrossingHalfWidth = 1.05f;

    [Header("Player")]
    [SerializeField, Min(1f)] private float playerMoveSpeed = 18f;

    [Header("Ball")]
    [SerializeField] private Sprite ballSprite;
    [SerializeField, Min(0.1f)] private float ballMass = 1.35f;
    [SerializeField, Min(0f)] private float ballLinearDamping = 2.15f;
    [SerializeField, Min(0.1f)] private float ballMaximumSpeed = 7f;

    [Header("Camera")]
    [SerializeField, Min(1f)] private float cameraOrthographicSize = 6f;
    [SerializeField, Min(0.01f)] private float cameraFollowSmoothTime = 0.28f;
    [SerializeField, Range(0f, 0.5f)] private float cameraCheckpointLookAhead = 0.35f;

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
    private bool isCourseComplete;
    private bool isMinigameFinished;
    private int currentCourseIndex;
    private int nextCheckpointIndex;
    private float startTime;
    private float completionTime;
    private Vector3 cameraFollowVelocity;
    private GUIStyle timerStyle;
    private GUIStyle completionStyle;
    private GUIStyle buttonStyle;

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
        [Min(1f)] public float aTime = 45f;
        [Min(1f)] public float bTime = 75f;
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
            Vector2 nextPosition = Vector2.MoveTowards(
                playerBody.position,
                dragTarget,
                playerMoveSpeed * Time.fixedDeltaTime);

            playerBody.MovePosition(nextPosition);
        }

        LimitBallSpeed();
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

    private void OnGUI()
    {
        if (timerStyle == null)
        {
            timerStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            timerStyle.normal.textColor = Color.white;

            completionStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            completionStyle.normal.textColor = Color.white;

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold
            };
        }

        timerStyle.fontSize = Mathf.Clamp(Screen.height / 30, 22, 36);
        completionStyle.fontSize = Mathf.Clamp(Screen.height / 38, 18, 28);
        buttonStyle.fontSize = Mathf.Clamp(Screen.height / 50, 16, 24);

        float elapsed = isCourseComplete ? completionTime : Time.unscaledTime - startTime;
        string courseLabel = courses.Count > 0
            ? $"Course {currentCourseIndex + 1} / {courses.Count}"
            : "No Courses Configured";
        string prefix = isCourseComplete ? "Complete!  " : "Time  ";
        string text = isMinigameFinished
            ? "Minigame Complete!"
            : courseLabel + "    " + prefix + FormatTime(elapsed);
        float timerWidth = Mathf.Min(680f, Screen.width - 32f);
        Rect timerRect = new Rect((Screen.width - timerWidth) * 0.5f, 16f, timerWidth, 84f);
        GUI.Label(timerRect, text, timerStyle);

        if (isMinigameFinished)
        {
            DrawFinishedPanel();
            return;
        }

        if (!isCourseComplete || courses.Count == 0)
        {
            return;
        }

        float panelWidth = Mathf.Min(680f, Screen.width - 40f);
        bool hasNextRequiredCourse = currentCourseIndex + 1 < GetRequiredCourseCount();
        bool completedLastRequiredCourse = currentCourseIndex == GetRequiredCourseCount() - 1;
        bool isOptionalCourse = currentCourseIndex >= GetRequiredCourseCount();
        bool hasNextOptionalCourse = isOptionalCourse && currentCourseIndex < courses.Count - 1;
        bool canPlayOptionalCourse = completedLastRequiredCourse && currentCourseIndex < courses.Count - 1;
        float panelHeight = canPlayOptionalCourse || hasNextOptionalCourse ? 330f : 250f;
        Rect panelRect = new Rect(
            (Screen.width - panelWidth) * 0.5f,
            (Screen.height - panelHeight) * 0.5f,
            panelWidth,
            panelHeight);
        GUI.Box(panelRect, GUIContent.none);

        CourseRuntime completedCourse = GetCurrentCourse();
        string ratingText = completedCourse != null
            ? GetRatingText(completedCourse.Rating)
            : string.Empty;
        string completionMessage;
        if (canPlayOptionalCourse)
        {
            completionMessage =
                $"Course {currentCourseIndex + 1}: {FormatTime(completionTime)}\n" +
                ratingText + "\n" +
                "All required courses are complete.\n" +
                $"Finish now or play optional Course {currentCourseIndex + 2}.";
        }
        else if (isOptionalCourse)
        {
            completionMessage =
                $"Optional Course {currentCourseIndex + 1}: {FormatTime(completionTime)}\n{ratingText}";
        }
        else
        {
            completionMessage =
                $"Course {currentCourseIndex + 1}: {FormatTime(completionTime)}\n{ratingText}";
        }

        GUI.Label(
            new Rect(panelRect.x + 24f, panelRect.y + 18f, panelRect.width - 48f, 140f),
            completionMessage,
            completionStyle);

        float buttonWidth = (panelRect.width - 63f) * 0.5f;
        bool showOptionalButton = canPlayOptionalCourse || hasNextOptionalCourse;
        float primaryButtonY = panelRect.yMax - (showOptionalButton ? 142f : 76f);
        Rect retryRect = new Rect(panelRect.x + 24f, primaryButtonY, buttonWidth, 54f);
        Rect continueRect = new Rect(retryRect.xMax + 15f, retryRect.y, buttonWidth, 54f);

        if (GUI.Button(retryRect, "Retry Course", buttonStyle))
        {
            RestartCurrentCourse();
        }

        if (hasNextRequiredCourse)
        {
            if (GUI.Button(continueRect, "Next Course", buttonStyle))
            {
                StartCourse(currentCourseIndex + 1);
            }
        }
        else if (completedLastRequiredCourse || isOptionalCourse)
        {
            if (GUI.Button(continueRect, "Finish Minigame", buttonStyle))
            {
                FinishMinigame();
            }
        }

        if (showOptionalButton)
        {
            Rect optionalRect = new Rect(
                panelRect.x + 24f,
                panelRect.yMax - 76f,
                panelRect.width - 48f,
                54f);
            if (GUI.Button(optionalRect, "Play Optional Course", buttonStyle))
            {
                StartCourse(currentCourseIndex + 1);
            }
        }
    }

    private void DrawFinishedPanel()
    {
        float panelWidth = Mathf.Min(680f, Screen.width - 40f);
        const float panelHeight = 250f;
        Rect panelRect = new Rect(
            (Screen.width - panelWidth) * 0.5f,
            (Screen.height - panelHeight) * 0.5f,
            panelWidth,
            panelHeight);
        GUI.Box(panelRect, GUIContent.none);

        GUI.Label(
            new Rect(panelRect.x + 24f, panelRect.y + 18f, panelRect.width - 48f, 140f),
            BuildRatingSummary(),
            completionStyle);

        bool hasOptionalCourse = courses.Count > GetRequiredCourseCount();
        float buttonWidth = hasOptionalCourse
            ? (panelRect.width - 63f) * 0.5f
            : panelRect.width - 48f;
        Rect replayRect = new Rect(panelRect.x + 24f, panelRect.yMax - 76f, buttonWidth, 54f);
        if (GUI.Button(replayRect, "Replay Required Courses", buttonStyle))
        {
            StartCourse(0);
        }

        if (hasOptionalCourse)
        {
            Rect optionalRect = new Rect(replayRect.xMax + 15f, replayRect.y, buttonWidth, 54f);
            if (GUI.Button(optionalRect, "Play Optional Course", buttonStyle))
            {
                StartCourse(GetRequiredCourseCount());
            }
        }
    }

    private void OnDestroy()
    {
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

        gameplayCamera.transform.position = GetClampedCameraPosition(playerBody.position);
        cameraFollowVelocity = Vector3.zero;
    }

    private void FollowPlayerWithCamera()
    {
        if (gameplayCamera == null || playerBody == null)
        {
            return;
        }

        Vector3 targetPosition = GetClampedCameraPosition(playerBody.position);
        gameplayCamera.transform.position = Vector3.SmoothDamp(
            gameplayCamera.transform.position,
            targetPosition,
            ref cameraFollowVelocity,
            cameraFollowSmoothTime,
            Mathf.Infinity,
            Time.unscaledDeltaTime);
    }

    private Vector3 GetClampedCameraPosition(Vector2 playerPosition)
    {
        Vector2 followTarget = playerPosition;
        CourseRuntime currentCourse = GetCurrentCourse();
        if (!isCourseComplete && currentCourse != null &&
            nextCheckpointIndex < currentCourse.Checkpoints.Count)
        {
            followTarget = Vector2.Lerp(
                playerPosition,
                currentCourse.Checkpoints[nextCheckpointIndex].position,
                cameraCheckpointLookAhead);
        }

        float halfHeight = gameplayCamera.orthographicSize;
        float halfWidth = halfHeight * gameplayCamera.aspect;
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
            return (minimum + maximum) * 0.5f;
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
                CreateCheckpointVisual(checkpoint, visual);
            }
        }
    }

    private void CreateCheckpointVisual(Transform checkpoint, CheckpointVisual visual)
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

        CreateCheckpointArrow(gateRoot, visual);
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

    private void CreateCheckpointArrow(Transform parent, CheckpointVisual visual)
    {
        SpriteRenderer stem = CreateSpriteObject(
            "Direction Arrow Stem",
            new Vector2(0f, 0.43f),
            new Vector2(0.075f, 0.28f),
            ActiveCheckpointColor,
            squareSprite,
            2,
            parent,
            true);
        visual.Renderers.Add(stem);

        SpriteRenderer leftHead = CreateSpriteObject(
            "Direction Arrow Left",
            new Vector2(-0.085f, 0.57f),
            new Vector2(0.075f, 0.23f),
            ActiveCheckpointColor,
            squareSprite,
            2,
            parent,
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
            parent,
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
        completionTime = Time.unscaledTime - startTime;

        CourseRuntime completedCourse = GetCurrentCourse();
        if (completedCourse != null)
        {
            completedCourse.HasCompletion = true;
            completedCourse.CompletionTime = completionTime;
            completedCourse.Rating = CalculateCourseRating(currentCourseIndex, completionTime);
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
        onMinigameCompleted?.Invoke();
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
        startTime = Time.unscaledTime;

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

    private CourseRating CalculateCourseRating(int courseIndex, float courseTime)
    {
        float aTime = 45f;
        float bTime = 75f;
        if (courseRatingThresholds != null && courseIndex >= 0 &&
            courseIndex < courseRatingThresholds.Length && courseRatingThresholds[courseIndex] != null)
        {
            CourseRatingThresholds thresholds = courseRatingThresholds[courseIndex];
            aTime = Mathf.Max(1f, thresholds.aTime);
            bTime = Mathf.Max(aTime, thresholds.bTime);
        }

        if (courseTime <= aTime)
        {
            return CourseRating.A;
        }

        return courseTime <= bTime ? CourseRating.B : CourseRating.C;
    }

    private static string GetRatingText(CourseRating rating)
    {
        switch (rating)
        {
            case CourseRating.A:
                return "Rating A - Great!";
            case CourseRating.B:
                return "Rating B - Good!";
            case CourseRating.C:
                return "Rating C - Complete";
            default:
                return "Not completed";
        }
    }

    private string BuildRatingSummary()
    {
        string summary = $"All {GetRequiredCourseCount()} required courses are complete.";
        for (int courseIndex = 0; courseIndex < courses.Count; courseIndex++)
        {
            CourseRuntime course = courses[courseIndex];
            if (!course.HasCompletion)
            {
                continue;
            }

            string optionalLabel = courseIndex >= GetRequiredCourseCount() ? " (Optional)" : string.Empty;
            summary +=
                $"\nCourse {courseIndex + 1}{optionalLabel}: {GetRatingText(course.Rating)} " +
                $"- {FormatTime(course.CompletionTime)}";
        }

        return summary;
    }

    private int GetRequiredCourseCount()
    {
        return courses.Count == 0
            ? 0
            : Mathf.Clamp(requiredCourseCount, 1, courses.Count);
    }

    private static string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        float seconds = totalSeconds - (minutes * 60f);
        return $"{minutes:00}:{seconds:00.0}";
    }
}
