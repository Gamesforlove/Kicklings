using System.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(evt.Name);
        asyncOperation.allowSceneActivation = false;

        //loadingCanvas.SetActive(true);
            
        do {
            await Task.Delay(100);
        } while (asyncOperation.progress < 0.9f);
        
        EventBus<OnSceneLoaded>.Raise(new  OnSceneLoaded(evt.EnumValue));
        
        asyncOperation.allowSceneActivation = true;
    }
}