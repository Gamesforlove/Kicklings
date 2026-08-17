using UnityEngine;
using EventBusSystem;
using CommonDataTypes;
using Scene_Management;

public class CampaignCutsceneController : MonoBehaviour
{
    public void GoToNextScene()
    {
        if (MatchFlow.Match.IsFinished)
        {
            CampaignTracker.Instance.PlayNextlevel();
        }
        else
        {
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.CampaignGameplay)); 
        }
    }
}
