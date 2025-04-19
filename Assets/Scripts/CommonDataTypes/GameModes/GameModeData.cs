using UnityEngine;

namespace CommonDataTypes
{
    [CreateAssetMenu(fileName = "GameModeData", menuName = "GameModeData")]
    public class GameModeData : ScriptableObject
    {
        [field: SerializeField] public int NumberOfPlayers { get; private set; }
        [field: SerializeField] public int TotalEntities { get; private set; }
    }
}