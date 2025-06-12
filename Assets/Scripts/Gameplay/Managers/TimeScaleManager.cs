using UnityEngine;

namespace Gameplay.Managers
{
    public static class TimeScaleManager
    {
        const float GameplayTimeScale = 1.5f;
        const float SlowMotionTimeScale = 0.2f;
        public static void PauseGame()
        {
            Time.timeScale = 0;
        }
        
        public static void SetDefaultTimeScale()
        {
            Time.timeScale = 1;
        }
        
        public static void SetGameplayTimeScale()
        {
            Time.timeScale = GameplayTimeScale;
        }
        
        public static void SlowMotion()
        {
            Time.timeScale = SlowMotionTimeScale;
        }
    }
}
