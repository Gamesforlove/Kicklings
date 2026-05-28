using Gameplay.Managers;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Gameplay.Spawners;

public class Stage0Scene2Tutorials : MonoBehaviour
{
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
