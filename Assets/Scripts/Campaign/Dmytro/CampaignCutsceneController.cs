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
    private void Start()
    {
        if (_cutsceneManager)
        {
            StartCutscene();
        }
    }
    private void OnEnable()
    {
        if (_cutsceneManager) _cutsceneManager.CutsceneEnded += OnCutsceneEnded;
        
        if (_cutsceneBuilder) _cutsceneBuilder.CutsceneEnded += OnCutsceneEnded;
    }
    private void OnDisable()
    {
        if (_cutsceneManager) _cutsceneManager.CutsceneEnded -= OnCutsceneEnded;

        if (_cutsceneBuilder) _cutsceneBuilder.CutsceneEnded -= OnCutsceneEnded;
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
    public void EnableNextSceneButton() => _nextSceneButton.SetActive(true);
    public void StartCutscene() => _cutsceneManager.PlayCutscene();
    private void OnCutsceneEnded()
    {
        _nextSceneButton.SetActive(true);
        _cutsceneEndedNotification.SetActive(true);
    }
}
