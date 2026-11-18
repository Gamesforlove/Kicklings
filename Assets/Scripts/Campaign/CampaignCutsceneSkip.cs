using Scene_Management;
using UnityEngine;
using UnityEngine.UI;

internal sealed class CampaignCutsceneSkip
{
    private const string PrefabPath = "Prefabs/UI/Cutscene Hold To Skip";

    private readonly CampaignCutsceneController _controller;
    private readonly CutsceneBuilder _builder;
    private CutsceneHoldToSkip _control;
    private Button _sceneExit;
    private bool _skipRequested;
    private ProcessCutsceneInput _suppressedInput;
    private TextboxTextAnimation _suppressedText;

    internal bool CanSkip => !_skipRequested && _sceneExit != null &&
        _builder != null && _builder.isActiveAndEnabled &&
        CampaignTracker.Instance != null && !CampaignTracker.Instance.IsTransitioning &&
        MatchFlow.Match is CampaignMatch;

    private CampaignCutsceneSkip(CampaignCutsceneController controller, CutsceneBuilder builder)
    {
        _controller = controller;
        _builder = builder;
    }

    internal static CampaignCutsceneSkip Attach(CampaignCutsceneController controller, CutsceneBuilder builder)
    {
        if (builder == null)
            return null;

        var skip = new CampaignCutsceneSkip(controller, builder);
        skip.Enable();
        return skip;
    }

    internal void Enable()
    {
        if (_control != null || _skipRequested || _controller.NextSceneButton == null)
            return;

        Canvas canvas = _controller.NextSceneButton.GetComponentInParent<Canvas>(true);
        _sceneExit = FindSceneExit();
        if (canvas == null || _sceneExit == null)
            return;

        GameObject prefab = Resources.Load<GameObject>(PrefabPath);
        if (prefab == null)
            return;

        _control = Object.Instantiate(prefab, canvas.transform, false).GetComponent<CutsceneHoldToSkip>();
        _control.Bind(this);
    }

    internal void Disable()
    {
        RestoreDialogueInput();
        if (_control != null)
        {
            _control.gameObject.SetActive(false);
            Object.Destroy(_control.gameObject);
        }
        _control = null;
    }

    private Button FindSceneExit()
    {
        Button localExit = GetSceneExit(_controller);
        if (localExit != null)
            return localExit;

        // The opening scene keeps its final exit in the second, initially inactive segment.
        foreach (GameObject root in _controller.gameObject.scene.GetRootGameObjects())
        {
            foreach (CampaignCutsceneController segment in root.GetComponentsInChildren<CampaignCutsceneController>(true))
            {
                Button exit = GetSceneExit(segment);
                if (exit != null)
                    return exit;
            }
        }
        return null;
    }

    private static Button GetSceneExit(CampaignCutsceneController segment)
    {
        if (segment.NextSceneButton == null || !segment.NextSceneButton.TryGetComponent(out Button button))
            return null;

        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            Object target = button.onClick.GetPersistentTarget(i);
            string method = button.onClick.GetPersistentMethodName(i);
            if ((target is CampaignCutsceneController && method == nameof(CampaignCutsceneController.GoToNextScene)) ||
                (target is ReturnToMenuButton && method == nameof(ReturnToMenuButton.ReturnToMap)))
                return button;
        }
        return null;
    }

    internal void SuppressDialogueClick()
    {
        if (_suppressedInput != null)
            return;

        _suppressedInput = FindDialogueInput();
        if (_suppressedInput == null)
            return;

        // The package polls anyKeyDown after Update, so a UI event alone cannot consume the mouse press.
        _suppressedText = _suppressedInput.textAnimation;
        _suppressedInput.textAnimation = null;
    }

    private ProcessCutsceneInput FindDialogueInput()
    {
        foreach (ProcessCutsceneInput input in Object.FindObjectsByType<ProcessCutsceneInput>(FindObjectsSortMode.None))
        {
            if (input.isActiveAndEnabled && input.gameObject.scene == _controller.gameObject.scene && input.textAnimation != null)
                return input;
        }
        return null;
    }

    internal void RestoreDialogueInput()
    {
        if (_suppressedInput != null)
            _suppressedInput.textAnimation = _suppressedText;
        _suppressedInput = null;
        _suppressedText = null;
    }

    internal bool TrySkipEntireCutscene()
    {
        if (!CanSkip)
            return false;

        _skipRequested = true;
        ProcessCutsceneInput input = FindDialogueInput();
        if (input != null)
        {
            TextboxTextAnimation text = input.textAnimation;
            input.enabled = false;
            text.enabled = false;
            if (text.TryGetComponent(out AudioSource voice))
                voice.Stop();
        }

        // The campaign transition owns the exit. Disabling the builder also cancels its pending
        // actions and music if the player skips during an actor animation.
        _builder.StopAllCoroutines();
        _builder.enabled = false;
        _sceneExit.onClick.Invoke();
        return true;
    }
}
