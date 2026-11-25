using System.Linq;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class StabilizeComponent : MonoBehaviour
    {
        const float MaxForwardRotation = 100f;
        const float RotationFactor = 10f;
        const float FallAngle = 55f;
        const float FallSettledAngle = 2f;
        const float FallSettledAngularSpeed = 5f;
        const float IdleSwingPeriod = 2f;
        const float IdleSwingBrakeTime = 0.15f;
        const float MaxIdleSpeed = 0.6f;
        
        EntityData _entityData;
        
        [SerializeField] GroundCheck[] _groundChecks;
        [Tooltip("Solid body contact with these layers also supports upright recovery, at any tilt.")]
        [SerializeField] LayerMask _bodySupportLayers;
        [Tooltip("Seconds of torso rotation to anticipate when righting a fallen character on the ground. Zero disables fall braking.")]
        [SerializeField, Min(0f)] float _fallRecoveryBrakeTime;
        [Tooltip("Target lean to either side, in degrees, while grounded between kicks.")]
        [SerializeField, Range(0f, 10f)] float _idleSwingDegrees;

        Rigidbody2D _rigidBody;
        PlayerActions _playerActions;
        Collider2D[] _solidBodyColliders;
        ContactFilter2D _bodySupportFilter;
        bool _recoveringFromFall;
        float _idleSwingPhase;

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerActions = GetComponent<PlayerActions>();
            if (_idleSwingDegrees > 0f)
                _idleSwingPhase = Random.Range(0f, 2f * Mathf.PI);
            if (_bodySupportLayers.value != 0)
            {
                _bodySupportFilter.SetLayerMask(_bodySupportLayers);
                _bodySupportFilter.useTriggers = false;
                _solidBodyColliders = GetComponentsInChildren<Collider2D>(true)
                    .Where(collider => !collider.isTrigger && collider.attachedRigidbody != null)
                    .ToArray();
            }
        }

        void FixedUpdate()
        {
            _rigidBody.angularVelocity = Mathf.Clamp(_rigidBody.angularVelocity, -40f, 40f);
            if (_entityData == null)
                return;

            float tilt = Mathf.DeltaAngle(0f, _rigidBody.rotation);
            bool upright = Mathf.Abs(tilt) <= _entityData.StabilizationDeadZoneDegrees;
            bool bodySupported = false;
            if (_fallRecoveryBrakeTime > 0f)
            {
                // CPU rigs can rock freely at rest and still finish a get-up.
                if (Mathf.Abs(tilt) <= FallSettledAngle && Mathf.Abs(_rigidBody.angularVelocity) <= FallSettledAngularSpeed)
                    _recoveringFromFall = false;

                if ((_recoveringFromFall || Mathf.Abs(tilt) >= FallAngle) && HasBodySupport())
                {
                    _recoveringFromFall = true;
                    // Brake before crossing upright, even while the torso is inside the angle dead zone.
                    StabilizeRotation(tilt + _rigidBody.angularVelocity * _fallRecoveryBrakeTime);
                    return;
                }
            }
            else
            {
                bodySupported = HasBodySupport();
            }

            if (!bodySupported && !_groundChecks.Any(check => check.IsGrounded))
                return;

            bool swinging = CanSwingAtRest(tilt);
            if (upright)
                SettleGroundedMotion(stopSlowRotation: !swinging);

            if (swinging)
            {
                float phase = Time.fixedTime * (2f * Mathf.PI / IdleSwingPeriod) + _idleSwingPhase;
                float targetTilt = _idleSwingDegrees * Mathf.Sin(phase);
                StabilizeRotation(tilt - targetTilt + _rigidBody.angularVelocity * IdleSwingBrakeTime);
            }
            else if (!upright && (bodySupported || Mathf.Abs(tilt) < MaxForwardRotation))
                StabilizeRotation(tilt);
        }

        void OnDisable() => _recoveringFromFall = false;

        public void SetUp(EntityData entityData)
        {
            _entityData = entityData;
        }

        bool CanSwingAtRest(float tilt)
        {
            return _idleSwingDegrees > 0f && !_recoveringFromFall && Mathf.Abs(tilt) <= 2f * _idleSwingDegrees &&
                   _rigidBody.linearVelocity.sqrMagnitude <= MaxIdleSpeed * MaxIdleSpeed &&
                   _playerActions != null && _playerActions.CanReceivePlayerAction && _playerActions.IsBetweenKicks &&
                   HasBodySupport();
        }

        bool HasBodySupport()
        {
            if (_solidBodyColliders == null) return false;

            foreach (var collider in _solidBodyColliders)
            {
                if (collider != null && collider.isActiveAndEnabled && collider.IsTouching(_bodySupportFilter))
                    return true;
            }
            return false;
        }

        void StabilizeRotation(float tilt)
        {
            float correction = Mathf.Clamp(tilt, -MaxForwardRotation, MaxForwardRotation);
            _rigidBody.AddTorque(-_entityData.StabilizationFactor * (correction / RotationFactor));
        }

        void SettleGroundedMotion(bool stopSlowRotation)
        {
            if (stopSlowRotation && _entityData.GroundedAngularVelocityDeadZone > 0f &&
                Mathf.Abs(_rigidBody.angularVelocity) <= _entityData.GroundedAngularVelocityDeadZone)
                _rigidBody.angularVelocity = 0f;

            bool kickHopActive = _entityData.MinimumKickHopSpeed > 0f &&
                                 _playerActions != null && _playerActions.IsWithinActionContactWindow;
            // Ground sensors can still overlap during takeoff. Do not settle an intentional hop.
            if (!kickHopActive && _entityData.GroundedLinearVelocityDeadZone > 0f &&
                _rigidBody.linearVelocity.sqrMagnitude <=
                _entityData.GroundedLinearVelocityDeadZone * _entityData.GroundedLinearVelocityDeadZone)
                _rigidBody.linearVelocity = Vector2.zero;
        }

    }
}
