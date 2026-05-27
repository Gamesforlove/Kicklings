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
        StartCoroutine(SetupForBlockTutorialCoroutine());
    }

    void SetupForGoalTutorial()
    {
        GoalTutorial?.Invoke();
    }

    public MoveToPointCurved ballMoveScript;
    public KickBlockTutorial blockTutorial;
    IEnumerator SetupForBlockTutorialCoroutine()
    {
        GoalsManager.Instance.SetCollidersEnabled(false);
        TimeScaleManager.SlowMotion();

        yield return new WaitForSecondsRealtime(3f);

        TimeScaleManager.SetGameplayTimeScale();
        PlayersManager.Instance?.ResetPlayers();
        BlockTutorial?.Invoke();

        BallScript ball = BallManager.Instance.Ball;
        if (ball == null)
            yield break;
        ball.Rigidbody.linearVelocity = Vector3.zero;
        ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        if (ballMoveScript == null)
            yield break;

        Vector2 startPoint = ball.transform.position;
        Vector2 endPoint = PlayersManager.Instance.Players[0].transform.position;
        Vector2 midPoint = (Vector2.up * 3f) + ((startPoint + endPoint) / 2f);
        ballMoveScript.StartCurveMove(ball.transform, startPoint, midPoint, endPoint, 2f);
        yield return new WaitUntil(() => ballMoveScript.Moving);
        yield return new WaitForSeconds(ballMoveScript.duration / 2f);

        blockTutorial.StartTutorial();
        yield return new WaitWhile(() => ballMoveScript.Moving);
        ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        GoalsManager.Instance.SetCollidersEnabled(true);
        TimeScaleManager.SlowMotion();
    }
}
