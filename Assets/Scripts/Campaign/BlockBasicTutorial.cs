using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BlockBasicTutorial : MonoBehaviour
{
    [SerializeField] private TutorialFade TutorialFade;
    [SerializeField] private TimedBallTutorial timingPractice;

    public UnityEvent onTutorialDone;

    Coroutine _tutorialCoroutine;
    public void StartTutorial()
    {
        if (_tutorialCoroutine != null)
            return;

        if (timingPractice == null)
        {
            Debug.LogError("The block lesson needs its timing practice component.", this);
            return;
        }

        // Place the ball at Grandpa before the coroutine's first frame of waiting.
        if (timingPractice.PrepareBlockTransition())
        {
            if (Stage0Scene2Tutorials.Instance != null)
                Stage0Scene2Tutorials.Instance.ShowBlockInstruction(TutorialFade);
            _tutorialCoroutine = StartCoroutine(tutorialRoutine());
        }
    }

    public float stopDistance;
    public MoveToPointCurved ballMoveScript;
    IEnumerator tutorialRoutine()
    {
        // The original paused/scripted block lesson is disabled; this step is timing practice only.
        yield return null;
        if (timingPractice == null)
        {
            Debug.LogError("The block lesson needs its timing practice component.", this);
            _tutorialCoroutine = null;
            yield break;
        }

        try
        {
            yield return timingPractice.Run(TimedBallTutorial.PracticeType.Block, TutorialFade, ballMoveScript);
            if (timingPractice.Succeeded)
                onTutorialDone?.Invoke();
        }
        finally
        {
            _tutorialCoroutine = null;
        }
    }

    void OnDisable()
    {
        if (_tutorialCoroutine != null)
            StopCoroutine(_tutorialCoroutine);
        _tutorialCoroutine = null;
        timingPractice?.CancelBlockPractice();
    }
}
