using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Gameplay.Managers;
using Gameplay.CharacterComponents;

public class BlockBasicTutorial : MonoBehaviour
{
    [SerializeField] private TutorialFade TutorialFade;
    [SerializeField] private TimedBallTutorial timingPractice;

    public UnityEvent onTutorialDone;

    List<PlayerActions> playerActions;
    public void StartTutorial()
    {
        StartCoroutine(tutorialRoutine());
    }

    KeyCode kick = KeyCode.Z;
    public float stopDistance;
    public MoveToPointCurved ballMoveScript;
    IEnumerator tutorialRoutine()
    {
        playerActions = PlayersManager.Instance?.GetPlayerActions();
        ToggleKickAllowed(false);
        //ToggleForceKickToHold(true);
        GoalsManager.Instance.SetCollidersEnabled(false);
        TimeScaleManager.SlowMotion();
        PlayersManager.Instance?.ResetPlayers();

        yield return null;
        yield return new WaitUntil(() => !Input.GetKey(kick));
        //PlayersManager.Instance?.ResetPlayers();

        BallScript ball = BallManager.Instance.Ball;
        if (ball == null)
            yield break;
        GrandpaStage0 grandpa = GrandpaStage0.Instance;
        if (ballMoveScript == null || grandpa == null)
            yield break;

        // Prepare a new block feed here; the preceding kick keeps its physical flight to the end.
        ballMoveScript.StopCurveMove();
        ball.ResetWithoutBlink();
        ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        ball.Rigidbody.position = (Vector2)grandpa.transform.position + Vector2.up * 2.2f;
        ball.Rigidbody.simulated = true;
        ball.Renderer.enabled = true;
        ball.Collider.enabled = true;

        ToggleKickAllowed(true);
        grandpa.DoScriptedKick();
        yield return null;
        ToggleKickAllowed(false);
        TimeScaleManager.SetGameplayTimeScale();

        const float moveDuration = 3f;
        Vector2 startPoint = ball.transform.position;
        PlayerActions mainPlayer = PlayersManager.Instance.Players[0].GetComponent<PlayerActions>();
        Collider2D headCollider = mainPlayer.HeadContactCollider;
        Vector2 endPoint = headCollider != null
            ? (Vector2)headCollider.bounds.center + Vector2.up * 0.5f
            : (Vector2)mainPlayer.transform.position + Vector2.up * 1.5f;
        Vector2 midPoint = (Vector2.up * 3f) + ((startPoint + endPoint) / 2f);
        ballMoveScript.StartCurveMove(ball.transform, startPoint, midPoint, endPoint, moveDuration, stopDistance);
        yield return new WaitForSeconds(1f);

        //yield return new WaitWhile(() => ballMoveScript.Moving);
        GoalsManager.Instance.SetCollidersEnabled(true);
        //TimeScaleManager.SlowMotion();

        const float slowdownSpeed = 0.94f;
        bool kicked = false;
        bool prompted = false;
        mainPlayer.PrepareTutorialHeadBlockHop();
        while (ball != null && /*!Input.GetKeyDown(KeyCode.Z) && *//*Vector2.Distance(ball.transform.position, transform.position) > stopDistance && */Time.timeScale > 0f)
        {
            if (Time.timeScale < 0.25f && !prompted)
            {
                ToggleKickAllowed(true);
                TutorialFade?.FadeIn();
                prompted = true;
            }

            if (prompted && (Input.GetKeyDown(kick) || Input.GetKey(kick)))
            {
                ReleaseScriptedBall(ball, mainPlayer);
                kicked = true;
                break;
            }
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 0f, Time.unscaledDeltaTime * slowdownSpeed);
            yield return null;
        }

        if (!kicked)
        {
            ToggleKickAllowed(true);
            TutorialFade?.FadeIn();
            Time.timeScale = 0f;
            yield return new WaitUntil(() => (Input.GetKeyDown(kick) || Input.GetKey(kick)));
            ReleaseScriptedBall(ball, mainPlayer);
        }

        yield return new WaitForSecondsRealtime(0.3f);
        ToggleForceKickToHold(false);
        TutorialFade?.FadeOut();

        if (timingPractice != null)
        {
            yield return timingPractice.Run(TimedBallTutorial.PracticeType.Block, TutorialFade, ballMoveScript);
            if (!timingPractice.Succeeded)
                yield break;
        }

        onTutorialDone?.Invoke();
    }

    void ReleaseScriptedBall(BallScript ball, PlayerActions mainPlayer)
    {
        ballMoveScript.StopCurveMove();
        ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Collider2D headCollider = mainPlayer.HeadContactCollider;
        Vector2 target = headCollider != null
            ? (Vector2)headCollider.bounds.center + Vector2.up * 0.5f
            : (Vector2)mainPlayer.transform.position + Vector2.up * 1.5f;
        const float flightTime = 0.45f;
        Vector2 gravity = Physics2D.gravity * ball.Rigidbody.gravityScale;
        ball.Rigidbody.linearVelocity = (target - ball.Rigidbody.position -
                                        0.5f * gravity * flightTime * flightTime) / flightTime;

        // If the control was enabled during this input frame, make sure its guided action runs once.
        if (!mainPlayer.IsWithinActionContactWindow)
            mainPlayer.OnActionPerformed();
        // A hop queued during the pause needs physics immediately, before the completion event.
        TimeScaleManager.SetGameplayTimeScale();
    }

    void ToggleKickAllowed(bool allowed)
    {
        if (playerActions != null)
        {
            foreach (PlayerActions playerAction in playerActions)
            {
                if (playerAction != null)
                    playerAction.CanKick = allowed;
            }
        }
    }

    void ToggleForceKickToHold(bool force)
    {
        if (playerActions != null)
        {
            foreach (PlayerActions playerAction in playerActions)
            {
                if (playerAction != null)
                    playerAction.ForcedToHoldKick = force;
            }
        }
    }

    void ScriptedKick()
    {
        if (playerActions != null)
        {
            foreach (PlayerActions playerAction in playerActions)
            {
                if (playerAction != null)
                    playerAction.ScriptedKick();
            }
        }
    }
}
