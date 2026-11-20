using System.Collections;
using System.Linq;
using EventBusSystem;
using Gameplay.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using static Gameplay.Spawners.PlayersSpawner;

namespace Gameplay.CharacterComponents
{
    public class PlayerActions : MonoBehaviour
    {
        public bool DisableInput { get; set; }
        public EntityData Data => _entityData;

        public delegate void JumpForceModifier(ref Vector2 jumpForce);
        public event JumpForceModifier ModifyJumpForce;

        EntityData _entityData;
        int _kickingDirectionMultiplier = 1;

        const float DefaultJumpCooldown = 1f;
        float _pendingKickHopSpeed;
        bool _pendingKickHopEvent;
        bool _jumpOnCd;
        CountdownTimer _jumpCdTimer;
        bool _inputActionActive;
        bool _kickAssistAvailable;
        bool _assistCollidersExpanded;
        float _kickAssistTimeRemaining;
        float _lastActionTime = float.NegativeInfinity;
        bool _tutorialHeadBlockHopPending;
        double _headerTakeoffTime = double.NegativeInfinity;
        bool _leftGroundOnJump;
        internal bool IsOnHeaderJump => _headerTakeoffTime != double.NegativeInfinity &&
            (!_leftGroundOnJump || !_groundChecks.Any(check => check.IsGrounded));
        
        [SerializeField] GameObject _kickingLeg;
        [Tooltip("Gently bring the released leg back to its standing angle, including after a collision or reset.")]
        [SerializeField] bool _centerReleasedLeg;
        [SerializeField] Collider2D[] _kickingLegColliders;
        [SerializeField] GroundCheck[] _groundChecks;
    
        Rigidbody2D _rigidbody;
        HingeJoint2D _kickingLegJoint;
        JointMotor2D _kickingLegJointMotor;
        bool _legAtRest = true;
        float? _heldKickAngle;
        float _fullKickTimeRemaining;
        PlayerInput _playerInput;
        Entity _entity;
        Collider2D[] _bodyColliders;
        Rigidbody2D[] _bodyRigidbodies;
        Coroutine _stopKickingLegMotorCoroutine;
        Coroutine _forceKickToHoldCoroutine;
        Coroutine _tutorialKickFollowThroughCoroutine;
    
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _entity = GetComponent<Entity>();
            _bodyColliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
            _bodyRigidbodies = GetComponentsInChildren<Rigidbody2D>(includeInactive: true);
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

        void FixedUpdate()
        {
            if (_headerTakeoffTime != double.NegativeInfinity)
            {
                if (!_groundChecks.Any(check => check.IsGrounded))
                    _leftGroundOnJump = true;
                else if (_leftGroundOnJump || (Time.fixedTimeAsDouble > _headerTakeoffTime + Time.fixedDeltaTime &&
                         _rigidbody.linearVelocityY <= 0f))
                    _headerTakeoffTime = double.NegativeInfinity;
            }

            if (_heldKickAngle.HasValue)
                UpdateHeldKick(_heldKickAngle.Value);
            else if (_centerReleasedLeg && _legAtRest && _entityData != null)
                HoldLegAtAngle(0f);

            float minimumHopSpeed = _pendingKickHopSpeed;
            bool raiseJumpEvent = _pendingKickHopEvent;
            _pendingKickHopSpeed = 0f;
            _pendingKickHopEvent = false;
            if (minimumHopSpeed > 0f)
                ApplyKickHop(minimumHopSpeed, raiseJumpEvent);
        }

        void OnDisable()
        {
            _headerTakeoffTime = double.NegativeInfinity;
            if (_centerReleasedLeg || _heldKickAngle.HasValue)
            {
                CancelKickingLegMotorStop();
                _heldKickAngle = null;
                _legAtRest = true;
                if (_kickingLegJoint != null)
                    HoldLegAtAngle(_kickingLegJoint.jointAngle);
            }
            _pendingKickHopSpeed = 0f;
            _pendingKickHopEvent = false;
        }

        public void SetUp(EntityData entityData)
        {
            SetActionData(entityData);
            _kickingDirectionMultiplier = transform.position.x > 0 ? -1 : 1;
            ExpandAssistColliders();
        }

        public void SetActionData(EntityData entityData)
        {
            _entityData = entityData;
            float jumpCooldown = _entityData.JumpCooldown > Mathf.Epsilon
                ? _entityData.JumpCooldown
                : DefaultJumpCooldown;
            _jumpCdTimer.Reset(jumpCooldown);
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
        public void ScriptedKick(float jumpForceMultiplier) =>
            PerformAction(applyMovementAssist: false, jumpForceMultiplier: Mathf.Clamp01(jumpForceMultiplier));
        public bool CanReceivePlayerAction => isActiveAndEnabled && CanKick && !DisableInput;
        public string CurrentControlScheme => _playerInput != null ? _playerInput.currentControlScheme : string.Empty;
        public int AttackingDirection => _kickingDirectionMultiplier;
        internal double LastKickTime { get; private set; } = double.NegativeInfinity;
        public bool IsWithinActionContactWindow => Time.time - _lastActionTime <=
            (_entityData?.KickAssist?.ContactAssistWindow ?? 0.45f);
        internal bool IsBetweenKicks => _legAtRest && !IsWithinActionContactWindow;
        public Collider2D KickingContactCollider => _kickingLeg.GetComponentsInChildren<Collider2D>()
            .FirstOrDefault(collider => collider.enabled && !collider.isTrigger);
        public Collider2D HeadContactCollider => _bodyColliders
            .FirstOrDefault(collider => collider.gameObject.name == "Head" && !collider.isTrigger);

        public bool IsKickingContact(Collider2D collider) => collider != null &&
            collider.transform.IsChildOf(_kickingLeg.transform);

        internal bool IsSwingingKickOutward => _kickingLegJoint.jointSpeed * _kickingDirectionMultiplier < -1f;

        // The paused introductory block can accept input before grounded physics refreshes.
        public void PrepareTutorialHeadBlockHop() => _tutorialHeadBlockHopPending = true;

        public void OnActionPerformed() => PerformAction(applyMovementAssist: false);

        public void PerformAssistedAction(Vector2 movementTarget) =>
            PerformAction(applyMovementAssist: true, movementTarget: movementTarget);

        void PerformAction(bool applyMovementAssist, float jumpForceMultiplier = 1f,
            Vector2? movementTarget = null)
        {
            if (!CanKick)
                return;
            if (DisableInput)
                return;

            bool tutorialHeadBlockHop = _tutorialHeadBlockHopPending;
            _tutorialHeadBlockHopPending = false;
            _lastActionTime = Time.time;
            BeginKickAssistWindow();
            Kick();
            bool grounded = _groundChecks.Any(gc => gc.IsGrounded);
            bool jumping = tutorialHeadBlockHop || (grounded && !_jumpOnCd);
            if (jumping)
                Jump(applyMovementAssist, tutorialHeadBlockHop, jumpForceMultiplier, movementTarget);

            // The minimum hop is independent of grounding, the full-jump cooldown and tutorial modifiers.
            float minimumHopSpeed = _entityData.MinimumKickHopSpeed * jumpForceMultiplier;
            if (minimumHopSpeed > 0f)
            {
                _pendingKickHopSpeed = Mathf.Max(_pendingKickHopSpeed, minimumHopSpeed);
                _pendingKickHopEvent |= grounded && !jumping;
            }
        }

        public void OnActionCancelled()
        {
            if (!CanKick)
                return;
            if (ForcedToHoldKick)
            {
                if (!inforceKickToHoldRoutine)
                    _forceKickToHoldCoroutine = StartCoroutine(forceKickToHold());
            }
            else
                ReturnLeftLegToOriginalPosition();
        }
    
        void Jump(bool applyMovementAssist, bool tutorialHeadBlockHop = false, float jumpForceMultiplier = 1f,
            Vector2? movementTarget = null)
        {
            Vector2 jumpForce = transform.up * _entityData.JumpPower;
            if (applyMovementAssist)
                ApplyMovementAssist(ref jumpForce, movementTarget);

            if (tutorialHeadBlockHop)
                jumpForce.y = Mathf.Max(jumpForce.y, _entityData.JumpPower * 1.25f);
            else if (applyMovementAssist)
                ModifyJumpForce?.Invoke(ref jumpForce);

            if (LockMovementToFacingDirection && Mathf.Sign(jumpForce.x) != Mathf.Sign(_kickingDirectionMultiplier))
                jumpForce.x = 0f;
            _rigidbody.AddForce(jumpForce * jumpForceMultiplier);
            if (jumpForce.y * jumpForceMultiplier > 0f)
                MarkHeaderTakeoff();
            EventBus<PlayerJumped>.Raise(new PlayerJumped());
            _jumpOnCd = true;
            _jumpCdTimer.Start();
        }

        void ApplyKickHop(float minimumHopSpeed, bool raiseJumpEvent)
        {
            float totalMass = 0f;
            float verticalMomentum = 0f;
            foreach (Rigidbody2D body in _bodyRigidbodies)
            {
                if (!IsKickHopBody(body))
                    continue;

                // Include the normal jump already queued for this step, so stronger jumps are not boosted.
                float verticalAcceleration = body.totalForce.y / body.mass +
                    Physics2D.gravity.y * body.gravityScale;
                float nextVerticalSpeed = body.linearVelocity.y + verticalAcceleration * Time.fixedDeltaTime;
                totalMass += body.mass;
                verticalMomentum += nextVerticalSpeed * body.mass;
            }
            if (totalMass <= 0f)
                return;

            float speedToAdd = minimumHopSpeed - verticalMomentum / totalMass;
            if (speedToAdd <= 0f)
                return;

            // Lift the connected body parts together instead of asking the torso to pull up the rig.
            foreach (Rigidbody2D body in _bodyRigidbodies)
            {
                if (IsKickHopBody(body))
                    body.AddForce(Vector2.up * (speedToAdd * body.mass), ForceMode2D.Impulse);
            }
            MarkHeaderTakeoff();
            if (raiseJumpEvent)
                EventBus<PlayerJumped>.Raise(new PlayerJumped());
        }

        void MarkHeaderTakeoff()
        {
            _headerTakeoffTime = Time.timeAsDouble;
            _leftGroundOnJump = !_groundChecks.Any(check => check.IsGrounded);
        }

        static bool IsKickHopBody(Rigidbody2D body) =>
            body != null && body.gameObject.activeInHierarchy && body.simulated &&
            body.bodyType == RigidbodyType2D.Dynamic;

        void ApplyMovementAssist(ref Vector2 jumpForce, Vector2? movementTarget = null)
        {
            EntityData.KickAssistSettings settings = _entityData?.KickAssist;
            BallScript ball = BallManager.Instance?.Ball;
            if (settings == null || ball == null || settings.MovementAssistRange <= 0f)
                return;

            Vector2 predictedBallPosition = movementTarget ?? ((Vector2)ball.transform.position +
                                            ball.Rigidbody.linearVelocity * settings.BallPredictionTime);
            Vector2 toBall = predictedBallPosition - _rigidbody.position;
            float nearRange = Mathf.Max(Mathf.Epsilon, settings.MovementAssistRange);

            if (Mathf.Abs(toBall.x) <= nearRange)
            {
                float horizontalOffset = Mathf.Clamp(toBall.x / nearRange, -1f, 1f);
                float requestedAssist = Mathf.Abs(horizontalOffset) *
                                        _entityData.JumpPower *
                                        settings.MovementAssistStrength;
                ApplyLeanRespectingHorizontalAssist(
                    ref jumpForce,
                    toBall.x,
                    requestedAssist,
                    settings
                );
                ApplyVerticalReachAssist(ref jumpForce, toBall.y, settings);
                return;
            }

            float targetX;
            if (movementTarget.HasValue)
                targetX = movementTarget.Value.x;
            else
            {
                PlayerActions teammate = PlayersManager.Instance?.GetTeammate(this);
                settings = GetFarTravelSettings(teammate, settings, out bool coversGoal);
                targetX = GetFarTravelTargetX(ball.Rigidbody, settings, teammate, coversGoal);
            }
            float horizontalDistance = targetX - _rigidbody.position.x;
            float fullAssistDistance = Mathf.Max(
                nearRange + Mathf.Epsilon,
                settings.FarMovementFullAssistDistance
            );
            float distanceWeight = Mathf.InverseLerp(
                nearRange,
                fullAssistDistance,
                Mathf.Abs(horizontalDistance)
            );
            float assistStrength = Mathf.Lerp(
                settings.MovementAssistStrength,
                settings.FarMovementAssistStrength,
                distanceWeight
            );

            jumpForce.y *= settings.FarMovementVerticalMultiplier;
            ApplyLeanRespectingHorizontalAssist(
                ref jumpForce,
                horizontalDistance,
                _entityData.JumpPower * assistStrength,
                settings
            );
        }

        void ApplyVerticalReachAssist(
            ref Vector2 jumpForce,
            float ballHeight,
            EntityData.KickAssistSettings settings
        )
        {
            if (settings.VerticalMovementAssistStrength <= 0f ||
                ballHeight <= settings.VerticalAssistMinimumBallHeight)
                return;

            float fullAssistHeight = Mathf.Max(
                settings.VerticalAssistMinimumBallHeight + Mathf.Epsilon,
                settings.VerticalAssistFullBallHeight
            );
            float verticalAssistWeight = Mathf.InverseLerp(
                settings.VerticalAssistMinimumBallHeight,
                fullAssistHeight,
                ballHeight
            );
            jumpForce.y += _entityData.JumpPower *
                           settings.VerticalMovementAssistStrength *
                           verticalAssistWeight;
        }

        void ApplyLeanRespectingHorizontalAssist(
            ref Vector2 jumpForce,
            float targetOffset,
            float requestedAssist,
            EntityData.KickAssistSettings settings
        )
        {
            if (Mathf.Abs(targetOffset) <= Mathf.Epsilon || requestedAssist <= Mathf.Epsilon)
                return;

            float targetDirection = Mathf.Sign(targetOffset);
            float naturalHorizontalForce = jumpForce.x;
            float uprightThreshold = _entityData.JumpPower * settings.NaturalLeanUprightThreshold;

            if (Mathf.Abs(naturalHorizontalForce) <= uprightThreshold)
            {
                jumpForce.x += targetDirection * requestedAssist *
                               settings.UprightMovementAssistMultiplier;
                return;
            }

            if (Mathf.Sign(naturalHorizontalForce) == targetDirection)
            {
                jumpForce.x += targetDirection * requestedAssist;
                return;
            }

            // Preserve the timing skill in the rocking body: assistance may soften a wrong-way
            // lean, but can never turn that lean into movement in the opposite direction.
            float maximumCorrection = Mathf.Abs(naturalHorizontalForce) *
                                      settings.WrongWayMovementDamping;
            jumpForce.x += targetDirection * Mathf.Min(requestedAssist, maximumCorrection);
        }

        EntityData.KickAssistSettings GetFarTravelSettings(PlayerActions teammate,
            EntityData.KickAssistSettings settings, out bool coversGoal)
        {
            bool isGoalkeeper = _entity != null && _entity.PlayerType == PlayerType.Goalkeeper;
            coversGoal = isGoalkeeper;
            if (!settings.ShareTravelRolesByPosition)
                return settings;

            EntityData.KickAssistSettings teammateSettings = teammate?.Data?.KickAssist;
            if (teammateSettings == null || !teammateSettings.ShareTravelRolesByPosition)
            {
                coversGoal = false;
                return settings;
            }

            float lead = (_rigidbody.position.x - teammate._rigidbody.position.x) * _kickingDirectionMultiplier;
            // Side by side, the designated goalie covers until one teammate pulls ahead.
            coversGoal = Mathf.Approximately(lead, 0f) ? isGoalkeeper : lead < 0f;
            return coversGoal == isGoalkeeper ? settings : teammateSettings;
        }

        float GetFarTravelTargetX(Rigidbody2D ballRigidbody, EntityData.KickAssistSettings settings,
            PlayerActions teammate, bool coversGoal)
        {
            float predictionTime = GetBallLandingPredictionTime(ballRigidbody, settings);
            float targetX = ballRigidbody.position.x +
                            ballRigidbody.linearVelocity.x * predictionTime;
            if (coversGoal && settings.GoalkeeperSupportOffset > 0f)
            {
                targetX -= _kickingDirectionMultiplier * settings.GoalkeeperSupportOffset;
                if (settings.GoalkeeperStayInOwnHalf)
                {
                    targetX = _kickingDirectionMultiplier > 0
                        ? Mathf.Min(targetX, 0f)
                        : Mathf.Max(targetX, 0f);
                }
            }

            if (teammate != null && settings.TeammateSeparationDistance > 0f)
            {
                float teammateOffset = _rigidbody.position.x - teammate._rigidbody.position.x;
                float teammateDistance = Mathf.Abs(teammateOffset);
                if (teammateDistance < settings.TeammateSeparationDistance)
                {
                    float separationDirection = teammateDistance > Mathf.Epsilon
                        ? Mathf.Sign(teammateOffset)
                        : (coversGoal ? -_kickingDirectionMultiplier : _kickingDirectionMultiplier);
                    float separationWeight = 1f - teammateDistance / settings.TeammateSeparationDistance;
                    targetX += separationDirection *
                               settings.TeammateSeparationTargetOffset *
                               separationWeight;
                }
            }

            return Mathf.Clamp(
                targetX,
                -settings.PlayableHorizontalLimit,
                settings.PlayableHorizontalLimit
            );
        }

        float GetBallLandingPredictionTime(
            Rigidbody2D ballRigidbody,
            EntityData.KickAssistSettings settings
        )
        {
            float minimumTime = settings.BallPredictionTime;
            float maximumTime = Mathf.Max(minimumTime, settings.MaximumTravelPredictionTime);
            if (ballRigidbody.position.y <= _rigidbody.position.y + 1f)
                return minimumTime;

            float gravity = Physics2D.gravity.y * ballRigidbody.gravityScale;
            float a = 0.5f * gravity;
            float b = ballRigidbody.linearVelocity.y;
            float c = ballRigidbody.position.y - _rigidbody.position.y;
            float discriminant = b * b - 4f * a * c;
            if (Mathf.Abs(a) <= Mathf.Epsilon || discriminant < 0f)
                return minimumTime;

            float squareRoot = Mathf.Sqrt(discriminant);
            float firstRoot = (-b + squareRoot) / (2f * a);
            float secondRoot = (-b - squareRoot) / (2f * a);
            float landingTime = float.PositiveInfinity;
            if (firstRoot > 0f)
                landingTime = firstRoot;
            if (secondRoot > 0f)
                landingTime = Mathf.Min(landingTime, secondRoot);

            if (float.IsPositiveInfinity(landingTime))
                return minimumTime;

            return Mathf.Clamp(landingTime, minimumTime, maximumTime);
        }

        public void Kick()
        {
            LastKickTime = Time.timeAsDouble;
            CancelKickingLegMotorStop();
            ApplyKickingPower(-1);
            _fullKickTimeRemaining = _entityData.KickAssist?.FullPowerKickDuration ?? 0f;
            if (_fullKickTimeRemaining > 0f && _kickingLegJoint.useLimits)
            {
                JointAngleLimits2D limits = _kickingLegJoint.limits;
                float stop = _kickingLegJointMotor.motorSpeed < 0f ? limits.min : limits.max;
                // Hold just inside the stop so the motor and the hinge limit don't fight.
                _heldKickAngle = Mathf.MoveTowards(stop, 0f, 3f);
            }
        }

        void UpdateHeldKick(float targetAngle)
        {
            float travelRemaining = (targetAngle - _kickingLegJoint.jointAngle) * Mathf.Sign(_kickingLegJointMotor.motorSpeed);
            float travelPerStep = Mathf.Abs(_kickingLegJointMotor.motorSpeed) * Time.fixedDeltaTime;
            if (_fullKickTimeRemaining > 0f && travelRemaining > travelPerStep)
            {
                _fullKickTimeRemaining -= Time.fixedDeltaTime;
                return;
            }

            // Once the strike ends, contact can deflect the foot without restarting a full-power kick.
            _fullKickTimeRemaining = 0f;
            HoldLegAtAngle(targetAngle);
        }

        public void CompleteTutorialKick()
        {
            CancelForcedKickHold();
            if (_tutorialKickFollowThroughCoroutine != null)
                StopCoroutine(_tutorialKickFollowThroughCoroutine);

            ForcedToHoldKick = false;
            CanKick = false;
            _tutorialKickFollowThroughCoroutine = StartCoroutine(TutorialKickFollowThrough());
        }

        IEnumerator TutorialKickFollowThrough()
        {
            // Contact marks success, but the normal motor still needs to finish the forward swing.
            Kick();
            yield return new WaitForSeconds(0.25f);
            ReturnLeftLegToOriginalPosition();
            _tutorialKickFollowThroughCoroutine = null;
        }

        void CancelForcedKickHold()
        {
            if (_forceKickToHoldCoroutine == null)
                return;

            StopCoroutine(_forceKickToHoldCoroutine);
            _forceKickToHoldCoroutine = null;
            inforceKickToHoldRoutine = false;
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
            _heldKickAngle = null;
            _legAtRest = direction == 0f;
            _kickingLegJointMotor.motorSpeed = _entityData.KickingPower * _kickingDirectionMultiplier * direction;
            _kickingLegJoint.motor = _kickingLegJointMotor;
        }

        void HoldLegAtAngle(float targetAngle)
        {
            float error = Mathf.DeltaAngle(_kickingLegJoint.jointAngle, targetAngle);
            JointMotor2D motor = _kickingLegJointMotor;
            motor.maxMotorTorque = Mathf.Min(motor.maxMotorTorque, 24f);
            motor.motorSpeed = Mathf.Abs(error) <= 1.5f ? 0f : Mathf.Clamp(error * 10f, -180f, 180f);

            JointMotor2D current = _kickingLegJoint.motor;
            if (!Mathf.Approximately(current.motorSpeed, motor.motorSpeed) ||
                !Mathf.Approximately(current.maxMotorTorque, motor.maxMotorTorque))
                _kickingLegJoint.motor = motor;
        }

        void ScheduleKickingLegMotorStop()
        {
            CancelKickingLegMotorStop();
            EntityData.KickAssistSettings settings = _entityData?.KickAssist;
            if (settings == null || settings.KickingLegReturnMotorDuration <= 0f)
            {
                if (_centerReleasedLeg)
                    ApplyKickingPower(0f);
                return;
            }

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
            CancelForcedKickHold();
            if (_tutorialKickFollowThroughCoroutine != null)
            {
                StopCoroutine(_tutorialKickFollowThroughCoroutine);
                _tutorialKickFollowThroughCoroutine = null;
            }
            if (_entityData != null)
                ApplyKickingPower(0f);
            _jumpCdTimer?.Stop();
            _jumpOnCd = false;
            _inputActionActive = false;
            _kickAssistAvailable = false;
            _kickAssistTimeRemaining = 0f;
            _lastActionTime = float.NegativeInfinity;
            LastKickTime = double.NegativeInfinity;
            _headerTakeoffTime = double.NegativeInfinity;
            _tutorialHeadBlockHopPending = false;
            _pendingKickHopSpeed = 0f;
            _pendingKickHopEvent = false;
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
            _forceKickToHoldCoroutine = null;
        }
    }
}
