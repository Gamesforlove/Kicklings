using Gameplay.CharacterComponents;
using UnityEngine;

public class TutorialPlayerAssist : MonoBehaviour
{
    [SerializeField, Min(1f)] float _movementAssistMultiplier = 2f;
    [SerializeField, Min(1f)] float _kickRescueMultiplier = 2f;
    [SerializeField, Min(1f)] float _contactWindowMultiplier = 1.75f;

    PlayerActions _actions;
    EntityData _originalData;
    EntityData _practiceData;

    public void BeginPractice(PlayerActions actions)
    {
        EndPractice();
        if (!isActiveAndEnabled || actions == null || actions.Data == null)
            return;

        _actions = actions;
        _originalData = actions.Data;
        // Clone the settings so no shared character asset or other player is modified.
        _practiceData = Instantiate(_originalData);
        _practiceData.name = _originalData.name + " (Goal Tutorial Assist)";
        EntityData.KickAssistSettings assist = _practiceData.KickAssist;
        if (assist == null)
            assist = _practiceData.KickAssist = new EntityData.KickAssistSettings();
        assist.MovementAssistStrength *= _movementAssistMultiplier;
        assist.FarMovementAssistStrength *= _movementAssistMultiplier;
        assist.UprightMovementAssistMultiplier = 1f;
        assist.WrongWayMovementDamping = 1f;
        assist.MinimumForwardVelocity *= _kickRescueMultiplier;
        assist.MaximumForwardVelocityAdded *= _kickRescueMultiplier;
        assist.ForwardVelocityCorrectionStrength = 1f;
        assist.ContactAssistWindow *= _contactWindowMultiplier;
        // This learner must cover the whole field without a teammate or goalkeeper behind them.
        assist.GoalkeeperSupportOffset = 0f;
        assist.GoalkeeperStayInOwnHalf = false;
        assist.TeammateSeparationDistance = 0f;
        _actions.ResetActionState();
        _actions.SetActionData(_practiceData);
    }

    public void EndPractice()
    {
        if (_actions != null && _practiceData != null && _actions.Data == _practiceData && _originalData != null)
        {
            _actions.ResetActionState();
            _actions.SetActionData(_originalData);
        }
        if (_practiceData != null)
            Destroy(_practiceData);
        _practiceData = null;
        _originalData = null;
        _actions = null;
    }

    void OnDisable() => EndPractice();
}
