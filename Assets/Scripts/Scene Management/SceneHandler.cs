using CommonDataTypes;
using EventBusSystem;
using System;
using System.Collections;
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
        
        yield return new WaitForSecondsRealtime(0.1f);

        EventBus<OnSceneLoaded>.Raise(new OnSceneLoaded(evt.EnumValue));

        asyncOperation.allowSceneActivation = true;
    }
    public static void LoadScene(SceneName name)
    {
        EventBus<OnLoadScene>.Raise(new OnLoadScene(name));
    }
    public static void LoadSceneByName(string _name)
    {
        if (Enum.TryParse(_name, out SceneName name))
        {
            EventBus<OnLoadScene>.Raise(new OnLoadScene(name));
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogError("Invalid scene name");
            #endif
        }
    }
}