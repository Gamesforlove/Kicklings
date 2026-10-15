using Gameplay.Managers;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Gameplay.Spawners;
using TMPro;
using UnityEngine.Serialization;

public class Stage0Scene2Tutorials : MonoBehaviour
{
    public static Stage0Scene2Tutorials Instance { get; private set; }

    [Header("Grandpa Dialogue")]
    [FormerlySerializedAs("_dialogueText")] public TextMeshProUGUI DialogueText;
    public string SpeakerName;
    public Color SpeakerColor = new Color32(255, 204, 102, 255);
    [FormerlySerializedAs("_kickPraise"), TextArea] public string KickPraise;
    [FormerlySerializedAs("_blockInstruction"), TextArea] public string BlockInstruction;
    [FormerlySerializedAs("_blockSuccess"), TextArea] public string BlockSuccess;
    [FormerlySerializedAs("_blockFailure"), TextArea] public string BlockFailure;

    public void ShowKickPraise(TutorialFade fade) => ShowGrandpaDialogue(KickPraise, fade);
    public void ShowBlockInstruction(TutorialFade fade) => ShowGrandpaDialogue(BlockInstruction, fade);
    public void ShowBlockSuccess(TutorialFade fade) => ShowGrandpaDialogue(BlockSuccess, fade);
    public void ShowBlockFailure(TutorialFade fade) => ShowGrandpaDialogue(BlockFailure, fade);

    void ShowGrandpaDialogue(string line, TutorialFade fade)
    {
        if (DialogueText == null)
            return;

        string speakerColor = ColorUtility.ToHtmlStringRGBA(SpeakerColor);
        DialogueText.text = $"<size=75%><color=#{speakerColor}>{SpeakerName}</color></size>\n<color=#FFFFFFFF>{line}</color>";
        fade?.FadeIn();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tutorial = TutorialState.KickTutorial;
        SetupForKickTutorial();
    }

    enum TutorialState { KickTutorial, BlockTutorial, GoalTutorial }
    TutorialState tutorial;
    public void NextTutorial()
    {
        if (tutorial == TutorialState.KickTutorial)
        {
            tutorial = TutorialState.BlockTutorial;
            SetupForBlockTutorial();
        }
        else if (tutorial == TutorialState.BlockTutorial)
        {
            tutorial = TutorialState.GoalTutorial;
            SetupForGoalTutorial();
        }
    }

    public UnityEvent KickTutorial;
    public UnityEvent BlockTutorial;
    public UnityEvent GoalTutorial;
    void SetupForKickTutorial()
    {
        KickTutorial?.Invoke();
    }

    void SetupForBlockTutorial()
    {
        BlockTutorial?.Invoke();
    }

    void SetupForGoalTutorial()
    {
        GoalTutorial?.Invoke();
    }
}
