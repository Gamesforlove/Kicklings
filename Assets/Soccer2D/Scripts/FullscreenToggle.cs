using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private bool isFullscreen;

    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        Debug.Log("FULLSCREEN TOGGLE: " + isFullscreen);
        SetFullscreen();
    }

    private void SetFullscreen() => Screen.fullScreen = isFullscreen;
}
