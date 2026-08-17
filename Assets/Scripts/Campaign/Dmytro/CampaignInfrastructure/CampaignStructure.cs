using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CampaignStructure", menuName = "Scriptable Objects/Campaign/CampaignStructure")]
[Serializable]
public class CampaignStructure : ScriptableObject
{
    [SerializeField] private List<Stage> stages = new List<Stage>();
    public IReadOnlyList<Stage> Stages => stages;
    public void InsertStage(int insertAt, Stage stage) // for narrative branching
    {
        stages.Insert(insertAt, stage);
    }
    public CampaignLevelData GetLevelData(int stage, int level)
    {
        return stages[stage].Levels[level];
    }
}
