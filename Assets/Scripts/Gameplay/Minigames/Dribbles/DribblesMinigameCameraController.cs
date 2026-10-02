using UnityEngine;

[DisallowMultipleComponent]
public sealed class DribblesMinigameCameraController : MonoBehaviour
{
    private const float MaximumCourseProgressLookAheadMultiplier = 1.1f;
    private const float MinimumCameraLookAheadRatio = 0.75f;
    private const float MinimumCameraFollowRatio = 0.25f;
    private const float CameraForwardFollowRatio = 0.55f;
    private const float MinimumCameraFollowSpeed = 0.75f;
    private const float CameraEdgeFollowStart = 0.45f;
    private const float CameraEdgeFollowEnd = 0.78f;

    [SerializeField] private Camera gameplayCamera;
    [SerializeField, Min(1f)] private float orthographicSize = 6f;
    [SerializeField, Min(0.01f)] private float followSmoothTime = 0.28f;
    [SerializeField, Range(0f, 0.5f)] private float checkpointLookAhead = 0.35f;

    private Rigidbody2D playerBody;
    private Vector2 fieldMinimum;
    private Vector2 fieldMaximum;
    private float playerMoveSpeed;
    private Vector3 followVelocity;
    private Vector2 previousPlayerPosition;

    public Camera GameplayCamera => gameplayCamera;

    public void Initialize(
        Rigidbody2D player,
        Vector2 minimum,
        Vector2 maximum,
        float moveSpeed)
    {
        playerBody = player;
        fieldMinimum = minimum;
        fieldMaximum = maximum;
        playerMoveSpeed = moveSpeed;

        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (gameplayCamera == null)
        {
            Debug.LogError("Dribbles minigame requires a scene camera tagged MainCamera.", this);
            enabled = false;
            return;
        }

        gameplayCamera.orthographic = true;
        gameplayCamera.orthographicSize = orthographicSize;
        previousPlayerPosition = playerBody.position;
    }

    public void Follow(
        Transform nextCheckpoint,
        int nextCheckpointIndex,
        int checkpointCount)
    {
        if (!enabled || gameplayCamera == null || playerBody == null)
            return;

        float deltaTime = Time.unscaledDeltaTime;
        if (deltaTime <= 0f)
            return;

        Vector2 playerPosition = playerBody.transform.position;
        Vector2 playerTravel = playerPosition - previousPlayerPosition;
        float playerTravelDistance = playerTravel.magnitude;
        previousPlayerPosition = playerPosition;

        float forwardTravelDistance = 0f;
        if (nextCheckpoint != null)
        {
            Vector2 checkpointDirection = (Vector2)nextCheckpoint.position - playerPosition;
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
        Vector3 targetPosition = GetCameraPosition(
            playerPosition,
            nextCheckpoint,
            nextCheckpointIndex,
            checkpointCount,
            movementAmount);
        Vector3 currentCameraPosition = gameplayCamera.transform.position;
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            currentCameraPosition,
            targetPosition,
            ref followVelocity,
            followSmoothTime,
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

    public void Snap(
        Transform nextCheckpoint,
        int nextCheckpointIndex,
        int checkpointCount)
    {
        if (!enabled || gameplayCamera == null || playerBody == null)
            return;

        Vector2 playerPosition = playerBody.position;
        playerBody.transform.position = playerPosition;
        gameplayCamera.transform.position = GetCameraPosition(
            playerPosition,
            nextCheckpoint,
            nextCheckpointIndex,
            checkpointCount,
            0f);
        followVelocity = Vector3.zero;
        previousPlayerPosition = playerPosition;
    }

    private Vector3 GetCameraPosition(
        Vector2 playerPosition,
        Transform nextCheckpoint,
        int nextCheckpointIndex,
        int checkpointCount,
        float movementAmount)
    {
        Vector2 followTarget = playerPosition;
        Vector2 lookAheadOffset = Vector2.zero;
        if (nextCheckpoint != null)
        {
            Vector2 checkpointOffset = (Vector2)nextCheckpoint.position - playerPosition;
            float verticalCheckpointDistance = Mathf.Abs(checkpointOffset.y);
            if (verticalCheckpointDistance > Mathf.Epsilon)
            {
                float courseProgress = checkpointCount > 1
                    ? (float)nextCheckpointIndex / (checkpointCount - 1)
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
                    checkpointLookAhead *
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
            followTarget.x = (fieldMinimum.x + fieldMaximum.x) * 0.5f;

        float cameraX = ClampCameraAxis(followTarget.x, fieldMinimum.x, fieldMaximum.x, halfWidth);
        float cameraY = ClampCameraAxis(followTarget.y, fieldMinimum.y, fieldMaximum.y, halfHeight);
        return new Vector3(cameraX, cameraY, -10f);
    }

    private static float ClampCameraAxis(
        float target,
        float minimum,
        float maximum,
        float cameraExtent)
    {
        float clampedMinimum = minimum + cameraExtent;
        float clampedMaximum = maximum - cameraExtent;

        if (clampedMinimum >= clampedMaximum)
            return Mathf.Clamp(target, clampedMaximum, clampedMinimum);

        return Mathf.Clamp(target, clampedMinimum, clampedMaximum);
    }
}
