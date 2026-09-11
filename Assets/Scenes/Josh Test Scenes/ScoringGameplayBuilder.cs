using CommonDataTypes;
using EventBusSystem;
using Gameplay.Managers;
using Scene_Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoringGameplayBuilder : MonoBehaviour
{
    public List<MinigameMatchBuilder> matchBuilders;

    void Awake()
    {
        StartCoroutine(executeMatchesAndTransitions());
    }

    private ScoringChallengeManager mm => ScoringChallengeManager.Instance;
    IEnumerator executeMatchesAndTransitions()
    {
        foreach (MinigameMatchBuilder builder in matchBuilders)
        {

            builder.BuildMatch();
            yield return null;
            yield return new WaitUntil(() => mm == null || !mm.enabled);
            
            //|| mm.MatchDone
        }
    }
}