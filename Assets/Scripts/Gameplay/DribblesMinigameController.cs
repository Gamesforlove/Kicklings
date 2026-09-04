using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("Scene transforms crossed by the ball in array order. Keep them as children of this object.")]
    [SerializeField] private Transform[] checkpoints;
    [SerializeField, Min(0.5f)] private float checkpointHalfWidth = 1.3f;
    [SerializeField, Min(0.25f)] private float checkpointCrossingHalfWidth = 1.05f;

    [Header("Player")]
    [SerializeField, Min(1f)] private float playerMoveSpeed = 11f;

    [Header("Ball")]
    [SerializeField] private Sprite ballSprite;
    [SerializeField, Min(0.1f)] private float ballMass = 1.1f;
    [SerializeField, Min(0f)] private float ballLinearDamping = 2.15f;

    [Header("Camera")]
    [SerializeField, Min(1f)] private float cameraOrthographicSize = 6f;
    [SerializeField, Min(0.01f)] private float cameraFollowSmoothTime = 0.28f;
    [SerializeField, Range(0f, 0.5f)] private float cameraCheckpointLookAhead = 0.35f;

    private readonly List<CheckpointVisual> checkpointVisuals = new List<CheckpointVisual>();
    private readonly List<Object> generatedAssets = new List<Object>();

    private Camera gameplayCamera;
    private Sprite squareSprite;
    private Sprite circleSprite;
    private Material particleMaterial;
    private Rigidbody2D playerBody;
    private Rigidbody2D ballBody;
    private Vector2 dragTarget;
    private Vector3 previousPointerScreenPosition;
    private Vector2 previousBallPosition;
    private bool isDragging;
    private bool isComplete;
    private int nextCheckpointIndex;
    private float startTime;
    private float completionTime;
    private Vector3 cameraFollowVelocity;
    private GUIStyle timerStyle;

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
        CreateCheckpoints();
        CreatePlayer();
        CreateBall();
        SnapCameraToPlayer();

        previousBallPosition = ballBody.position;
        startTime = Time.unscaledTime;
        RefreshCheckpointColors();
    }

    private void Update()
    {
        HandleMouseInput();
        PulseActiveCheckpoint();
    }

    private void LateUpdate()
    {
        FollowPlayerWithCamera();
    }

    private void FixedUpdate()
    {
        if (isComplete)
        {
            return;
        }

        if (isDragging)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                playerBody.position,
                dragTarget,
                playerMoveSpeed * Time.fixedDeltaTime);

            playerBody.MovePosition(nextPosition);
        }

        CheckForCheckpointCrossing();
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
        }

        timerStyle.fontSize = Mathf.Clamp(Screen.height / 30, 22, 36);

        float elapsed = isComplete ? completionTime : Time.unscaledTime - startTime;
        string prefix = isComplete ? "Complete!  " : "Time  ";
        string text = prefix + FormatTime(elapsed);
        Rect timerRect = new Rect((Screen.width - 480f) * 0.5f, 16f, 480f, 72f);
        GUI.Label(timerRect, text, timerStyle);
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
        if (checkpoints != null)
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                if (checkpoints[i] == null)
                {
                    continue;
                }

                Transform checkpoint = checkpoints[i];
                Vector3 leftPost = checkpoint.TransformPoint(Vector3.left * checkpointHalfWidth);
                Vector3 rightPost = checkpoint.TransformPoint(Vector3.right * checkpointHalfWidth);
                Gizmos.DrawWireSphere(leftPost, 0.23f);
                Gizmos.DrawWireSphere(rightPost, 0.23f);
                Gizmos.DrawLine(leftPost, rightPost);
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
        if (!isComplete && checkpoints != null && nextCheckpointIndex < checkpoints.Length &&
            checkpoints[nextCheckpointIndex] != null)
        {
            followTarget = Vector2.Lerp(
                playerPosition,
                checkpoints[nextCheckpointIndex].position,
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

    private void CreateCheckpoints()
    {
        if (checkpoints == null)
        {
            return;
        }

        for (int i = 0; i < checkpoints.Length; i++)
        {
            CheckpointVisual visual = new CheckpointVisual();
            checkpointVisuals.Add(visual);

            if (checkpoints[i] == null)
            {
                continue;
            }

            Transform gateRoot = new GameObject("Runtime Marker").transform;
            gateRoot.SetParent(checkpoints[i], false);
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
            bounciness = 0.05f,
            friction = 0.42f
        };
        generatedAssets.Add(material);
        collider.sharedMaterial = material;

        ballBody = ballObject.AddComponent<Rigidbody2D>();
        ballBody.mass = ballMass;
        ballBody.gravityScale = 0f;
        ballBody.linearDamping = ballLinearDamping;
        ballBody.angularDamping = 1.1f;
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
        if (isComplete || gameplayCamera == null)
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
                isDragging = true;
                dragTarget = playerBody.position;
                previousPointerScreenPosition = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentPointerScreenPosition = Input.mousePosition;
            Vector3 pointerDelta = currentPointerScreenPosition - previousPointerScreenPosition;
            float worldUnitsPerPixelX = (gameplayCamera.orthographicSize * 2f * gameplayCamera.aspect) / Screen.width;
            float worldUnitsPerPixelY = (gameplayCamera.orthographicSize * 2f) / Screen.height;
            Vector2 desiredPosition = dragTarget + new Vector2(
                pointerDelta.x * worldUnitsPerPixelX,
                pointerDelta.y * worldUnitsPerPixelY);

            dragTarget = new Vector2(
                Mathf.Clamp(desiredPosition.x, fieldMinimum.x + PlayerRadius, fieldMaximum.x - PlayerRadius),
                Mathf.Clamp(desiredPosition.y, fieldMinimum.y + PlayerRadius, fieldMaximum.y - PlayerRadius));
            previousPointerScreenPosition = currentPointerScreenPosition;
        }
    }

    private void CheckForCheckpointCrossing()
    {
        Vector2 currentBallPosition = ballBody.position;
        if (checkpoints != null && nextCheckpointIndex < checkpoints.Length &&
            checkpoints[nextCheckpointIndex] != null)
        {
            Transform checkpoint = checkpoints[nextCheckpointIndex];
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
                    bool isFinalCheckpoint = nextCheckpointIndex == checkpoints.Length - 1;
                    PlayCheckpointBurst(checkpoint.position, isFinalCheckpoint);
                    nextCheckpointIndex++;
                    RefreshCheckpointColors();

                    if (nextCheckpointIndex >= checkpoints.Length)
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

    private void RefreshCheckpointColors()
    {
        for (int i = 0; i < checkpointVisuals.Count; i++)
        {
            CheckpointVisual visual = checkpointVisuals[i];
            bool isActiveCheckpoint = !isComplete && i == nextCheckpointIndex;
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

    private void PulseActiveCheckpoint()
    {
        if (isComplete || nextCheckpointIndex >= checkpointVisuals.Count)
        {
            return;
        }

        float pulse = (Mathf.Sin(Time.unscaledTime * 5f) + 1f) * 0.08f;
        Color pulseColor = Color.Lerp(ActiveCheckpointColor, Color.white, pulse);
        SetCheckpointColor(checkpointVisuals[nextCheckpointIndex], pulseColor);
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
        isComplete = true;
        isDragging = false;
        completionTime = Time.unscaledTime - startTime;

        playerBody.linearVelocity = Vector2.zero;
        ballBody.linearVelocity = Vector2.zero;
        ballBody.angularVelocity = 0f;
        ballBody.bodyType = RigidbodyType2D.Kinematic;
        RefreshCheckpointColors();
    }

    private static string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        float seconds = totalSeconds - (minutes * 60f);
        return $"{minutes:00}:{seconds:00.0}";
    }
}
