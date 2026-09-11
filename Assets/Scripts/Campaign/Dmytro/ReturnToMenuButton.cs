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
        SceneHandler.LoadScene(SceneName.MainMenu);
    }
    public void ReturnToMap()
    {
        MatchFlow.DisposeMatch();
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
        SceneHandler.LoadScene(SceneName.CampaignMap);
    }
}
