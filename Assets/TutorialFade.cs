using UnityEngine;
using System.Collections;
public class TutorialFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void FadeIn() => StartCoroutine(fadeInRoutine());
    IEnumerator fadeInRoutine()
    {
        while (canvasGroup != null && canvasGroup.alpha < 1f && !fadingOut)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 1.2f);
            yield return null;
        }
    }

    public void FadeOut() => StartCoroutine(fadeOutRoutine());

    bool fadingOut = false;
    IEnumerator fadeOutRoutine()
    {
        fadingOut = true;
        while (canvasGroup != null && canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.unscaledDeltaTime * 1.2f);
            yield return null;
        }
    }
}
