using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CampaignFlow", menuName = "Scriptable Objects/CampaignFlow")]
public class CampaignFlow : ScriptableObject
{
    public List<StageFlow> stages;
}
