using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class KickTutorial : MonoBehaviour
{
    [SerializeField] private float InitialDelay;

    public UnityEvent onTutorialDone;

    void Start()
    {
        StartCoroutine(kickTutorialRoutine());
    }

    IEnumerator kickTutorialRoutine()
    {
        yield return new WaitForSeconds(InitialDelay);
        while(Time.timeScale > 0.1f)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 0.1f, Time.unscaledDeltaTime * 0.8f);
            yield return null;
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        onTutorialDone?.Invoke();

        while (Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime * 2f);
            yield return null;
        }
    }
}
