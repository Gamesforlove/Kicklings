using System.Collections;
using UnityEngine;

public class MoveToPointCurved : MonoBehaviour
{
    public Transform obj;
    public Transform startPoint;
    public Transform midPoint;
    public Transform endPoint;
    public float duration = 1f;
    [Header("Tutorial Kick Flight")]
    [SerializeField, Min(0.1f)] float _kickFlightDuration = 2f;
    [SerializeField, Min(0f)] float _kickFlightLift = 0.75f;
    Coroutine _moveCoroutine;
    Coroutine _kickFlightCoroutine;
    BallScript _kickFlightBall;
    float _kickFlightGravity;
    float _kickFlightElapsed;
    float _activeKickFlightDuration;
    bool _kickFlightActive;
    Vector2 _kickFlightStart, _kickFlightControlA, _kickFlightControlB, _kickFlightEnd;

    public void StartCurveMove(Transform obj, Vector2 start, Vector2 mid, Vector2 end, float duration, float stopDistance)
    {
        StopCurveMove();
        this.obj = obj;
        this.duration = duration;
        _moveCoroutine = StartCoroutine(MoveAlongCurve(start, mid, end, stopDistance));
    }

    public void StartCurveMove()
    {
        StopCurveMove();
        _moveCoroutine = StartCoroutine(MoveAlongCurve(startPoint.position, midPoint.position, endPoint.position));
    }

    public void StopCurveMove()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        if (_kickFlightCoroutine != null)
            StopCoroutine(_kickFlightCoroutine);
        _moveCoroutine = null;
        _kickFlightCoroutine = null;
        ReleaseKickFlight();
        Moving = false;
    }

    public void StartKickFlight(BallScript ball, Vector2 target)
    {
        StopCurveMove();
        if (ball == null || ball.Rigidbody == null ||
            ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        obj = ball.transform;
        _kickFlightBall = ball;
        _kickFlightGravity = ball.Rigidbody.gravityScale;
        _kickFlightCoroutine = StartCoroutine(KickFlightRoutine(target));
    }

    IEnumerator KickFlightRoutine(Vector2 target)
    {
        Moving = true;
        try
        {
            // Give the leg its physical follow-through before gradually shaping the outgoing shot.
            yield return new WaitForSeconds(0.25f);
            if (_kickFlightBall == null)
                yield break;

            Rigidbody2D body = _kickFlightBall.Rigidbody;
            _kickFlightStart = body.position;
            _kickFlightEnd = target;
            Vector2 launchVelocity = body.linearVelocity;
            float horizontalDistance = Mathf.Abs(target.x - body.position.x);
            float direction = Mathf.Sign(target.x - body.position.x);
            if (horizontalDistance < 0.5f || launchVelocity.x * direction <= 0f)
                yield break;

            _activeKickFlightDuration = Mathf.Max(0.1f, _kickFlightDuration);
            float leadDistance = Mathf.Clamp(
                Mathf.Abs(launchVelocity.x) * _activeKickFlightDuration / 3f,
                horizontalDistance * 0.2f, horizontalDistance * 0.55f);
            float leadHeight = Mathf.Clamp(
                body.position.y + Mathf.Max(0f, launchVelocity.y) * _activeKickFlightDuration / 3f,
                body.position.y, Mathf.Max(body.position.y, target.y) + _kickFlightLift * 0.4f);
            _kickFlightControlA = new Vector2(body.position.x + direction * leadDistance, leadHeight);
            _kickFlightControlB = target + new Vector2(
                -direction * horizontalDistance * 0.18f, _kickFlightLift);

            _kickFlightElapsed = 0f;
            _kickFlightActive = true;
            body.gravityScale = 0f;
            _kickFlightBall.CollisionEntered += OnKickFlightCollision;
            while (_kickFlightActive && _kickFlightBall != null)
                yield return null;
        }
        finally
        {
            ReleaseKickFlight();
            _kickFlightCoroutine = null;
            Moving = false;
        }
    }

    void FixedUpdate()
    {
        if (!_kickFlightActive || _kickFlightBall == null)
            return;

        Rigidbody2D body = _kickFlightBall.Rigidbody;
        if (body.bodyType != RigidbodyType2D.Dynamic || !body.simulated)
        {
            ReleaseKickFlight();
            return;
        }

        _kickFlightElapsed += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(_kickFlightElapsed / _activeKickFlightDuration);
        float u = 1f - t;
        Vector2 desiredPosition = u * u * u * _kickFlightStart +
                                  3f * u * u * t * _kickFlightControlA +
                                  3f * u * t * t * _kickFlightControlB +
                                  t * t * t * _kickFlightEnd;
        Vector2 tangent = 3f * u * u * (_kickFlightControlA - _kickFlightStart) +
                          6f * u * t * (_kickFlightControlB - _kickFlightControlA) +
                          3f * t * t * (_kickFlightEnd - _kickFlightControlB);
        Vector2 correction = Vector2.ClampMagnitude((desiredPosition - body.position) * 3f, 2f);
        Vector2 desiredVelocity = tangent / _activeKickFlightDuration + correction;
        float blend = 1f - Mathf.Exp(-Time.fixedDeltaTime / 0.12f);
        body.linearVelocity = Vector2.Lerp(body.linearVelocity, desiredVelocity, blend);

        if (_kickFlightElapsed >= _activeKickFlightDuration)
            ReleaseKickFlight();
    }

    void OnKickFlightCollision() => ReleaseKickFlight();

    void ReleaseKickFlight()
    {
        _kickFlightActive = false;
        if (_kickFlightBall != null)
        {
            _kickFlightBall.CollisionEntered -= OnKickFlightCollision;
            _kickFlightBall.Rigidbody.gravityScale = _kickFlightGravity;
            _kickFlightBall = null;
        }
    }

    void OnDisable() => StopCurveMove();

    public bool Moving { get; private set; } = false;
    IEnumerator MoveAlongCurve(Vector2 start, Vector2 mid, Vector2 end, float stopDistance = 1)
    {
        Moving = true;
        float elapsed = 0f;
        while (elapsed < duration && Vector2.Distance(obj.position, end) > stopDistance)
        {
            float t = elapsed / duration;
            // Optional easing
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            Vector2 pos = QuadraticBezier(start, mid, end, easedT);
            obj.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Moving = false;
        _moveCoroutine = null;
    }

    Vector2 QuadraticBezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * a
             + 2f * oneMinusT * t * b
             + t * t * c;
    }
}
