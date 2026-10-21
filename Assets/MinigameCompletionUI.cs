using Scene_Management;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays a timed minigame result without depending on a particular minigame.
/// Call ShowCompletion(elapsedSeconds) when gameplay ends. Retry reloads the
/// minigame scene, and Finish can advance the active campaign.
/// </summary>
[DisallowMultipleComponent]
public sealed class MinigameCompletionUI : MonoBehaviour
{
    public enum CompletionRank { None, A, B, C }

    [Header("Rank Times (Seconds)")]
    [Tooltip("Maximum completion time for A, inclusive. Lower times are better.")]
    [Min(0f)] public float aRankTime = 8f;
    [Tooltip("Maximum completion time for B, inclusive. Set this at or above the A time.")]
    [Min(0f)] public float bRankTime = 22f;
    [Tooltip("Maximum completion time for C, inclusive. Set this at or above the B time. Slower results are unranked.")]
    [Min(0f)] public float cRankTime = 36f;

    [Header("UI References")]
    [Tooltip("The popup root, including its backdrop. If omitted, this component's GameObject is shown and hidden instead.")]
    public GameObject completionPanel;
    public TMP_Text titleText;
    public TMP_Text rankText;
    public TMP_Text timeText;
    public Button retryButton;
    public Button finishButton;

    [Header("Presentation")]
    public bool hideOnAwake = true;
    public string completionTitle = "MINIGAME COMPLETE!";
    public Color aRankColor = new Color32(99, 230, 129, 255);
    public Color bRankColor = new Color32(85, 221, 242, 255);
    public Color cRankColor = new Color32(255, 118, 95, 255);
    public string unrankedLabel = "-";
    public Color unrankedColor = Color.white;

    [Header("Button Behavior")]
    [Tooltip("Retry reloads the entire scene containing this UI. Disable this to provide a custom restart through On Retry.")]
    public bool reloadSceneOnRetry = true;
    [Tooltip("Finish records a win and advances the active campaign. Disable this to provide custom completion through On Finish.")]
    public bool continueCampaignOnFinish = true;

    [Header("Optional Button Events")]
    [Tooltip("Invoked after the popup closes, before the default scene reload.")]
    public UnityEvent onRetry = new UnityEvent();
    [Tooltip("Invoked after the popup closes and the default campaign completion has been handled, if a campaign is active.")]
    public UnityEvent onFinish = new UnityEvent();

    public float CompletionTime { get; private set; }
    public CompletionRank CurrentRank { get; private set; }
    public bool IsVisible => isShowingCompletion &&
        (completionPanel != null ? completionPanel.activeInHierarchy : gameObject.activeInHierarchy);

    private bool initialized;
    private bool isShowingCompletion;
    private Button boundRetryButton;
    private Button boundFinishButton;

    private void Awake()
    {
        Initialize();
        if (hideOnAwake && !isShowingCompletion)
            Hide();
    }

    private void OnDestroy()
    {
        if (boundRetryButton != null)
            boundRetryButton.onClick.RemoveListener(Retry);
        if (boundFinishButton != null)
            boundFinishButton.onClick.RemoveListener(Finish);
    }

    public void ShowCompletion(float elapsedSeconds)
    {
        Initialize();
        CompletionTime = elapsedSeconds;
        CurrentRank = CalculateRank(elapsedSeconds);

        if (titleText != null)
            titleText.text = completionTitle;
        if (timeText != null)
            timeText.text = IsValidTime(elapsedSeconds) ? $"{elapsedSeconds:00.0} seconds" : "-- seconds";
        if (rankText != null)
        {
            rankText.text = CurrentRank == CompletionRank.None ? unrankedLabel : CurrentRank.ToString();
            rankText.color = GetRankColor(CurrentRank);
        }

        isShowingCompletion = true;
        gameObject.SetActive(true);
        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    public CompletionRank CalculateRank(float elapsedSeconds)
    {
        if (!IsValidTime(elapsedSeconds))
            return CompletionRank.None;

        float aMaximum = Mathf.Max(0f, aRankTime);
        float bMaximum = Mathf.Max(aMaximum, bRankTime);
        float cMaximum = Mathf.Max(bMaximum, cRankTime);

        if (elapsedSeconds <= aMaximum)
            return CompletionRank.A;
        if (elapsedSeconds <= bMaximum)
            return CompletionRank.B;
        return elapsedSeconds <= cMaximum ? CompletionRank.C : CompletionRank.None;
    }

    public void Hide()
    {
        isShowingCompletion = false;
        if (completionPanel != null)
            completionPanel.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void Retry()
    {
        if (!IsVisible)
            return;

        Scene minigameScene = gameObject.scene;
        Hide();
        onRetry?.Invoke();

        if (!reloadSceneOnRetry)
            return;

        // A persistent UI has no scene path, so use the active gameplay scene.
        if (string.IsNullOrEmpty(minigameScene.path))
            minigameScene = SceneManager.GetActiveScene();

        Time.timeScale = 1f;
#if UNITY_EDITOR
        // Support playing a minigame scene directly before it is in the build list.
        if (minigameScene.buildIndex < 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                minigameScene.path, new LoadSceneParameters(LoadSceneMode.Single));
            return;
        }
#endif
        SceneManager.LoadScene(minigameScene.path);
    }

    public void Finish()
    {
        if (!IsVisible)
            return;

        Hide();

        // Directly opened minigame scenes may not have an active campaign.
        CampaignTracker campaignTracker = CampaignTracker.Instance;
        if (continueCampaignOnFinish && MatchFlow.Match is CampaignMatch &&
            !MatchFlow.Match.IsFinished && campaignTracker != null)
        {
            Time.timeScale = 1f;
            MatchFlow.Match.IsFinished = true;
            MatchFlow.Match.IsPlayerWinner = true;
            campaignTracker.HandleEndgame();
            campaignTracker.ContinueCampaign();
        }

        onFinish?.Invoke();
    }

    private void Initialize()
    {
        if (initialized)
            return;

        initialized = true;
        boundRetryButton = retryButton;
        boundFinishButton = finishButton;
        if (boundRetryButton != null)
            boundRetryButton.onClick.AddListener(Retry);
        if (boundFinishButton != null)
            boundFinishButton.onClick.AddListener(Finish);
    }

    private Color GetRankColor(CompletionRank rank)
    {
        switch (rank)
        {
            case CompletionRank.A: return aRankColor;
            case CompletionRank.B: return bRankColor;
            case CompletionRank.C: return cRankColor;
            default: return unrankedColor;
        }
    }

    private static bool IsValidTime(float seconds)
    {
        return seconds >= 0f && !float.IsNaN(seconds) && !float.IsInfinity(seconds);
    }
}
