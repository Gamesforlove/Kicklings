using System.Collections;
using System.Linq;
using EventBusSystem;
using Gameplay.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
{
    public class PlayerActions : MonoBehaviour
    {
        public bool DisableInput { get; set; }

        EntityData _entityData;
        int _kickingDirectionMultiplier = 1;

        const float DefaultJumpCooldown = 1f;
        bool _jumpOnCd;
        CountdownTimer _jumpCdTimer;
        bool _inputActionActive;
        bool _kickAssistAvailable;
        bool _assistCollidersExpanded;
        float _kickAssistTimeRemaining;
        
        [SerializeField] GameObject _kickingLeg;
        [SerializeField] Collider2D[] _kickingLegColliders;
        [SerializeField] GroundCheck[] _groundChecks;
    
        Rigidbody2D _rigidbody;
        HingeJoint2D _kickingLegJoint;
        JointMotor2D _kickingLegJointMotor;
        PlayerInput _playerInput;
        Collider2D[] _bodyColliders;
        Coroutine _stopKickingLegMotorCoroutine;
    
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _bodyColliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
            _kickingLegJoint = _kickingLeg.GetComponent<HingeJoint2D>();
            _kickingLegJointMotor = _kickingLegJoint.motor;
            _jumpCdTimer = new CountdownTimer(DefaultJumpCooldown);
            _jumpCdTimer.OnTimerStop += () => _jumpOnCd = false;
            DisableInput = false;
        }

        private void Start()
        {
            DisableInput = false;
        }

        void Update()
        {
            _jumpCdTimer.Tick(Time.deltaTime);

            if (!_kickAssistAvailable)
                return;

            _kickAssistTimeRemaining -= Time.deltaTime;
            if (_kickAssistTimeRemaining <= 0f)
                _kickAssistAvailable = false;
        }

        public void SetUp(EntityData entityData)
        {
            _entityData = entityData;
            _kickingDirectionMultiplier = transform.position.x > 0 ? -1 : 1;
            float jumpCooldown = _entityData.JumpCooldown > Mathf.Epsilon
                ? _entityData.JumpCooldown
                : DefaultJumpCooldown;
            _jumpCdTimer.Reset(jumpCooldown);
            ExpandAssistColliders();
        }

        public void OnKick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!CanReceivePlayerAction)
                    return;

                PlayersManager playersManager = PlayersManager.Instance;
                if (playersManager != null &&
                    !playersManager.TryBeginPlayerAction(this, CurrentControlScheme))
                    return;

                _inputActionActive = true;
                PerformAction(applyMovementAssist: true);
            }
            else if (context.canceled && _inputActionActive)
            {
                _inputActionActive = false;
                PlayersManager.Instance?.EndPlayerAction(this, CurrentControlScheme);
                OnActionCancelled();
            }
        }

        public bool CanKick { get; set; } = true;
        public bool ForcedToHoldKick { get; set; } = false;
        [field: SerializeField] public bool LockMovementToFacingDirection { get; set; } = false;
        public void ScriptedKick() => OnActionPerformed();
        public bool CanReceivePlayerAction => isActiveAndEnabled && CanKick && !DisableInput;
        public string CurrentControlScheme => _playerInput != null ? _playerInput.currentControlScheme : string.Empty;

        public void OnActionPerformed() => PerformAction(applyMovementAssist: false);

        void PerformAction(bool applyMovementAssist)
        {
            if (!CanKick)
                return;
            if (DisableInput)
                return;

            BeginKickAssistWindow();
            Kick();
            if (_groundChecks.Any(gc => gc.IsGrounded) && !_jumpOnCd)
                Jump(applyMovementAssist);
        }

        public void OnActionCancelled()
        {
            if (!CanKick)
                return;
            if (ForcedToHoldKick)
                StartCoroutine(forceKickToHold());
            else
                ReturnLeftLegToOriginalPosition();
        }
    
        void Jump(bool applyMovementAssist)
        {
            Vector2 jumpForce = transform.up * _entityData.JumpPower;
            if (applyMovementAssist)
                ApplyMovementAssist(ref jumpForce);

            if (LockMovementToFacingDirection && Mathf.Sign(jumpForce.x) != Mathf.Sign(_kickingDirectionMultiplier))
                jumpForce.x = 0f;
            _rigidbody.AddForce(jumpForce);
            EventBus<PlayerJumped>.Raise(new PlayerJumped());
            _jumpOnCd = true;
            _jumpCdTimer.Start();
        }

        void ApplyMovementAssist(ref Vector2 jumpForce)
        {
            EntityData.KickAssistSettings settings = _entityData?.KickAssist;
            BallScript ball = BallManager.Instance?.Ball;
            if (settings == null || ball == null || settings.MovementAssistRange <= 0f)
                return;

            Vector2 predictedBallPosition = (Vector2)ball.transform.position +
                                            ball.Rigidbody.linearVelocity * settings.BallPredictionTime;
            Vector2 toBall = predictedBallPosition - _rigidbody.position;
            if (toBall.sqrMagnitude > settings.MovementAssistRange * settings.MovementAssistRange)
                return;

            float horizontalOffset = Mathf.Clamp(toBall.x / settings.MovementAssistRange, -1f, 1f);
            jumpForce.x += horizontalOffset * _entityData.JumpPower * settings.MovementAssistStrength;

            if (settings.VerticalMovementAssistStrength <= 0f ||
                toBall.y <= settings.VerticalAssistMinimumBallHeight)
                return;

            float fullAssistHeight = Mathf.Max(
                settings.VerticalAssistMinimumBallHeight + Mathf.Epsilon,
                settings.VerticalAssistFullBallHeight
            );
            float verticalAssistWeight = Mathf.InverseLerp(
                settings.VerticalAssistMinimumBallHeight,
                fullAssistHeight,
                toBall.y
            );
            jumpForce.y += _entityData.JumpPower *
                           settings.VerticalMovementAssistStrength *
                           verticalAssistWeight;
        }

        public void Kick()
        {
            CancelKickingLegMotorStop();
            ApplyKickingPower(-1);
        }
        public void DisableKickingLeg()
        {
            foreach (Collider2D collider in _kickingLegColliders)
                collider.enabled = false;
        }
        public void EnableKickingLeg()
        {
            foreach (Collider2D collider in _kickingLegColliders)
                collider.enabled = true;
        }
        public void ReturnLeftLegToOriginalPosition()
        {
            ApplyKickingPower(1);
            ScheduleKickingLegMotorStop();
        }
        
        void ApplyKickingPower(float direction)
        {
            _kickingLegJointMotor.motorSpeed = _entityData.KickingPower * _kickingDirectionMultiplier * direction;
            _kickingLegJoint.motor = _kickingLegJointMotor;
        }

        void ScheduleKickingLegMotorStop()
        {
            CancelKickingLegMotorStop();
            EntityData.KickAssistSettings settings = _entityData?.KickAssist;
            if (settings == null || settings.KickingLegReturnMotorDuration <= 0f)
                return;

            _stopKickingLegMotorCoroutine = StartCoroutine(StopKickingLegMotorAtRest(settings));
        }

        IEnumerator StopKickingLegMotorAtRest(EntityData.KickAssistSettings settings)
        {
            float elapsed = 0f;
            while (elapsed < settings.KickingLegReturnMotorDuration &&
                   Mathf.Abs(_kickingLegJoint.jointAngle) > settings.KickingLegRestAngleTolerance)
            {
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            ApplyKickingPower(0f);
            _stopKickingLegMotorCoroutine = null;
        }

        void CancelKickingLegMotorStop()
        {
            if (_stopKickingLegMotorCoroutine == null)
                return;

            StopCoroutine(_stopKickingLegMotorCoroutine);
            _stopKickingLegMotorCoroutine = null;
        }

        void BeginKickAssistWindow()
        {
            EntityData.KickAssistSettings settings = _entityData?.KickAssist;
            _kickAssistTimeRemaining = settings != null ? settings.ContactAssistWindow : 0f;
            _kickAssistAvailable = _kickAssistTimeRemaining > 0f;
        }

        void ExpandAssistColliders()
        {
            if (_assistCollidersExpanded || _entityData?.KickAssist == null)
                return;

            EntityData.KickAssistSettings settings = _entityData.KickAssist;
            foreach (Collider2D bodyCollider in _bodyColliders)
            {
                if (bodyCollider is not BoxCollider2D boxCollider)
                    continue;

                switch (boxCollider.gameObject.name)
                {
                    case "Head":
                        ScaleCollider(boxCollider, settings.HeadColliderScale, scaleHorizontalOffset: false, scaleVerticalOffset: false);
                        break;
                    case "RightLeg":
                    case "LeftLeg":
                        // Keep the leg horizontally centered. Several kid rigs have a rearward X
                        // offset, and scaling that offset makes them balance on their heels.
                        ScaleCollider(boxCollider, settings.LegColliderScale, scaleHorizontalOffset: false, scaleVerticalOffset: true);
                        break;
                    case "Foot":
                        ScaleCollider(boxCollider, settings.FootColliderScale, scaleHorizontalOffset: true, scaleVerticalOffset: true);
                        break;
                }
            }

            _assistCollidersExpanded = true;
        }

        static void ScaleCollider(
            BoxCollider2D collider,
            float scale,
            bool scaleHorizontalOffset,
            bool scaleVerticalOffset
        )
        {
            scale = Mathf.Max(1f, scale);
            collider.size *= scale;
            collider.offset = new Vector2(
                scaleHorizontalOffset ? collider.offset.x * scale : collider.offset.x,
                scaleVerticalOffset ? collider.offset.y * scale : collider.offset.y
            );
        }

        public bool TryApplyBadKickRescue(Rigidbody2D ballRigidbody, Collider2D contactedBodyCollider)
        {
            EntityData.KickAssistSettings settings = _entityData?.KickAssist;
            if (!_kickAssistAvailable || settings == null || ballRigidbody == null ||
                ballRigidbody.bodyType != RigidbodyType2D.Dynamic)
                return false;

            // A single intentional action may contact through several colliders in one physics step.
            // Consume the window on the first body contact so the rescue can never stack.
            _kickAssistAvailable = false;

            Vector2 velocity = ballRigidbody.linearVelocity;
            float speed = velocity.magnitude;
            Vector2 forward = Vector2.right * _kickingDirectionMultiplier;
            float forwardVelocity = Vector2.Dot(velocity, forward);
            float contactAssistMultiplier = GetContactAssistMultiplier(contactedBodyCollider, settings);
            float requiredForwardVelocity = Mathf.Max(
                settings.MinimumForwardVelocity * contactAssistMultiplier,
                speed * settings.ForwardVelocityDeadZone
            );

            if (forwardVelocity >= requiredForwardVelocity)
                return false;

            float velocityToAdd = Mathf.Min(
                (requiredForwardVelocity - forwardVelocity) * settings.ForwardVelocityCorrectionStrength,
                settings.MaximumForwardVelocityAdded * contactAssistMultiplier
            );

            if (velocityToAdd <= Mathf.Epsilon)
                return false;

            // This runs inside the collision callback, before the next rendered frame. Preserve the
            // physics-generated vertical velocity and spin, and only add a capped forward component.
            ballRigidbody.linearVelocity = velocity + forward * velocityToAdd;
            return true;
        }

        float GetContactAssistMultiplier(
            Collider2D contactedBodyCollider,
            EntityData.KickAssistSettings settings
        )
        {
            if (contactedBodyCollider == null)
                return 1f;
            if (contactedBodyCollider.gameObject.name == "Head")
                return Mathf.Max(1f, settings.HeadContactAssistMultiplier);
            if (contactedBodyCollider.attachedRigidbody == _rigidbody)
                return Mathf.Max(1f, settings.TorsoContactAssistMultiplier);

            return 1f;
        }

        public float GetActionSelectionScore(Rigidbody2D ballRigidbody)
        {
            if (ballRigidbody == null)
                return 0f;

            float predictionTime = _entityData?.KickAssist?.BallPredictionTime ?? 0.35f;
            Vector2 predictedBallPosition = ballRigidbody.position +
                                            ballRigidbody.linearVelocity * predictionTime;
            float closestDistanceSquared = float.PositiveInfinity;

            foreach (Collider2D bodyCollider in _bodyColliders)
            {
                if (bodyCollider == null || !bodyCollider.enabled || bodyCollider.isTrigger)
                    continue;

                Vector2 closestPoint = bodyCollider.ClosestPoint(predictedBallPosition);
                closestDistanceSquared = Mathf.Min(
                    closestDistanceSquared,
                    (predictedBallPosition - closestPoint).sqrMagnitude
                );
            }

            if (float.IsPositiveInfinity(closestDistanceSquared))
                closestDistanceSquared = (predictedBallPosition - _rigidbody.position).sqrMagnitude;

            if (!_groundChecks.Any(gc => gc.IsGrounded))
                closestDistanceSquared += 0.25f;
            if (_jumpOnCd)
                closestDistanceSquared += 0.1f;

            return closestDistanceSquared;
        }

        public void ResetActionState()
        {
            CancelKickingLegMotorStop();
            if (_entityData != null)
                ApplyKickingPower(0f);
            _inputActionActive = false;
            _kickAssistAvailable = false;
            _kickAssistTimeRemaining = 0f;
        }

        // used to scripted events, tutorials, in-game cutscenes, etc.
        bool inforceKickToHoldRoutine = false;
        IEnumerator forceKickToHold()
        {
            if (inforceKickToHoldRoutine)
                yield break;
            inforceKickToHoldRoutine = true;
            bool prevCanKick = CanKick;
            CanKick = false;
            yield return new WaitForSeconds(0.7f);
            ReturnLeftLegToOriginalPosition();
            CanKick = prevCanKick;
            inforceKickToHoldRoutine = false;
        }
    }
}
