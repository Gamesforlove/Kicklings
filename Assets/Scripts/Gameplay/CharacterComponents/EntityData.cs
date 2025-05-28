using UnityEngine;

namespace Gameplay.CharacterComponents
{
    [CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
    public class EntityData : ScriptableObject
    {
        public float JumpPower = 3000f;
        public float KickingPower = 800f;
        public float StabilizationFactor = 35f;
    }
}