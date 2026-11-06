using Gameplay.CharacterComponents;
using Gameplay.Managers;
using UnityEngine;

public class TutorialLowBallHop : MonoBehaviour
{
    [Tooltip("Player's upward hop force near low balls during goal practice only.")]
    [SerializeField, Range(0.1f, 1f)] float _hopMultiplier = 0.5f;

    PlayerActions _actions;
    Rigidbody2D _body;

    public void BeginPractice(PlayerActions actions)
    {
        EndPractice();
        if (!isActiveAndEnabled)
            return;
        _actions = actions;
        _body = actions != null ? actions.GetComponent<Rigidbody2D>() : null;
        if (_actions != null)
            _actions.ModifyJumpForce += ApplyLowBallHop;
    }

    void ApplyLowBallHop(ref Vector2 jumpForce)
    {
        if (_hopMultiplier >= 1f || _actions == null || _body == null)
            return;

        BallScript ball = BallManager.Instance != null ? BallManager.Instance.Ball : null;
        Collider2D kickingCollider = _actions.KickingContactCollider;
        if (ball == null || ball.Collider == null || kickingCollider == null ||
            ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        float predictionTime = _actions.Data?.KickAssist?.BallPredictionTime ?? 0.35f;
        float nearRange = _actions.Data?.KickAssist?.MovementAssistRange ?? 3f;
        Vector2 predictedPosition = ball.Rigidbody.position + ball.Rigidbody.linearVelocity * predictionTime;
        predictedPosition.y += 0.5f * Physics2D.gravity.y * ball.Rigidbody.gravityScale *
                               predictionTime * predictionTime;
        float lowBallHeight = kickingCollider.bounds.max.y + 0.25f;
        bool staysLow = ball.Collider.bounds.max.y <= lowBallHeight &&
                        predictedPosition.y + ball.Collider.bounds.extents.y <= lowBallHeight;
        if (staysLow && Mathf.Abs(predictedPosition.x - _body.position.x) <= nearRange)
            jumpForce.y *= Mathf.Clamp01(_hopMultiplier);
    }

    public void EndPractice()
    {
        if (_actions != null)
            _actions.ModifyJumpForce -= ApplyLowBallHop;
        _actions = null;
        _body = null;
    }

    void OnDisable() => EndPractice();
}
