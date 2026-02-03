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

    void Awake()
    {
        StartCoroutine(executeMatchesAndTransitions());
    }

    private MatchManager mm => MatchManager.Instance;
    IEnumerator executeMatchesAndTransitions()
    {
        foreach (CampaignMatchBuilder builder in matchBuilders)
        {
            if (builder != null && builder.preMatchTransitionsAndTutorials != null)
            {
                foreach (StageAction action in builder.preMatchTransitionsAndTutorials)
                    if (action != null)
                        yield return action.Execute();
            }

            builder.BuildMatch();
            yield return null;
            yield return new WaitUntil(() => mm == null || !mm.enabled || mm.MatchDone);

            if (builder != null && builder.postMatchTransitionsAndTutorials != null)
            {
                foreach (StageAction action in builder.postMatchTransitionsAndTutorials)
                if (action != null)
                    yield return action.Execute();
            }
        }
    }
}