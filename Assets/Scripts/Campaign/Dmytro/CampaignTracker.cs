using CommonDataTypes;
using EventBusSystem;
using SaveSystem;
using Scene_Management;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;

public class CampaignTracker : MonoBehaviour
{
    public static CampaignTracker Instance;
    [SerializeField] private CampaignStructure _campaign;
    [Header("Transition")]
    [SerializeField] private GameObject _transitionCanvas;
    [SerializeField] private Animator _transition;
    [SerializeField] private float _waitAfterSlideIn;
    [SerializeField] private float _waitBeforeSlideOut;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        EventBus<OnSceneLoaded>.OnEvent += OnSceneLoaded;
    }
    private void OnDisable()
    {
        EventBus<OnSceneLoaded>.OnEvent -= OnSceneLoaded;
    }
    public void StartCampaign() 
    {
        if (SaveLoadGame.Load())
        {
            #if UNITY_EDITOR
                Debug.Log("Save file check is healthy");
            #endif
            StartCoroutine(NextlevelRoutine());
            //StartMatch(1);
        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogError("Can't load saved data, created a fresh save");
            #endif
            SaveLoadGame.Save(new StorageData());
            StartCoroutine(NextlevelRoutine());
            return;
        }
    }
    public void ContinueCampaign()
    {
        MatchFlow.ContinueCampaign();
    }
    public void PlayNextlevel()
    {
        StartCoroutine(NextlevelRoutine());
    }
    public void PlaylevelAfterCutScene()
    {
        StartCoroutine(LevelAfterCutSceneRoutine());
    }
    public void ReplayLevel(int stage, int level)
    {
        //StartCoroutine(ReplayLevelRoutine());
    }
    public void TransitionToScene(SceneName name)
    {
        StartCoroutine(TransitionToSceneRoutine(name));
    }
    public void TransitionToScene(string SceneName)
    {
        if (Enum.TryParse(SceneName, out SceneName name))
        {
            StartCoroutine(TransitionToSceneRoutine(name));
        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogError("Invalid scene name");
            #endif
        }
    }
    public void StartMatch(int numberOfPlayers)
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            int playerLevel = SaveLoadGame.LoadedData.PlayerLevel;
            int stage = SaveLoadGame.LoadedData.stage;
            CampaignLevelData levelData = _campaign.GetLevelData(stage, playerLevel);

            MatchSettings matchSettings = new MatchSettings.Builder()
            .WithNumberOfPlayers(numberOfPlayers)
            .WithIsCampaignMatch(true)
            .WithLevelData(levelData)
            .Build();

            MatchFlow.CreateCampaignMatch(matchSettings);
        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogError("Can't load saved data");
            #endif
            return;
        }
    }
    public void HandleEndgame(bool IsWinner)
    {
        if (!SaveLoadGame.DataIsLoaded)
        {
            #if UNITY_EDITOR
                        Debug.LogError("Data is not loaded");
            #endif
        }

        if (MatchFlow.Match == null || MatchFlow.Match.IsReplayMatch)
        {
            return;
        }

        _campaign.CurrentStage.EndgameBehavior.Invoke(_campaign, IsWinner);        
    }
    public void HandleEndgame(/*param 1,2,3*/)//for minigames
    {
        if (!SaveLoadGame.DataIsLoaded)
        {
            #if UNITY_EDITOR
                        Debug.LogError("Data is not loaded");
            #endif
        }

        if (MatchFlow.Match == null || MatchFlow.Match.IsReplayMatch)
        {
            return;
        }

        EndgameBehaviour.IncrementAndSaveData(_campaign);        
    }
    public IEnumerator NextlevelRoutine()
    {
        _transitionCanvas.SetActive(true);
        _transition.SetTrigger("SlideIn");
        yield return new WaitForSeconds(_waitAfterSlideIn);
        StartMatch(1);
    }
    private IEnumerator OnSceneLoadedRoutine()
    {
        _transitionCanvas.SetActive(true);
        yield return new WaitForSeconds(_waitBeforeSlideOut);
        _transition.SetTrigger("SlideOut");
    }
    private IEnumerator LevelAfterCutSceneRoutine()
    {
        _transitionCanvas.SetActive(true);
        _transition.SetTrigger("SlideIn");
        yield return new WaitForSeconds(_waitAfterSlideIn);
        EventBus<OnLoadScene>.Raise(new OnLoadScene(MatchFlow.Match.Settings.LevelData.LevelGameplayScene));
    }
    private IEnumerator TransitionToSceneRoutine(SceneName name)
    {
        _transitionCanvas.SetActive(true);
        _transition.SetTrigger("SlideIn");
        yield return new WaitForSeconds(_waitAfterSlideIn);
        EventBus<OnLoadScene>.Raise(new OnLoadScene(name));
    }


    private void OnApplicationQuit()
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
    }
    private void OnSceneLoaded(OnSceneLoaded onSceneLoaded)
    {
/*        string name = onSceneLoaded.SceneName;
        if (name == SceneName.CampaignStartScreen.ToString() ||
            name == SceneName.MainMenu.ToString() ||
            name == SceneName.Gameplay.ToString())
        {
            return;
        }*/
        StartCoroutine(OnSceneLoadedRoutine());
    }
}
