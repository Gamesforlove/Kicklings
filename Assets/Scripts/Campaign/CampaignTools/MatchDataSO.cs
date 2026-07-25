using CommonDataTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchDataSO", menuName = "Scriptable Objects/MatchData")]
public class MatchDataSO : ScriptableObject
{
    [SerializeField]
    public MatchSettings MatchSettings = new MatchSettings();
}
