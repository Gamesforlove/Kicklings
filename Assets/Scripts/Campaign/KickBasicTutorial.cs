using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Gameplay.Managers;
using Gameplay.CharacterComponents;

public class KickBasicTutorial : MonoBehaviour
{
    [SerializeField] private float InitialDelay;
    [SerializeField] private TutorialFade TutorialFade;

    public UnityEvent onTutorialDone;

    List<PlayerActions> playerActions;
    public void StartTutorial()
    {
        StartCoroutine(tutorialRoutine());
    }

    KeyCode kick = KeyCode.Z;
    public float stopDistance;
    public Transform ballDropTarget;
    public MoveToPointCurved ballMoveScript;
    private const float headHeightOffset = 2.2f;
    public float curveMidpointHeightOffset = 3f;
    public float curveMoveDuration = 1.5f;
    IEnumerator tutorialRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        playerActions = PlayersManager.Instance?.GetPlayerActions();
        yield return null;
        ToggleKickAllowed(false);
        ToggleForceKickToHold(true);
        if (InitialDelay > 0)
            yield return new WaitForSeconds(InitialDelay);

        BallScript ball = BallManager.Instance?.Ball;
        bool kicked = false;
        bool prompted = false;
        float startDistance = distance();
        while (ball != null && /*!Input.GetKeyDown(KeyCode.Z) && *//*Vector2.Distance(ball.transform.position, ballDropTarget.position) > stopDistance && */Time.timeScale > 0f)
        {
            if (Time.timeScale < 0.13f && !prompted)
            {
                ToggleKickAllowed(true);
                TutorialFade?.FadeIn();
                prompted = true;
            }

            if (prompted && (Input.GetKeyDown(kick) || Input.GetKey(kick)))
            {
                ToggleForceKickToHold(true);
                kicked = true;
                break;
            }
            Time.timeScale = Mathf.Lerp(1f, 0f, 1f - (distance() / startDistance));
            yield return null;
        }

        float distance() => Vector2.Distance(ball.transform.position, ballDropTarget.position) - stopDistance;

        if (!kicked)
        {
            ToggleForceKickToHold(true);
            ToggleKickAllowed(true);
            TutorialFade?.FadeIn();
            yield return new WaitUntil(() => (Input.GetKeyDown(kick) || Input.GetKey(kick)));
        }

        yield return new WaitForSecondsRealtime(0.3f);
        ToggleForceKickToHold(false);
        ToggleKickAllowed(false);
        TutorialFade?.FadeOut();

        if (ball != null && ballMoveScript != null && GrandpaStage0.Instance != null)
        {
            ball.Rigidbody.linearVelocity = Vector2.zero;
            ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            ball.Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;

            Vector2 startPoint = ball.transform.position;
            Vector2 endPoint = (Vector2)GrandpaStage0.Instance.transform.position + Vector2.up * headHeightOffset;
            Vector2 midPoint = new Vector2((startPoint.x + endPoint.x) * 0.5f, Mathf.Max(startPoint.y, endPoint.y) + curveMidpointHeightOffset);

            ballMoveScript.StartCurveMove(ball.transform, startPoint, midPoint, endPoint, curveMoveDuration, stopDistance);
            yield return new WaitUntil(() => ballMoveScript.Moving);
            
            float totalDistance = Vector2.Distance(startPoint, endPoint);
            yield return null;
            while (ballMoveScript.Moving)
            {
                float remaining = Vector2.Distance(ball.transform.position, endPoint);
                float t = remaining / totalDistance;
                if (t > 0.5f)
                    Time.timeScale = Mathf.Lerp(1f, 0.05f, (1f - t));
                yield return null;
            }
            Time.timeScale = 0f;
        }

        onTutorialDone?.Invoke();
        Stage0Scene2Tutorials.Instance?.NextTutorial();

        //const float speedup = 2f;
        //while (Time.timeScale < 1f)
        //{
        //    Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime * speedup);
        //    yield return null;
        //}
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
