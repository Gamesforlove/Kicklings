using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Gameplay.Managers;
using Gameplay.CharacterComponents;

public class KickTutorial : MonoBehaviour
{
    [SerializeField] private float InitialDelay;
    [SerializeField] private TutorialFade TutorialFade;

    public UnityEvent onTutorialDone;

    List<PlayerActions> playerActions;
    void Start()
    {
        StartCoroutine(kickTutorialRoutine());
    }

    KeyCode kick = KeyCode.Z;
    IEnumerator kickTutorialRoutine()
    {
        yield return null;
        playerActions = PlayersManager.Instance?.GetPlayerActions();
        ToggleKickAllowed(false);
        ToggleForceKickToHold(true);
        yield return new WaitForSeconds(InitialDelay);
        BallScript ball = BallManager.Instance?.Ball;
        const float dist = 0.6f;
        const float slowdownSpeed = 0.65f;
        bool kicked = false;
        while (ball != null && /*!Input.GetKeyDown(KeyCode.Z) && */Vector2.Distance(ball.transform.position, transform.position) > dist && Time.timeScale > 0f)
        {
            if (Time.timeScale < 0.05f && (Input.GetKeyDown(kick) || Input.GetKey(kick)))
            {
                ToggleKickAllowed(true);
                ScriptedKick();
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
            yield return new WaitUntil(() => (Input.GetKeyDown(kick) || Input.GetKey(kick)));
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
