using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/Campaign/Stage")]
[Serializable]
public class Stage : ScriptableObject
{
    [SerializeField] private List<CampaignLevelData> levels = new List<CampaignLevelData>();
    public IReadOnlyList<CampaignLevelData> Levels => levels;
    public int LevelCount => levels.Count;
}
