using CommonDataTypes;
using EventBusSystem;
using Gameplay.Managers;
using Scene_Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CampaignGameplayExecution : MonoBehaviour
{
    public List<CampaignMatchBuilder> matchBuilders;

    void Start()
    {
        StartCoroutine(executeMatchesAndTransitions());
    }

    private MatchManager mm => MatchManager.Instance;
    IEnumerator executeMatchesAndTransitions()
    {
        foreach (CampaignMatchBuilder builder in matchBuilders)
        {
            foreach (StageAction action in builder.preMatchTransitionsAndTutorials)
                yield return action.Execute();

            builder.BuildMatch();
            yield return null;
            yield return new WaitUntil(() => mm == null || !mm.enabled || mm.MatchDone);

            foreach (StageAction action in builder.postMatchTransitionsAndTutorials)
                yield return action.Execute();
        }
    }
}