using CommonDataTypes;
using Gameplay.Managers;
using Scene_Management;
using System.Collections.Generic;
using UnityEngine;

public class CampaignMatchBuilder : MonoBehaviour
{
    public List<StageAction> preMatchTransitionsAndTutorials;
    public MatchDataSO matchDataSO;
    public MatchManager matchManager;
    public List<StageAction> postMatchTransitionsAndTutorials;

    private void Awake()
    {
        if (matchManager == null)
            Debug.LogError("MatchManager reference is missing in CampaignMatchBuilder!", this);
        else
            matchManager.enabled = false;
    }

    public void BuildMatch()
    {
        MatchSettings matchSettings = new MatchSettings.Builder()
            .WithNumberOfPlayers(matchDataSO.MatchSettings.NumberOfPlayers)
            .WithLeftShirtIndex(matchDataSO.MatchSettings.LeftSideShirtIndex)
            .WithLeftShoesIndex(matchDataSO.MatchSettings.LeftSideShoesIndex)
            .WithLeftCountryImageIndex(matchDataSO.MatchSettings.LeftCountryImageIndex)
            .WithRightShirtIndex(matchDataSO.MatchSettings.RightSideShirtIndex)
            .WithRightShoesIndex(matchDataSO.MatchSettings.RightSideShoesIndex)
            .WithRightCountryImageIndex(matchDataSO.MatchSettings.RightCountryImageIndex)
            .WithIsCampaignMatch(true)
            .Build();

        matchManager.enabled = true;
        Match match = MatchFlow.CreateCampaignMatch(matchSettings);
        matchManager.SetNewMatch(match);
    }
}
