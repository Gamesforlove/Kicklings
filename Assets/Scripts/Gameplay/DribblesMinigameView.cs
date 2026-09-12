using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class DribblesMinigameView : MonoBehaviour
{
    [Header("In-Game HUD")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject startPrompt;
    [SerializeField] private GameObject restartHint;

    [Header("Completion Panel")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TextMeshProUGUI completionTitleText;
    [SerializeField] private TextMeshProUGUI completionGradeText;
    [SerializeField] private TextMeshProUGUI completionTimeText;
    [SerializeField] private TextMeshProUGUI completionPraiseText;
    [SerializeField] private TextMeshProUGUI completionMessageText;

    [Header("Final Scoreboard")]
    [SerializeField] private GameObject finalScoreboard;
    [SerializeField] private TextMeshProUGUI finalCourseColumn;
    [SerializeField] private TextMeshProUGUI finalScoreColumn;
    [SerializeField] private TextMeshProUGUI finalTimeColumn;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionalButton;
    [SerializeField] private TextMeshProUGUI retryButtonText;
    [SerializeField] private TextMeshProUGUI continueButtonText;
    [SerializeField] private TextMeshProUGUI optionalButtonText;

    public TextMeshProUGUI TimerText => timerText;
    public GameObject StartPrompt => startPrompt;
    public GameObject RestartHint => restartHint;
    public GameObject CompletionPanel => completionPanel;
    public RectTransform CompletionPraiseRect => completionPraiseText.rectTransform;
    public RectTransform CompletionMessageRect => completionMessageText.rectTransform;
    public TextMeshProUGUI CompletionTitleText => completionTitleText;
    public TextMeshProUGUI CompletionGradeText => completionGradeText;
    public TextMeshProUGUI CompletionTimeText => completionTimeText;
    public TextMeshProUGUI CompletionPraiseText => completionPraiseText;
    public TextMeshProUGUI CompletionMessageText => completionMessageText;
    public GameObject FinalScoreboard => finalScoreboard;
    public TextMeshProUGUI FinalCourseColumn => finalCourseColumn;
    public TextMeshProUGUI FinalScoreColumn => finalScoreColumn;
    public TextMeshProUGUI FinalTimeColumn => finalTimeColumn;
    public Button RetryButton => retryButton;
    public Button ContinueButton => continueButton;
    public Button OptionalButton => optionalButton;
    public RectTransform RetryButtonRect => retryButton.GetComponent<RectTransform>();
    public RectTransform ContinueButtonRect => continueButton.GetComponent<RectTransform>();
    public RectTransform OptionalButtonRect => optionalButton.GetComponent<RectTransform>();
    public TextMeshProUGUI RetryButtonText => retryButtonText;
    public TextMeshProUGUI ContinueButtonText => continueButtonText;
    public TextMeshProUGUI OptionalButtonText => optionalButtonText;
}
