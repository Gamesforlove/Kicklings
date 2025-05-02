using UnityEngine;

namespace Gameplay.Managers
{
    public class PauseManager : MonoBehaviour
    {
        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }
    }
}
