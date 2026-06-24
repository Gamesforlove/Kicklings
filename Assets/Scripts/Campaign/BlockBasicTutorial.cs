using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Gameplay.Managers;
using Gameplay.CharacterComponents;

public class BlockBasicTutorial : MonoBehaviour
{
    [SerializeField] private TutorialFade TutorialFade;

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
        ToggleForceKickToHold(true);
        GoalsManager.Instance.SetCollidersEnabled(false);
        TimeScaleManager.SlowMotion();

        yield return new WaitForSecondsRealtime(3f);

        TimeScaleManager.SetGameplayTimeScale();
        PlayersManager.Instance?.ResetPlayers();

        BallScript ball = BallManager.Instance.Ball;
        if (ball == null)
            yield break;
        ball.Rigidbody.linearVelocity = Vector3.zero;
        ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        if (ballMoveScript == null)
            yield break;

        const float moveDuration = 3f;
        Vector2 startPoint = ball.transform.position;
        Vector2 endPoint = (Vector2)PlayersManager.Instance.Players[0].transform.position + Vector2.up * 0.5f;
        Vector2 midPoint = (Vector2.up * 3f) + ((startPoint + endPoint) / 2f);
        ballMoveScript.StartCurveMove(ball.transform, startPoint, midPoint, endPoint, moveDuration, stopDistance);
        yield return new WaitUntil(() => ballMoveScript.Moving);
        yield return new WaitForSeconds(1f);

        //yield return new WaitWhile(() => ballMoveScript.Moving);
        GoalsManager.Instance.SetCollidersEnabled(true);
        //TimeScaleManager.SlowMotion();

        const float slowdownSpeed = 0.94f;
        bool kicked = false;
        bool prompted = false;
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
                ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
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
            ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        yield return new WaitForSecondsRealtime(0.3f);
        ToggleForceKickToHold(false);
        onTutorialDone?.Invoke();
        TutorialFade?.FadeOut();

        const float speedup = 2f;
        while (Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime * speedup);
            yield return null;
        }
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
