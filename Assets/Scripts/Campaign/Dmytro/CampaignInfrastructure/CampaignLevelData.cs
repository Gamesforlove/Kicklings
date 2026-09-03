using CommonDataTypes;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CampaignLevelData", menuName = "Scriptable Objects/Campaign/CampaignLevelData")]
[Serializable]
public class CampaignLevelData : ScriptableObject
{
    [field: SerializeField] public GameObject Player1 { get; private set; } 
    [field: SerializeField] public GameObject Player2 { get; private set; } 
    [field: SerializeField] public GameObject Opponent1 { get; private set; }
    [field: SerializeField] public GameObject Opponent2 { get; private set; }
    [field: SerializeField] public SceneName PreMatchCutScene { get; private set; } = SceneName.None;
    [field: SerializeField] public SceneName AfterMatchCutScene { get; private set; } = SceneName.None;
    [field: SerializeField] public SceneName AfterMatchDefeatCutScene { get; private set; } = SceneName.None;
    [field: SerializeField] public TutorialType TutorialMatch { get; private set; } = TutorialType.None;
    [field: SerializeField] public SceneName LevelGameplayScene { get; private set; } = SceneName.CampaignGameplay; // for minigames

    [field: SerializeField] public bool CustomBehaviourAfterLevel { get; private set; } = false;

    [SerializeField] private Stage insertStage;

    public void ExecuteCustomBehavior()
    {

    }
}
public enum TutorialType
{
    None,
    BasicTutorial,
    PassToturial
}
