using System.Collections;
using UnityEngine;

public class MapCharacterController : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    public float StepDuration = .3f;
    public IEnumerator MoveToPoint(Vector2 point)
    {
        Vector2 startPos = _rect.anchoredPosition;
        point = _rect.parent.InverseTransformPoint(point);
        float elapsed = 0f;
        while (elapsed < StepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / StepDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _rect.anchoredPosition = Vector2.Lerp(startPos, point, smoothT);
            yield return null;
        }
        _rect.anchoredPosition = point;
    }
}
