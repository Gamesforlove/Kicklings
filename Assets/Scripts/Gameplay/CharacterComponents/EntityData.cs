using System;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    [CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
    public class EntityData : ScriptableObject
    {
        [Serializable]
        public class KickAssistSettings
        {
            [Tooltip("How far ahead, in seconds, the ball position is predicted for player selection and movement assist.")]
            [Min(0f)] public float BallPredictionTime = 0.35f;
            [Tooltip("Maximum distance at which a player action receives a horizontal nudge toward the predicted ball position.")]
            [Min(0f)] public float MovementAssistRange = 3f;
            [Tooltip("Maximum horizontal movement correction as a proportion of jump power.")]
            [Range(0f, 0.5f)] public float MovementAssistStrength = 0.12f;
            [Tooltip("Additional world-up jump force, as a proportion of jump power, when the predicted ball is above the character. Zero disables it.")]
            [Range(0f, 0.5f)] public float VerticalMovementAssistStrength = 0f;
            [Tooltip("Ball height above the character at which vertical reach assistance begins.")]
            [Min(0f)] public float VerticalAssistMinimumBallHeight = 0.8f;
            [Tooltip("Ball height above the character at which the full vertical reach assistance is applied.")]
            [Min(0f)] public float VerticalAssistFullBallHeight = 2.5f;
            [Tooltip("Time after an action starts during which the first ball contact can receive bad-kick rescue.")]
            [Min(0f)] public float ContactAssistWindow = 0.45f;
            [Tooltip("A kick whose forward velocity is at least this proportion of its total speed is left untouched.")]
            [Range(0f, 1f)] public float ForwardVelocityDeadZone = 0.35f;
            [Tooltip("Soft minimum forward ball velocity used only when a kick falls outside the dead zone.")]
            [Min(0f)] public float MinimumForwardVelocity = 2.4f;
            [Tooltip("How strongly a bad kick is moved toward the soft minimum forward velocity.")]
            [Range(0f, 1f)] public float ForwardVelocityCorrectionStrength = 0.65f;
            [Tooltip("Maximum velocity that bad-kick rescue can add in the team's forward direction.")]
            [Min(0f)] public float MaximumForwardVelocityAdded = 2.2f;
            [Tooltip("Additional bad-kick rescue applied when the intentional contact comes from the head.")]
            [Min(1f)] public float HeadContactAssistMultiplier = 1f;
            [Tooltip("Additional bad-kick rescue applied when the intentional contact comes from the root torso collider.")]
            [Min(1f)] public float TorsoContactAssistMultiplier = 1f;
            [Tooltip("Maximum time the return motor may run before it is stopped at rest. Zero preserves the legacy continuously-driven motor.")]
            [Min(0f)] public float KickingLegReturnMotorDuration = 0f;
            [Tooltip("The return motor stops early when the leg is this close to its zero-angle resting pose.")]
            [Min(0f)] public float KickingLegRestAngleTolerance = 4f;
            [Tooltip("Runtime size multiplier for physical head colliders. This does not change visuals.")]
            [Range(1f, 1.25f)] public float HeadColliderScale = 1.08f;
            [Tooltip("Runtime size multiplier for physical upper/lower leg colliders. This does not change visuals.")]
            [Range(1f, 1.25f)] public float LegColliderScale = 1.1f;
            [Tooltip("Runtime size multiplier for existing physical foot colliders. This does not change visuals.")]
            [Range(1f, 1.25f)] public float FootColliderScale = 1.12f;
        }

        [Tooltip( "Amount of force applied to the character when jumping, the higher the value, the higher the jump" )]
        public float JumpPower = 3000f;
        [Tooltip("Minimum time between grounded jumps. Lower values make a missed input recover sooner without increasing jump force.")]
        [Min(0f)] public float JumpCooldown = 1f;
        [Tooltip( "Amount of force applied to the character when kicking, the higher the value, the harder the kick" )]
        public float KickingPower = 800f;
        [Tooltip( "Amount of stabilization force applied to the character, the higher the value, the more stiff the character will be" )]
        public float StabilizationFactor = 35f;
        [Tooltip("Grounded rotations inside this angle are treated as upright so stabilization does not chatter around zero.")]
        [Min(0f)] public float StabilizationDeadZoneDegrees = 0f;
        [Tooltip("When grounded and upright, total root velocity below this value is settled to zero. Zero disables settling.")]
        [Min(0f)] public float GroundedLinearVelocityDeadZone = 0f;
        [Tooltip("When grounded and upright, root angular velocity below this value is settled to zero. Zero disables settling.")]
        [Min(0f)] public float GroundedAngularVelocityDeadZone = 0f;
        [Tooltip("Subtle soccer-only assistance. Good kicks inside the forward dead zone are not modified.")]
        public KickAssistSettings KickAssist = new();
    }
}
