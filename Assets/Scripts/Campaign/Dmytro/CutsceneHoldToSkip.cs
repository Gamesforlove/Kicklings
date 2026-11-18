using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[DisallowMultipleComponent]
public sealed class CutsceneHoldToSkip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup visual;
    [SerializeField] private RectTransform progressBar;
    [SerializeField, Min(0.1f)] private float holdDuration = 2f;
    [SerializeField, Range(0f, 1f)] private float idleOpacity = 0.8f;

    private CampaignCutsceneSkip skip;
    private Mouse heldMouse;
    private int heldPointer;
    private float holdStartTime;
    private bool hovering;

    internal void Bind(CampaignCutsceneSkip owner) => skip = owner;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData is ExtendedPointerEventData { device: Mouse })
        {
            hovering = true;
            UpdateOpacity();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left ||
            eventData is not ExtendedPointerEventData { device: Mouse mouse } || skip == null)
            return;

        skip.SuppressDialogueClick();
        if (!skip.CanSkip || !Application.isFocused || heldMouse != null)
            return;

        heldMouse = mouse;
        heldPointer = eventData.pointerId;
        holdStartTime = Time.unscaledTime;
        UpdateOpacity();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.pointerId == heldPointer)
            CancelHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData is not ExtendedPointerEventData { device: Mouse })
            return;

        hovering = false;
        if (eventData.pointerId == heldPointer)
            CancelHold();
        else
            UpdateOpacity();
    }

    private void LateUpdate()
    {
        skip?.RestoreDialogueInput();
        if (heldMouse == null)
            return;

        if (!heldMouse.leftButton.isPressed || !Application.isFocused || skip == null || !skip.CanSkip)
        {
            CancelHold();
            return;
        }

        float progress = Mathf.Clamp01((Time.unscaledTime - holdStartTime) / holdDuration);
        SetProgress(progress);
        if (progress < 1f)
            return;

        CancelHold();
        if (skip.TrySkipEntireCutscene())
            gameObject.SetActive(false);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            ResetPointer();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            ResetPointer();
    }

    private void OnDisable()
    {
        skip?.RestoreDialogueInput();
        ResetPointer();
    }

    private void ResetPointer()
    {
        hovering = false;
        CancelHold();
    }

    private void CancelHold()
    {
        heldMouse = null;
        SetProgress(0f);
        UpdateOpacity();
    }

    private void SetProgress(float progress)
    {
        progressBar.localScale = new Vector3(progress, 1f, 1f);
    }

    private void UpdateOpacity()
    {
        visual.alpha = hovering || heldMouse != null ? 1f : idleOpacity;
    }
}
