using System.Collections;
using System.Collections.Generic;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Managers;
using TMPro;
using UnityEngine;

public class TimedBallTutorial : MonoBehaviour
{
    const float SuccessfulKickSlowMotionDuration = 1.5f;
    const float KickSpeedRecoveryDuration = 0.35f;
    const float SuccessfulKickFlightDuration = 2.5f;

    public enum PracticeType { Kick, Block }

    [SerializeField] TextMeshProUGUI _promptText;
    [SerializeField, Min(0.1f)] float _flightDuration = 1.25f;
    [SerializeField, Min(0.1f)] float _attemptDuration = 4f;
    [SerializeField, Min(0f)] float _readyDelay = 0.5f;
    [SerializeField, Min(0f)] float _retryDelay = 1f;
    [SerializeField, Min(0f)] float _blockHeightAboveHead = 0.6f;
    [Header("Manual Block Feed")]
    [Tooltip("Time taken for the ball to reach the player's horizontal position.")]
    [SerializeField, Min(0.1f)] float _blockFlightDuration = 1.5f;
    [SerializeField, Min(0f)] float _blockArcHeight = 2f;
    [SerializeField, Min(0.5f)] float _blockPassDistance = 2.5f;
    [Tooltip("Grandpa's jump force during block feeds only; player jump strength is unchanged.")]
    [SerializeField, Range(0f, 1f)] float _blockGrandpaJumpMultiplier = 0.5f;
    [SerializeField, Min(0.1f)] float _blockLandingTimeout = 8f;
    [Header("Block Dialogue Timing")]
    [SerializeField, Min(0f)] float _blockIntroDialogueDelay = 1f;
    [SerializeField, Min(0f)] float _blockFeedbackDialogueDuration = 1.5f;
    [Tooltip("Pause after the success dialogue before completing the block tutorial.")]
    [SerializeField, Min(0f)] float _blockCompletionDelay;

    class ActionState
    {
        public PlayerActions Actions;
        public bool CanKick;
        public bool ForcedToHoldKick;
        public bool DisableInput;
        public Cpu Cpu;
        public bool CpuEnabled;
    }

    readonly List<ActionState> _actionStates = new();
    BallScript _ball;
    BallManager _ballManager;
    MatchManager _matchManager;
    PlayerActions _player;
    PlayerActions _grandpaActions;
    GrandpaStage0 _grandpa;
    MoveToPointCurved _kickFlightMover;
    PracticeType _practiceType;
    bool _running;
    bool _attemptActive;
    bool _previousResetSuppression;
    bool _matchWasEnabled;
    readonly ContactPoint2D[] _blockContacts = new ContactPoint2D[8];
    bool _blockFeedActive;
    float _blockFeedGravity;
    float _blockFeedElapsed;
    float _blockFeedDuration;
    float _blockMissHeight;
    Vector2 _blockArcStart, _blockArcMid, _blockArcEnd;
    bool _blockTransitionPrepared;
    bool _blockInterpolationCaptured;
    RigidbodyInterpolation2D _blockPreviousInterpolation;
    BallScript _blockTransitionBall;
    MatchManager _blockMatchManager;
    bool _blockMatchEventsCaptured;
    bool _previousBlockMatchEventSuppression;
    readonly Dictionary<Rigidbody2D, RigidbodyInterpolation2D> _blockPlayerInterpolations = new();

    public bool Succeeded { get; private set; }

    public bool PrepareBlockTransition()
    {
        if (_running)
            return false;

        PlayersManager players = PlayersManager.Instance;
        BallScript ball = BallManager.Instance != null ? BallManager.Instance.Ball : null;
        GrandpaStage0 grandpa = GrandpaStage0.Instance;
        if (players == null || players.Players.Count == 0 || ball == null || grandpa == null)
        {
            Debug.LogError("The block transition needs its player, ball, and Grandpa.", this);
            return false;
        }

        // Keep the match enabled: CampaignGameplayExecution advances when it is disabled.
        _blockMatchManager = MatchManager.Instance;
        if (_blockMatchManager != null)
        {
            if (!_blockMatchEventsCaptured)
            {
                _previousBlockMatchEventSuppression = _blockMatchManager.SuppressGoalAndOutEvents;
                _blockMatchEventsCaptured = true;
            }
            _blockMatchManager.SuppressGoalAndOutEvents = true;
            _blockMatchManager.StopAllCoroutines();
        }
        PlaceBlockBallAtGrandpa(players, ball, grandpa, preserveKickArrival: true);
        _blockTransitionPrepared = true;
        return true;
    }

    void PlaceBlockBallAtGrandpa(PlayersManager players, BallScript ball, GrandpaStage0 grandpa,
        bool preserveKickArrival = false)
    {
        // A reset that started before the handoff must not keep blinking or re-enabling this ball.
        ball.StopAllCoroutines();
        if (!_blockInterpolationCaptured)
        {
            _blockPreviousInterpolation = ball.Rigidbody.interpolation;
            _blockInterpolationCaptured = true;
        }
        _blockTransitionBall = ball;
        ball.Rigidbody.interpolation = RigidbodyInterpolation2D.None;
        ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        ball.Rigidbody.linearVelocity = Vector2.zero;
        ball.Rigidbody.angularVelocity = 0f;
        // Keep the learner's position and pose through the kick-to-block handoff.
        // Only a failed block attempt returns the learner to the grounded reset position.
        if (!preserveKickArrival)
            PlaceBlockPlayerOnGround(players);

        Vector2 feedPosition = grandpa.GetTutorialFeedPosition(players.Players[0].transform.position.x);
        // Hand off the exact completed kick position, even if Grandpa shifted during its flight.
        if (preserveKickArrival && grandpa.TryTakeTutorialKickArrival(out Vector2 arrivedPosition))
            feedPosition = arrivedPosition;
        Vector3 visiblePosition = new Vector3(feedPosition.x, feedPosition.y, ball.transform.position.z);
        ball.transform.SetPositionAndRotation(visiblePosition, Quaternion.identity);
        ball.Rigidbody.position = feedPosition;
        ball.Rigidbody.rotation = 0f;
        ball.Rigidbody.simulated = true;
        ball.Renderer.enabled = true;
        ball.Collider.enabled = true;
    }

    void PlaceBlockPlayerOnGround(PlayersManager players)
    {
        RestoreBlockPlayerInterpolation();
        GameObject player = players.Players[0];
        foreach (Rigidbody2D body in player.GetComponentsInChildren<Rigidbody2D>())
        {
            _blockPlayerInterpolations.Add(body, body.interpolation);
            body.interpolation = RigidbodyInterpolation2D.None;
        }

        players.ResetMainPlayer();
        Physics2D.SyncTransforms();

        float feetY = float.PositiveInfinity;
        float lowestBodyY = float.PositiveInfinity;
        float topY = player.transform.position.y;
        foreach (Collider2D bodyCollider in player.GetComponentsInChildren<Collider2D>())
        {
            if (!bodyCollider.enabled || bodyCollider.isTrigger)
                continue;

            Bounds bounds = bodyCollider.bounds;
            lowestBodyY = Mathf.Min(lowestBodyY, bounds.min.y);
            topY = Mathf.Max(topY, bounds.max.y);
            string part = bodyCollider.gameObject.name;
            if (part == "LeftLeg" || part == "RightLeg" || part == "Foot")
                feetY = Mathf.Min(feetY, bounds.min.y);
        }
        if (float.IsPositiveInfinity(feetY))
            feetY = lowestBodyY;

        float groundY = float.NegativeInfinity;
        Vector2 rayStart = new Vector2(player.transform.position.x, topY + 1f);
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(rayStart, Vector2.down, 100f))
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground") && hit.normal.y > 0.5f)
                groundY = Mathf.Max(groundY, hit.point.y);
        }
        if (!float.IsPositiveInfinity(feetY) && !float.IsNegativeInfinity(groundY))
            player.transform.position += Vector3.up * (groundY + 0.02f - feetY);

        Physics2D.SyncTransforms();
        foreach (Rigidbody2D body in _blockPlayerInterpolations.Keys)
        {
            body.position = body.transform.position;
            body.rotation = body.transform.eulerAngles.z;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.WakeUp();
        }
        Physics2D.SyncTransforms();
    }

    void RestoreBlockPlayerInterpolation()
    {
        foreach (KeyValuePair<Rigidbody2D, RigidbodyInterpolation2D> entry in _blockPlayerInterpolations)
        {
            if (entry.Key != null)
                entry.Key.interpolation = entry.Value;
        }
        _blockPlayerInterpolations.Clear();
    }

    void RestoreBlockInterpolation()
    {
        RestoreBlockPlayerInterpolation();
        if (_blockInterpolationCaptured && _blockTransitionBall != null)
            _blockTransitionBall.Rigidbody.interpolation = _blockPreviousInterpolation;
        _blockInterpolationCaptured = false;
        _blockTransitionPrepared = false;
        _blockTransitionBall = null;
    }

    void RestoreBlockMatchEvents()
    {
        if (_blockMatchEventsCaptured && _blockMatchManager != null)
            _blockMatchManager.SuppressGoalAndOutEvents = _previousBlockMatchEventSuppression;
        _blockMatchEventsCaptured = false;
        _blockMatchManager = null;
    }

    public IEnumerator Run(PracticeType practiceType, TutorialFade prompt, MoveToPointCurved ballMove)
    {
        Succeeded = false;
        PlayersManager players = PlayersManager.Instance;
        _ballManager = BallManager.Instance;
        _ball = _ballManager != null ? _ballManager.Ball : null;
        _grandpa = GrandpaStage0.Instance;
        _player = players != null && players.Players.Count > 0
            ? players.Players[0].GetComponent<PlayerActions>()
            : null;
        _grandpaActions = _grandpa != null ? _grandpa.GetComponent<PlayerActions>() : null;
        if (_running || _ball == null || _player == null || _grandpaActions == null)
        {
            Debug.LogError("Timing practice needs the tutorial player, ball, and Grandpa.", this);
            yield break;
        }

        Collider2D targetCollider = practiceType == PracticeType.Kick
            ? _player.KickingContactCollider
            : _player.HeadContactCollider;
        if (targetCollider == null)
        {
            Debug.LogError("Timing practice needs the player's kicking leg or head collider.", this);
            yield break;
        }

        if (practiceType == PracticeType.Block && !_blockTransitionPrepared && !PrepareBlockTransition())
            yield break;

        _running = true;
        _practiceType = practiceType;
        _previousResetSuppression = _ballManager.SuppressAutomaticReset;
        _ballManager.SuppressAutomaticReset = true;
        _matchManager = MatchManager.Instance;
        _matchWasEnabled = _matchManager != null && _matchManager.enabled;
        if (_matchManager != null && _practiceType != PracticeType.Block)
            _matchManager.enabled = false;

        // Grandpa feeds deliberately; goal/out notifications must not interrupt a practice attempt.
        foreach (PlayerActions actions in players.GetPlayerActions())
        {
            Cpu cpu = actions.GetComponent<Cpu>();
            _actionStates.Add(new ActionState
            {
                Actions = actions,
                CanKick = actions.CanKick,
                ForcedToHoldKick = actions.ForcedToHoldKick,
                DisableInput = actions.DisableInput,
                Cpu = cpu,
                CpuEnabled = cpu != null && cpu.enabled
            });
            if (cpu != null)
            {
                cpu.StopAllCoroutines();
                cpu.enabled = false;
            }
            actions.ResetActionState();
            actions.CanKick = false;
            actions.ForcedToHoldKick = false;
            actions.DisableInput = actions != _player;
        }

        _kickFlightMover = ballMove;
        ballMove?.StopCurveMove();
        _ball.ContactedPlayer += OnBallContactedPlayer;
        if (_practiceType == PracticeType.Block)
            _ball.CollisionEntered += OnBlockFeedCollision;
        TimeScaleManager.SetGameplayTimeScale();
        SetPrompt(false);
        prompt?.FadeIn();
        Stage0Scene2Tutorials dialogue = _practiceType == PracticeType.Block
            ? Stage0Scene2Tutorials.Instance
            : null;
        bool firstAttempt = true;

        try
        {
            while (!Succeeded && _running && _ball != null && _player != null && _grandpa != null)
            {
                _attemptActive = false;
                _player.CanKick = false;
                if (_practiceType == PracticeType.Block)
                {
                    StopBlockFeed();
                    if (_blockTransitionPrepared)
                        _blockTransitionPrepared = false;
                    else
                        PlaceBlockBallAtGrandpa(players, _ball, _grandpa);
                }
                else
                {
                    _ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    _ball.ResetWithoutBlink();
                    players.ResetPlayers();
                    _ball.Rigidbody.position = GetFeedPosition();
                    _ball.Renderer.enabled = true;
                    _ball.Collider.enabled = true;
                }
                if (dialogue != null && firstAttempt)
                {
                    dialogue.ShowBlockInstruction(prompt);
                    yield return WaitForDialogue(_blockIntroDialogueDelay);
                }
                else
                {
                    if (dialogue != null)
                        SetPrompt(false);
                    yield return new WaitForSeconds(_readyDelay);
                }
                firstAttempt = false;

                if (!_running || _ball == null || _player == null || _grandpa == null)
                    yield break;

                FeedBall(targetCollider);
                if (_practiceType == PracticeType.Block)
                {
                    // The next real input must produce a head-block hop even if grounding is stale.
                    _player.PrepareTutorialHeadBlockHop();
                }
                _player.CanKick = true;
                _attemptActive = true;
                float elapsed = 0f;
                float attemptDuration = Mathf.Max(_attemptDuration, _flightDuration + 0.5f);
                bool grandpaLegReturned = false;
                while (!Succeeded && _running && _ball != null &&
                       (_practiceType == PracticeType.Block || elapsed < attemptDuration))
                {
                    elapsed += Time.deltaTime;
                    if (!grandpaLegReturned && elapsed >= 0.2f)
                    {
                        _grandpaActions.ReturnLeftLegToOriginalPosition();
                        grandpaLegReturned = true;
                    }
                    if (_practiceType == PracticeType.Block && BlockBallHasDropped(elapsed))
                        break;
                    yield return null;
                }

                _attemptActive = false;
                if (!Succeeded && _running)
                {
                    if (_practiceType == PracticeType.Block)
                    {
                        _player.OnActionCancelled();
                        _player.CanKick = false;
                    }
                    if (dialogue != null)
                    {
                        dialogue.ShowBlockFailure(prompt);
                        yield return WaitForDialogue(_blockFeedbackDialogueDuration);
                    }
                    else
                    {
                        SetPrompt(true);
                        yield return new WaitForSeconds(_retryDelay);
                    }
                }
            }

            if (Succeeded)
            {
                if (dialogue != null)
                    dialogue.ShowBlockSuccess(prompt);
                else if (_promptText != null)
                    _promptText.text = "Good timing!";

                if (_practiceType == PracticeType.Kick)
                {
                    yield return WaitForSuccessfulKickFlight(_ball);
                }
                else
                {
                    _player.OnActionCancelled();
                    _player.CanKick = false;
                    if (dialogue != null)
                        yield return WaitForDialogue(_blockFeedbackDialogueDuration);
                    else
                        yield return new WaitForSeconds(0.5f);
                }
                prompt?.FadeOut();
                if (_practiceType == PracticeType.Block)
                {
                    // Keep practice reset/goal suppression active until the end screen is ready.
                    yield return WaitForDialogue(_blockCompletionDelay);
                }
            }
        }
        finally
        {
            RestorePracticeState();
        }
    }

    static IEnumerator WaitForDialogue(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            if (Time.timeScale > 0f)
                elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    Vector2 GetFeedPosition()
    {
        return _grandpa.GetTutorialFeedPosition(_player.transform.position.x);
    }

    void FeedBall(Collider2D targetCollider)
    {
        if (_practiceType == PracticeType.Block)
        {
            FeedBlockBall(targetCollider);
            return;
        }

        Vector2 start = GetFeedPosition();
        Vector2 target = targetCollider.bounds.center;
        if (_practiceType == PracticeType.Block)
            target.y += _blockHeightAboveHead;

        float flightTime = Mathf.Max(0.1f, _flightDuration);
        float damping = _ball.Rigidbody.linearDamping;
        float travelFactor = damping > Mathf.Epsilon
            ? (1f - Mathf.Exp(-damping * flightTime)) / damping
            : flightTime;
        float gravityFactor = damping > Mathf.Epsilon
            ? (flightTime - travelFactor) / damping
            : 0.5f * flightTime * flightTime;
        Vector2 gravity = Physics2D.gravity * _ball.Rigidbody.gravityScale;

        _grandpaActions.CanKick = true;
        _grandpaActions.DisableInput = false;
        _grandpa.DoScriptedKick();
        _grandpaActions.CanKick = false;
        _grandpaActions.DisableInput = true;

        _ball.Rigidbody.position = start;
        _ball.Rigidbody.simulated = true;
        _ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _ball.Rigidbody.linearVelocity = (target - start - gravity * gravityFactor) / travelFactor;
        _ball.Rigidbody.angularVelocity = 0f;
        // After launch, normal physics and the player's usual action determine the result.
    }

    void FeedBlockBall(Collider2D headCollider)
    {
        // Launch from the staged ball, including the exact position reached by the initial kick.
        _blockArcStart = _ball.Rigidbody.position;
        Bounds standingHead = headCollider.bounds;
        float direction = Mathf.Sign(standingHead.center.x - _blockArcStart.x);
        _blockArcEnd = (Vector2)standingHead.center +
                       Vector2.right * direction * Mathf.Max(0.5f, _blockPassDistance);

        // The player intercepts the arc before its endpoint; an untouched shot continues behind them.
        float totalDistance = Mathf.Max(0.1f, Mathf.Abs(_blockArcEnd.x - _blockArcStart.x));
        float crossingT = Mathf.Clamp(
            Mathf.Abs(standingHead.center.x - _blockArcStart.x) / totalDistance, 0.05f, 0.95f);
        _blockFeedDuration = Mathf.Max(0.1f, _blockFlightDuration) / crossingT;
        float crossingHeight = standingHead.max.y +
                               _ball.Collider.bounds.extents.y + _blockHeightAboveHead;
        float remainingT = 1f - crossingT;
        float requiredMidHeight = (crossingHeight -
                                   remainingT * remainingT * _blockArcStart.y -
                                   crossingT * crossingT * _blockArcEnd.y) /
                                  (2f * remainingT * crossingT);
        _blockArcMid = (_blockArcStart + _blockArcEnd) * 0.5f;
        _blockArcMid.y = Mathf.Max(requiredMidHeight,
            Mathf.Max(_blockArcStart.y, _blockArcEnd.y) + _blockArcHeight);
        _blockMissHeight = _player.transform.position.y - 3f;

        _grandpaActions.CanKick = true;
        _grandpaActions.DisableInput = false;
        _grandpa.DoScriptedKick(_blockGrandpaJumpMultiplier);
        _grandpaActions.CanKick = false;
        _grandpaActions.DisableInput = true;

        _ball.Rigidbody.position = _blockArcStart;
        _ball.Rigidbody.simulated = true;
        _ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _ball.Rigidbody.linearVelocity = Vector2.zero;
        _ball.Rigidbody.angularVelocity = 0f;
        _blockFeedGravity = _ball.Rigidbody.gravityScale;
        _ball.Rigidbody.gravityScale = 0f;
        if (_blockInterpolationCaptured)
            _ball.Rigidbody.interpolation = _blockPreviousInterpolation;
        RestoreBlockPlayerInterpolation();
        _blockFeedElapsed = 0f;
        _blockFeedActive = true;
    }

    void FixedUpdate()
    {
        if (!_running || !_attemptActive || _practiceType != PracticeType.Block ||
            !_blockFeedActive || _ball == null)
            return;

        float flightDuration = _blockFeedDuration;
        _blockFeedElapsed += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(_blockFeedElapsed / flightDuration);
        Vector2 nextPosition = QuadraticCurve.Evaluate(_blockArcStart, _blockArcMid, _blockArcEnd, t);
        // Follow the authored arc with a dynamic body so a real block can interrupt it.
        _ball.Rigidbody.linearVelocity = (nextPosition - _ball.Rigidbody.position) / Time.fixedDeltaTime;
        if (_blockFeedElapsed >= flightDuration)
            StopBlockFeed();
    }

    void OnBlockFeedCollision() => StopBlockFeed();

    void StopBlockFeed()
    {
        if (!_blockFeedActive)
            return;

        _blockFeedActive = false;
        if (_ball != null)
            _ball.Rigidbody.gravityScale = _blockFeedGravity;
    }

    bool BlockBallHasDropped(float elapsed)
    {
        if (_blockFeedActive)
            return false;

        int contactCount = _ball.Collider.GetContacts(_blockContacts);
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D contact = _blockContacts[i];
            if ((contact.collider != null && contact.collider.CompareTag("Ground")) ||
                (contact.otherCollider != null && contact.otherCollider.CompareTag("Ground")))
                return true;
        }

        // Recover if the miss falls below the field or the ball gets stuck off the ground.
        return _ball.Collider.bounds.max.y < _blockMissHeight ||
               elapsed >= _blockFeedDuration + _blockLandingTimeout;
    }

    public void CancelBlockPractice()
    {
        if (_running && _practiceType == PracticeType.Block)
            RestorePracticeState();
        else if (_blockTransitionPrepared)
        {
            RestoreBlockInterpolation();
            RestoreBlockMatchEvents();
        }
    }

    void OnBallContactedPlayer(PlayerActions actions, Collider2D bodyCollider)
    {
        if (Succeeded || !_attemptActive || actions != _player || !actions.IsWithinActionContactWindow)
            return;

        bool correctContact = _practiceType == PracticeType.Kick
            ? actions.IsKickingContact(bodyCollider)
            : bodyCollider == actions.HeadContactCollider;
        if (correctContact)
        {
            Succeeded = true;
            if (_practiceType == PracticeType.Kick)
                BeginSuccessfulKick(actions, _ball, _kickFlightMover);
        }
    }

    public static void BeginSuccessfulKick(PlayerActions actions, BallScript ball, MoveToPointCurved flightMover)
    {
        actions.CompleteTutorialKick();
        TimeScaleManager.SlowMotion();
        GrandpaStage0 grandpa = GrandpaStage0.Instance;
        if (ball != null && flightMover != null && grandpa != null)
        {
            Vector2 target = (Vector2)grandpa.transform.position + Vector2.up * 2.2f;
            flightMover.StartKickFlight(ball, target);
        }
    }

    public static IEnumerator WaitForSuccessfulKickFlight(BallScript ball)
    {
        float lastAppliedScale = TimeScaleManager.SlowMotionScale;
        bool restoreSpeedOnExit = true;
        try
        {
            float slowMotionElapsed = 0f;
            while (ball != null && slowMotionElapsed < SuccessfulKickSlowMotionDuration)
            {
                if (Time.timeScale > 0f)
                {
                    TimeScaleManager.SlowMotion();
                    lastAppliedScale = Time.timeScale;
                    slowMotionElapsed += Time.unscaledDeltaTime;
                }
                yield return null;
            }

            if (ball == null)
                yield break;

            float recoveryElapsed = 0f;
            while (ball != null && recoveryElapsed < KickSpeedRecoveryDuration)
            {
                if (Time.timeScale > 0f)
                {
                    recoveryElapsed += Time.unscaledDeltaTime;
                    lastAppliedScale = Mathf.Lerp(TimeScaleManager.SlowMotionScale,
                        TimeScaleManager.GameplayScale,
                        Mathf.Clamp01(recoveryElapsed / KickSpeedRecoveryDuration));
                    Time.timeScale = lastAppliedScale;
                }
                yield return null;
            }

            if (ball == null)
                yield break;

            TimeScaleManager.SetGameplayTimeScale();
            restoreSpeedOnExit = false;

            // Slow motion is extra breathing room, followed by the full normal-speed flight window.
            float flightElapsed = 0f;
            while (ball != null && flightElapsed < SuccessfulKickFlightDuration)
            {
                if (Time.timeScale > 0f)
                    flightElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        finally
        {
            if (restoreSpeedOnExit && Time.timeScale > 0f &&
                Mathf.Approximately(Time.timeScale, lastAppliedScale))
                TimeScaleManager.SetGameplayTimeScale();
        }
    }

    void SetPrompt(bool retry)
    {
        if (_promptText == null)
            return;
        if (_practiceType == PracticeType.Block)
        {
            _promptText.text = "Press Z to block the ball";
            return;
        }
        string action = _practiceType == PracticeType.Kick ? "kick" : "block";
        _promptText.text = retry
            ? $"Try again: time your {action} with Z"
            : $"Time your {action} with Z";
    }

    void OnDisable()
    {
        CancelBlockPractice();
        RestorePracticeState();
    }

    void RestorePracticeState()
    {
        if (!_running)
            return;

        _running = false;
        _attemptActive = false;
        if (_practiceType == PracticeType.Block)
        {
            StopBlockFeed();
            if (_ball != null)
            {
                _ball.CollisionEntered -= OnBlockFeedCollision;
                if (_ball.Rigidbody.bodyType == RigidbodyType2D.Kinematic)
                    _ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            }
            RestoreBlockInterpolation();
            RestoreBlockMatchEvents();
        }
        _kickFlightMover?.StopCurveMove();
        _kickFlightMover = null;
        if (_ball != null)
            _ball.ContactedPlayer -= OnBallContactedPlayer;
        if (_ballManager != null)
            _ballManager.SuppressAutomaticReset = _previousResetSuppression;
        if (_matchManager != null && _practiceType != PracticeType.Block)
            _matchManager.enabled = _matchWasEnabled;
        foreach (ActionState state in _actionStates)
        {
            if (state.Actions != null)
            {
                state.Actions.ResetActionState();
                state.Actions.CanKick = state.CanKick;
                state.Actions.ForcedToHoldKick = state.ForcedToHoldKick;
                state.Actions.DisableInput = state.DisableInput;
            }
            if (state.Cpu != null)
                state.Cpu.enabled = state.CpuEnabled;
        }
        _actionStates.Clear();
    }
}
