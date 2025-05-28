using UnityEngine;

namespace Gameplay.CharacterComponents
{
    [CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
    public class EntityData : ScriptableObject
    {
        [Tooltip( "Amount of force applied to the character when jumping, the higher the value, the higher the jump" )]
        public float JumpPower = 3000f;
        [Tooltip( "Amount of force applied to the character when kicking, the higher the value, the harder the kick" )]
        public float KickingPower = 800f;
        [Tooltip( "Amount of stabilization force applied to the character, the higher the value, the more stiff the character will be" )]
        public float StabilizationFactor = 35f;
    }
}