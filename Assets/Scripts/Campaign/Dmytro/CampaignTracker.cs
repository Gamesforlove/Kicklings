using CommonDataTypes;
using EventBusSystem;
using SaveSystem;
using Scene_Management;
using System;
using System.Collections;
using UnityEngine;

public class CampaignTracker : MonoBehaviour
{
    public static CampaignTracker Instance;
    [SerializeField] private CampaignStructure _campaign;

    [Header("Transition")]
    [SerializeField] private GameObject _transitionCanvas;
    [SerializeField] private bool _inTransition = false;
    [SerializeField] private Animator _transition;
    [SerializeField] private float _waitAfterSlideIn;
    [SerializeField] private float _waitBeforeSlideOut;
    
    private TransitionState _transitionState = TransitionState.BehindTheScreen;

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
        if (_inTransition) return;
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
            SaveLoadGame.Load();
            StartCoroutine(NextlevelRoutine());
            return;
        }
    }
    public void ContinueCampaign()
    {
        if (_inTransition) return;
        MatchFlow.ContinueCampaign();
    }
    public void PlayNextlevel()
    {
        if (_inTransition) return;
        StartCoroutine(NextlevelRoutine());
    }
    public void PlaylevelAfterCutScene()
    {
        if (_inTransition) return;
        StartCoroutine(LevelAfterCutSceneRoutine());
    }
    public void ReplayLevel(CampaignLevelData data)
    {
        if (_inTransition) return;
        StartCoroutine(NextlevelRoutine(data));
    }
    public void TransitionToScene(SceneName name)
    {
        if (_inTransition) return;
        StartCoroutine(TransitionToSceneRoutine(name));
    }
    public void TransitionToScene(string SceneName)
    {
        if (_inTransition) return;
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
    public void StartMatch()
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            int playerLevel = SaveLoadGame.LoadedData.PlayerLevel;
            int stage = SaveLoadGame.LoadedData.stage;
            CampaignLevelData levelData = _campaign.GetLevelData(stage, playerLevel);

            MatchSettings matchSettings = new MatchSettings.Builder()
            .WithGoalsToEndMatch(levelData.GoalsToWin)
            .WithNumberOfPlayers(1)
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
    private void StartMatch(CampaignLevelData data, bool isReplayMatch = false)
    {
            MatchSettings matchSettings = new MatchSettings.Builder()
            .WithGoalsToEndMatch(data.GoalsToWin) 
            .WithNumberOfPlayers(1)
            .WithSplitControls(true)
            .WithIsCampaignMatch(true)
            .WithLevelData(data)
            .Build();

            MatchFlow.CreateCampaignMatch(matchSettings, isReplayMatch: isReplayMatch);
    }
    public void HandleEndgame(bool IsWinner)
    {
        if (!SaveLoadGame.DataIsLoaded)
        {
            #if UNITY_EDITOR
                        Debug.LogError("Data is not loaded");
            #endif
        }

        if (MatchFlow.Match == null || MatchFlow.Match.IsReplayMatch) return;

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


    public IEnumerator NextlevelRoutine(CampaignLevelData data = null)
    {
        TransitionSlideIn();
        yield return new WaitForSeconds(_waitAfterSlideIn);

        if (data) StartMatch(data, isReplayMatch: true);
        else StartMatch();
    }
    private IEnumerator OnSceneLoadedRoutine()
    {
        if (_transitionState == TransitionState.SlidedOut) yield break;

        yield return new WaitForSeconds(_waitBeforeSlideOut);
        TransitionSlideOut();
    }
    private IEnumerator LevelAfterCutSceneRoutine()
    {
        TransitionSlideIn();
        yield return new WaitForSeconds(_waitAfterSlideIn);
        EventBus<OnLoadScene>.Raise(new OnLoadScene(MatchFlow.Match.Settings.LevelData.LevelGameplayScene));
    }
    private IEnumerator TransitionToSceneRoutine(SceneName name)
    {
        TransitionSlideIn();
        yield return new WaitForSeconds(_waitAfterSlideIn);
        EventBus<OnLoadScene>.Raise(new OnLoadScene(name));
    }


    private void TransitionSlideIn()
    {
        _inTransition = true;
        _transitionCanvas.SetActive(true);
        _transitionState = TransitionState.SlidedIn;
        _transition.SetTrigger("SlideIn");
    }
    private void TransitionSlideOut()
    {
        _transitionCanvas.SetActive(true);
        _transitionState = TransitionState.SlidedOut;
        _transition.SetTrigger("SlideOut");
        _inTransition = false;
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
        string name = onSceneLoaded.SceneName;
        if (
            //name == SceneName.CampaignStartScreen.ToString() ||
            //name == SceneName.MainMenu.ToString() ||
            name == SceneName.Gameplay.ToString())
        {
            return;
        }
        StartCoroutine(OnSceneLoadedRoutine());
    }
}
