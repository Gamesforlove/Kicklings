using System.Collections;
using EventBusSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    void OnEnable()
    {
        EventBus<OnLoadScene>.OnEvent += HandleLoadScene;
    }

    void OnDisable()
    {
        EventBus<OnLoadScene>.OnEvent -= HandleLoadScene;
    }

    private void HandleLoadScene(OnLoadScene evt)
    {
        StartCoroutine(LoadSceneCoroutine(evt));
    }

    IEnumerator LoadSceneCoroutine(OnLoadScene evt)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(evt.Name);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(0.1f);

        EventBus<OnSceneLoaded>.Raise(new OnSceneLoaded(evt.EnumValue));

        asyncOperation.allowSceneActivation = true;
    }
}