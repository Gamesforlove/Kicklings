using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/Campaign/Stage")]
[Serializable]
public class Stage : ScriptableObject
{
    [field: SerializeField] public StageType StageType { get; private set; } = StageType.Linear;
    [SerializeField] private List<CampaignLevelData> levels = new List<CampaignLevelData>();
    public IReadOnlyList<CampaignLevelData> Levels => levels;
    public int LevelCount => levels.Count;
    public bool IsLastLevel(int level) => level == levels.Count - 1;

    public Action<CampaignStructure, bool> EndgameBehavior { get; private set; } = EndgameBehaviour.HandleLinearEndgame;

    public void UpdateBehavior()
    {
        EndgameBehavior = StageType switch
        {
            StageType.Tournament => EndgameBehaviour.HandleTournamentEndgame,
            StageType.Season => EndgameBehaviour.HandleSeasonEndgame,
            StageType.Linear => EndgameBehaviour.HandleLinearEndgame,
            _ => EndgameBehaviour.HandleLinearEndgame
        };
    }
}
public enum StageType
{
    Tournament,
    Season,
    Linear
}
