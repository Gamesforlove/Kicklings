using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CampaignFlow", menuName = "Scriptable Objects/CampaignFlow")]
[System.Serializable]
public class CampaignFlowSO : ScriptableObject
{
    public List<StageFlowSO> stages;
}
