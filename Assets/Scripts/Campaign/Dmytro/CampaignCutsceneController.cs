using UnityEngine;
using CommonDataTypes;
using Scene_Management;
using Invictus.Art.Campaign;

public class CampaignCutsceneController : MonoBehaviour
{
    [SerializeField] private GameObject _nextSceneButton;
    [SerializeField] private GameObject _cutsceneEndedNotification;
    [SerializeField] private CutsceneManager _cutsceneManager;
    [SerializeField] private CutsceneBuilder _cutsceneBuilder;

    private CampaignCutsceneSkip _skip;
    internal GameObject NextSceneButton => _nextSceneButton;
    internal bool IsAwaitingContinue =>
        (_nextSceneButton != null && _nextSceneButton.activeInHierarchy) ||
        (_cutsceneEndedNotification != null && _cutsceneEndedNotification.activeInHierarchy);

    private void Start()
    {
        if (_cutsceneManager)
        {
            StartCutscene();
        }
    }

    private void LateUpdate() => _skip?.RefreshVisibility();

    private void OnEnable()
    {
        if (_cutsceneManager) _cutsceneManager.CutsceneEnded += OnCutsceneEnded;
        
        if (_cutsceneBuilder)
        {
            _cutsceneBuilder.CutsceneEnded += OnCutsceneEnded;
            _skip ??= new CampaignCutsceneSkip(this, _cutsceneBuilder);
            _skip.Enable();
        }
    }
    private void OnDisable()
    {
        if (_cutsceneManager) _cutsceneManager.CutsceneEnded -= OnCutsceneEnded;

        if (_cutsceneBuilder) _cutsceneBuilder.CutsceneEnded -= OnCutsceneEnded;

        _skip?.Disable();
    }
    public void GoToNextScene()
    {
        if (MatchFlow.Match.IsFinished)
        {
            if (MatchFlow.Match.IsReplayMatch)
            {
                CampaignTracker.Instance.TransitionToScene(SceneName.CampaignMap);
                return;
            }
            CampaignTracker.Instance.PlayNextlevel();
        }
        else
        {
            CampaignTracker.Instance.PlaylevelAfterCutScene(); 
        }
    }
    public void EnableNextSceneButton()
    {
        _nextSceneButton.SetActive(true);
        _skip?.RefreshVisibility();
    }
    public void StartCutscene() => _cutsceneManager.PlayCutscene();
    private void OnCutsceneEnded()
    {
        EnableNextSceneButton();
        _cutsceneEndedNotification.SetActive(true);
    }
}
