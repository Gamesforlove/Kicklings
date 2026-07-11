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

        [Header("Default")]
        [SerializeField] DifficultySettings _defaultSettings;
        [Header("Easy")] 
        [SerializeField] DifficultySettings _easy1;
        [SerializeField] DifficultySettings _easy2;
        [SerializeField] DifficultySettings _easy3;
        [Header("Medium")]
        [SerializeField] DifficultySettings _medium4;
        [SerializeField] DifficultySettings _medium5;
        [SerializeField] DifficultySettings _medium6;
        [Header("Hard")]
        [SerializeField] DifficultySettings _hard7;
        [SerializeField] DifficultySettings _hard8;
        [SerializeField] DifficultySettings _hard9;

        public DifficultySettings GetSettingsForDifficulty(DifficultyLevel level)
        {
            return level switch
            {
                DifficultyLevel.Easy1 => _easy1,
                DifficultyLevel.Easy2 => _easy2,
                DifficultyLevel.Easy3 => _easy3,
                DifficultyLevel.Medium4 => _medium4,
                DifficultyLevel.Medium5 => _medium5,
                DifficultyLevel.Medium6 => _medium6,
                DifficultyLevel.Hard7 => _hard7,
                DifficultyLevel.Hard8 => _hard8,
                DifficultyLevel.Hard9 => _hard9,
                _ => _defaultSettings // Default to medium if custom or invalid
            };
        }
    }
    
    public enum DifficultyLevel
    {
        Easy1,
        Easy2,
        Easy3,
        Medium4,
        Medium5,
        Medium6,
        Hard7,
        Hard8,
        Hard9,
        Default
    }
}