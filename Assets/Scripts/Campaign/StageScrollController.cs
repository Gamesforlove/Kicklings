using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(ScrollView))]
public class StageScrollController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private MapCharacterController _character;
    [SerializeField] private CampaignLevel[] _levels;
    [SerializeField] private float _xOffset = 960f;
    private float _xMaxOffset;
    private float _xMinOffset;
    private int _characterLevel;
    private bool _busyScrolling = false;
    private bool _busyMovingCharacter = false;

    private void Awake()
    {
        if (_scrollRect == null)
            _scrollRect = GetComponent<ScrollRect>();

        if (_scrollRect != null && _scrollRect.content != null)
        {
            _levels = _scrollRect.content.GetComponentsInChildren<CampaignLevel>();
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
        if (_busyScrolling) return;
        StartCoroutine(ScrollToLevelRoutine(_levels[levelIndex].RectTransform, duration));
    }
    public void MoveCharacterToLevel(int levelIndex)
    {
        if (_busyMovingCharacter) return;
        StartCoroutine(MoveCharacterToLevelRoutine(levelIndex));
    }
    public void MoveCharacterAndScrollToLevel(int levelIndex)
    {
        if (!_busyMovingCharacter)
            StartCoroutine(MoveCharacterToLevelRoutine(levelIndex));
        if (!_busyScrolling)
            StartCoroutine(ScrollToLevelRoutine(_levels[levelIndex].RectTransform, 1f));
    }
    public void ScrollToLevel_BUTTON(int levelIndex)
    {
        if (_busyScrolling) return;
        StartCoroutine(ScrollToLevelRoutine(_levels[levelIndex].RectTransform, 1f));
    }
    public IEnumerator ScrollToLevelRoutine(RectTransform target, float duration)
    {
        if (_scrollRect == null || _scrollRect.content == null || target == null)
            yield break;

        _busyScrolling = true;
        Canvas.ForceUpdateCanvases();// for actual positions

        Vector2 targetLocalPos = _scrollRect.content.InverseTransformPoint(target.position);

        Vector2 targetContentPos = new Vector2(Mathf.Clamp(-targetLocalPos.x + _xOffset, _xMinOffset, _xMaxOffset), -targetLocalPos.y);

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
        _busyScrolling = false;
    }
    public IEnumerator MoveCharacterToLevelRoutine(int levelIndex, float duration = .3f)
    {
        _busyMovingCharacter = true;
        //_character.StepDuration = duration;
        int step = _characterLevel < levelIndex ? 1 : -1;
        int i = _characterLevel;
        while (i != levelIndex)
        {
            i += step;
            yield return StartCoroutine(_character.MoveToPoint(_levels[i].CharacterPoint.position));
        }
        _characterLevel = levelIndex;
        _busyMovingCharacter = false;
    }
}
