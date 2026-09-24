using UnityEngine;
using CommonDataTypes;
using Scene_Management;
using SaveSystem;

public class ReturnToMenuButton : MonoBehaviour
{
    public void ReturnToMenu()
    {
        MatchFlow.DisposeMatch();
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
        CampaignTracker.Instance.TransitionToScene(SceneName.MainMenu);
        //SceneHandler.LoadScene(SceneName.MainMenu);
    }
    public void ReturnToMap()
    {
        MatchFlow.DisposeMatch();
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
        CampaignTracker.Instance.TransitionToScene(SceneName.CampaignMap);
        //SceneHandler.LoadScene(SceneName.CampaignMap);
    }
}
