using System.Collections;
using UnityEngine;

public class MoveToPointCurved : MonoBehaviour
{
    public Transform obj;
    public Transform startPoint;
    public Transform midPoint;
    public Transform endPoint;
    public float duration = 1f;

    public void StartCurveMove(Transform obj, Vector2 start, Vector2 mid, Vector2 end, float duration)
    {
        this.obj = obj;
        this.duration = duration;
        StartCoroutine(MoveAlongCurve(start, mid, end));
    }

    public void StartCurveMove()
    {
        StartCoroutine(MoveAlongCurve(startPoint.position, midPoint.position, endPoint.position));
    }

    public bool Moving { get; private set; } = false;
    IEnumerator MoveAlongCurve(Vector2 start, Vector2 mid, Vector2 end)
    {
        Moving = true;
        float elapsed = 0f;
        while (elapsed < duration && Vector2.Distance(obj.position, end) > 1f)
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
    }

    Vector2 QuadraticBezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * a
             + 2f * oneMinusT * t * b
             + t * t * c;
    }
}