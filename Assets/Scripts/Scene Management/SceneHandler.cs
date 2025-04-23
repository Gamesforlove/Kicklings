using System.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = CommonDataTypes.Scene;

public class SceneHandler : MonoBehaviour
{
    void OnEnable()
    {
        EventBus<OnLoadScene>.OnEvent += LoadScene;
    }

    void OnDisable()
    {
        EventBus<OnLoadScene>.OnEvent -= LoadScene;
    }

    public async void LoadScene(OnLoadScene evt)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(evt.SceneName);
        asyncOperation.allowSceneActivation = false;

        //loadingCanvas.SetActive(true);
            
        do {
            await Task.Delay(100);
        } while (asyncOperation.progress < 0.9f);
        
        asyncOperation.allowSceneActivation = true;
    }
}