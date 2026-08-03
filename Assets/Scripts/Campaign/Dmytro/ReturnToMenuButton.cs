using UnityEngine;
using CommonDataTypes;

public class ReturnToMenuButton : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneHandler.LoadScene(SceneName.MainMenu);
    }
}
