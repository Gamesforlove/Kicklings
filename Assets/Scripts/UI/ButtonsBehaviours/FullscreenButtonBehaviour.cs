using System;
using UnityEngine;

namespace UI.ButtonsBehaviours
{
    public class FullscreenButtonBehaviour : MonoBehaviour
    {
        void Start()
        {
            bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            Screen.fullScreen = savedFullscreen;
        }

        public void ToggleFullscreen()
        {
            bool newFullscreenState = !Screen.fullScreen;
            
            Screen.fullScreen = newFullscreenState;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            
            PlayerPrefs.SetInt("Fullscreen", newFullscreenState ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"Modo pantalla completa actualizado: {newFullscreenState}");
        }
    }
}
