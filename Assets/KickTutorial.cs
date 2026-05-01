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
        playerActions = PlayersManager.Instance?.GetPlayerActions();
    }

    IEnumerator kickTutorialRoutine()
    {
        yield return null;
        ToggleKickAllowed(false);
        yield return new WaitForSeconds(InitialDelay);
        BallScript ball = BallManager.Instance?.Ball;
        const float dist = 0.6f;
        while (ball != null && !Input.GetKeyDown(KeyCode.Z) && Vector2.Distance(ball.transform.position, transform.position) > dist)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 0f, Time.unscaledDeltaTime * 0.65f);
            yield return null;
        }

        TutorialFade?.FadeIn();
        ToggleKickAllowed(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        onTutorialDone?.Invoke();
        TutorialFade?.FadeOut();

        while (Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime * 2f);
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
}
