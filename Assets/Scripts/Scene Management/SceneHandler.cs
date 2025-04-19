using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }

    [Header("Loading Screen Settings")]
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image progressBar;
    private float targetProgress;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else {
            Destroy(gameObject);
        }
    }

    public async void LoadScene(Scene selectedScene)
    {
        targetProgress = 0;
        progressBar.fillAmount = 0;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(selectedScene.ToString());
        asyncOperation.allowSceneActivation = false;

        loadingCanvas.SetActive(true);
            
        do {
            await Task.Delay(100);
            targetProgress = asyncOperation.progress;
        } while (asyncOperation.progress < 0.9f);
        // } while (progressBar.fillAmount < 0.9f);


        asyncOperation.allowSceneActivation = true;
        loadingCanvas.SetActive(false);
    }

    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, targetProgress, 2 * Time.deltaTime);
    }
}

[Serializable]
public enum Scene
{
    MainMenu,
    GameScene
}
