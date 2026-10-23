using UnityEngine;
using CommonDataTypes;
using Scene_Management;
using Invictus.Art.Campaign;

public class CampaignCutsceneController : MonoBehaviour
{
    [SerializeField] private GameObject _nextSceneButton;
    [SerializeField] private CutsceneManager _cutsceneManager;
    private void Start()
    {
        if (_cutsceneManager)
        {
            StartCutscene();
        }
    }
    private void OnEnable()
    {
        if (!_cutsceneManager) return;

        _cutsceneManager.CutsceneEnded += OnCutsceneEnded;
    }
    private void OnDisable()
    {
        if (!_cutsceneManager) return;

        _cutsceneManager.CutsceneEnded -= OnCutsceneEnded;
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
    public void OnCutsceneEnded()
    {
        _nextSceneButton.SetActive(true);
    }
}
