using System.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = CommonDataTypes.Scene;

public class SceneHandler : MonoBehaviour
{
    [Header("Loading Screen Settings")]
    //[SerializeField] private GameObject loadingCanvas;
    //[SerializeField] private Image progressBar;
    private float targetProgress;

    void OnEnable()
    {
        EventBus<CreateMatch>.OnEvent += CreateMatch;
    }

    void OnDisable()
    {
        EventBus<CreateMatch>.OnEvent -= CreateMatch;
    }

    void CreateMatch(CreateMatch _)
    {
        LoadScene(Scene.Gameplay);
    }

    public async void LoadScene(Scene selectedScene)
    {
        targetProgress = 0;
        //progressBar.fillAmount = 0;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(selectedScene.ToString());
        asyncOperation.allowSceneActivation = false;

        //loadingCanvas.SetActive(true);
            
        do {
            await Task.Delay(100);
            targetProgress = asyncOperation.progress;
        } while (asyncOperation.progress < 0.9f);
        // } while (progressBar.fillAmount < 0.9f);


        asyncOperation.allowSceneActivation = true;
        //loadingCanvas.SetActive(false);
    }

    /*private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, targetProgress, 2 * Time.deltaTime);
    }*/
}