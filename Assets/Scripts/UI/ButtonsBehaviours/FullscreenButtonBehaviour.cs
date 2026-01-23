using System;
using UnityEngine;

namespace UI.ButtonsBehaviours
{
    public class FullscreenButtonBehaviour : ButtonBehaviour
    {
        protected override void Start()
        {
            base.Start();
           #if !UNITY_WEBGL
            // Solo aplicar automáticamente el fullscreen en plataformas que lo permiten directamente
            bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            Screen.fullScreen = savedFullscreen;
            #endif
        }

        protected override void OnClick()
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
