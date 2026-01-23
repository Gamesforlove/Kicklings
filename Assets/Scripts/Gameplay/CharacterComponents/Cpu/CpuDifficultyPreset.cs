using UnityEngine;

namespace Gameplay.CharacterComponents.Cpu
{
    [CreateAssetMenu(fileName = "CpuDifficultyPreset", menuName = "Game/CPU Difficulty Preset")]
    public class CpuDifficultyPreset : ScriptableObject
    {
        [System.Serializable]
        public struct FloatRange
        {
            public float Min;
            public float Max;

            public float RandomValue => Random.Range(Min, Max);
        }
        
        [System.Serializable]
        public struct ProximityPoint
        {
            [SerializeField] string _name;
            public float BaseRadius;
        }

        [System.Serializable]
        public class DifficultySettings
        {
            [Tooltip("Time taken in between kicks (in seconds)")]
            public FloatRange TimeBetweenKicks;
    
            [Tooltip("Delay in which the kick will be performed after a sensor detects the ball (in seconds)")]
            public FloatRange ReactionTime;
            
            public ProximityPoint[] ProximityPoints;
        }

        [Header("Difficulty Configurations")] 
        [SerializeField] DifficultySettings _easySettings;
        [SerializeField] DifficultySettings _mediumSettings;
        [SerializeField] DifficultySettings _hardSettings;
        [SerializeField] DifficultySettings _defaultSettings;

        public DifficultySettings GetSettingsForDifficulty(DifficultyLevel level)
        {
            return level switch
            {
                DifficultyLevel.Easy => _easySettings,
                DifficultyLevel.Medium => _mediumSettings,
                DifficultyLevel.Hard => _hardSettings,
                _ => _defaultSettings // Default to medium if custom or invalid
            };
        }
    }
    
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        Default
    }
}