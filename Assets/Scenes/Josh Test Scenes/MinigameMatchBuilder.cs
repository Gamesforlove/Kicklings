using CommonDataTypes;
using Gameplay.Managers;
using Scene_Management;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MinigameMatchBuilder : MonoBehaviour
{
    public MatchDataSO matchDataSO;
    public ScoringChallengeManager ScoringChallengeManager;
    
    private void Awake()
    {
        if (ScoringChallengeManager == null)
            Debug.LogError("MatchManager reference is missing in CampaignMatchBuilder!", this);
        else
            ScoringChallengeManager.enabled = false;
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

        ScoringChallengeManager.enabled = true;
        Match match = MatchFlow.CreateCampaignMatch(matchSettings);
        ScoringChallengeManager.StartChallenge();
    }
}