using UnityEngine;
using EventBusSystem;
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
    public void GoToNextScene()
    {
        if (MatchFlow.Match.IsFinished)
        {
            CampaignTracker.Instance.PlayNextlevel();
        }
        else
        {
            CampaignTracker.Instance.PlaylevelAfterCutScene(); 
        }
    }
    public void EnableNextSceneButton() => _nextSceneButton.SetActive(true);
    public void StartCutscene() => _cutsceneManager.PlayCutscene();
}
