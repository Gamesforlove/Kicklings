using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(ScrollView))]
public class StageScrollController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform[] _levels;
    [SerializeField] private float _xOffset = 1000f;
    private float _xMaxOffset;
    private float _xMinOffset;

    private void Awake()
    {
        // 1. Берем компонент ScrollRect, который висит на этом же GameObject
        if (_scrollRect == null)
            _scrollRect = GetComponent<ScrollRect>();

        // 2. Достаем все дочерние элементы из объекта Content
        if (_scrollRect != null && _scrollRect.content != null)
        {
            _levels = _scrollRect.content.Cast<RectTransform>().ToArray();
        }
        _xMaxOffset = _scrollRect.content.offsetMin.x;
        _xMinOffset = -_scrollRect.content.offsetMax.x;
    }
    private void Start()
    {
        //StartCoroutine(ScrollToLevel(_levels.Last()));
    }
    public void ScrollToLevel(int levelIndex, float duration = 0.5f)
    {
        StartCoroutine(ScrollToLevelRoutine(_levels[levelIndex], duration));
    }
    public void ScrollToLevel_BUTTON(int levelIndex)
    {
        StartCoroutine(ScrollToLevelRoutine(_levels[levelIndex], 1f));
    }

    public IEnumerator ScrollToLevelRoutine(RectTransform target, float duration)
    {
        if (_scrollRect == null || _scrollRect.content == null || target == null)
            yield break;

        Canvas.ForceUpdateCanvases();// for actual positions

        Vector2 targetLocalPos = _scrollRect.content.InverseTransformPoint(target.position);



        Vector2 targetContentPos = new Vector2(Mathf.Clamp(-targetLocalPos.x + _xOffset, _xMinOffset, _xMaxOffset), -targetLocalPos.y);


        // Учитываем текущие ограничение только по одной оси, если скролл горизонтальный или вертикальный:
        if (!_scrollRect.horizontal) targetContentPos.x = _scrollRect.content.anchoredPosition.x;
        if (!_scrollRect.vertical) targetContentPos.y = _scrollRect.content.anchoredPosition.y;

        Vector2 startPos = _scrollRect.content.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _scrollRect.content.anchoredPosition = Vector2.Lerp(startPos, targetContentPos, smoothT);
            yield return null;
        }

        _scrollRect.content.anchoredPosition = targetContentPos;
    }
}
