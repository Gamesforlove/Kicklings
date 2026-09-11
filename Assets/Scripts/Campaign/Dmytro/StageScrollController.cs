using SaveSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class StageScrollController : MonoBehaviour
{
    public int CharacterLevel { get; private set; }

    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private MapCharacterController _character;
    [SerializeField] private CampaignLevel[] _levels;
    [SerializeField] private float _xOffset = 960f;
    [SerializeField] private float _defaultScrollTime = 1f;
    [SerializeField] private CampaignStructure _campaign;
    private float _xMaxOffset;
    private float _xMinOffset;
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
        if (CharacterLevel == levelIndex) return;
        if (_busyScrolling) return;

        ForceUpdateCanvas();
        StartCoroutine(ScrollToLevelRoutine(levelIndex, duration));
    }
    public void MoveCharacterToLevel(int levelIndex)
    {
        if (CharacterLevel == levelIndex) return;
        if (_busyMovingCharacter) return;

        ForceUpdateCanvas();
        StartCoroutine(MoveCharacterToLevelRoutine(levelIndex));
    }
    public void MoveCharacterAndScrollToLevel(int levelIndex)
    {
        if (CharacterLevel == levelIndex) return;

        ForceUpdateCanvas();
        if (!_busyMovingCharacter)
            StartCoroutine(MoveCharacterToLevelRoutine(levelIndex));
        if (!_busyScrolling)
            StartCoroutine(ScrollToLevelRoutine(levelIndex, _defaultScrollTime));
    }
    public void InstantMoveCharacterAndScrollToLevel(int levelIndex)
    {
        if (CharacterLevel == levelIndex) return;

        ForceUpdateCanvas();
            StartCoroutine(MoveCharacterToLevelRoutine(levelIndex));
            StartCoroutine(ScrollToLevelRoutine(levelIndex, 0f));
    }
    public void ScrollToLevel_BUTTON(int levelIndex)
    {
        if (_busyScrolling) return;

        ForceUpdateCanvas();
        StartCoroutine(ScrollToLevelRoutine(levelIndex, _defaultScrollTime));
    }
    public IEnumerator ScrollToLevelRoutine(int levelIndex, float duration)
    {
        RectTransform target = _levels[levelIndex].RectTransform;
        if (_scrollRect == null || _scrollRect.content == null || target == null)
            yield break;

        _busyScrolling = true;

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

    private static void ForceUpdateCanvas()
    {
        Canvas.ForceUpdateCanvases();// for actual positions
    }

    public IEnumerator MoveCharacterToLevelRoutine(int levelIndex, float duration = .3f)
    {
        ForceUpdateCanvas();
        _busyMovingCharacter = true;
        _levels[CharacterLevel].DisableStartButton();
        //_character.StepDuration = duration;
        int step = CharacterLevel < levelIndex ? 1 : -1;
        int i = CharacterLevel;
        while (i != levelIndex)
        {
            i += step;
            yield return StartCoroutine(_character.MoveToPoint(_levels[i].CharacterPoint.position));
        }
        CharacterLevel = levelIndex;
        UpdateSaveData();
        _busyMovingCharacter = false;
        _levels[levelIndex].EnableLevel();
        _levels[levelIndex].EnableStartButton();
    }
    public void InstantMoveCharacterToLevel(int levelIndex)
    {
        ForceUpdateCanvas();
        _levels[CharacterLevel].DisableStartButton();

        _character.TeleportToPoint(_levels[levelIndex].CharacterPoint.position);

        CharacterLevel = levelIndex;
        UpdateSaveData();
        _levels[levelIndex].EnableLevel();
        _levels[levelIndex].EnableStartButton();
    }

    private void UpdateSaveData()
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.LoadedData.PlayerLevel = CharacterLevel;
            if (CharacterLevel > SaveLoadGame.LoadedData.lastUnlockedLevel)
            {
                SaveLoadGame.LoadedData.lastUnlockedLevel = CharacterLevel;
            }
        }
    }

    public void EnableLevels(int last)
    {
        for (int i = 0; i <= last; i++)
        {
            _levels[i].EnableLevel();
            _levels[i].EnableStartButton();
        }
    }
    public void EnableLevels(int level, int stage)
    {
        int levelIndex = 0;
        int lastCompletedStage = (int)Mathf.Clamp01(stage - 1);
        for (int i = 0; i < lastCompletedStage; i++)
        {
            int levelCount = _campaign.Stages[i].LevelCount;
            for (int j = 0; j <= levelCount; j++, levelIndex++)
            {
                _levels[levelIndex].EnableLevel();
                _levels[levelIndex].EnableStartButton();
                //_levels[levelIndex].InitializeLevelData();
            }
        }
        for (int j = 0; j <= level; j++, levelIndex++)
        {
            _levels[levelIndex].EnableLevel();
            _levels[levelIndex].EnableStartButton();
            //_levels[levelIndex].InitializeLevelData();
        }
    }
}
