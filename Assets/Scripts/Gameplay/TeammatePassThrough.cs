using System.Collections.Generic;
using Gameplay.CharacterComponents;
using Gameplay.Managers;
using UnityEngine;

namespace Gameplay
{
    // One instance belongs to one match ball. Only the receiver's solid colliders can be bypassed.
    internal sealed class TeammatePassThrough
    {
        // A tap may meet the ball over a few physics steps. A foot held out for the
        // rest of a jump should not turn the subsequent landing into a deliberate kick.
        const float ReceiveStrikeWindow = 0.22f;

        sealed class Participant
        {
            internal readonly PlayerActions Actions;
            internal readonly Rigidbody2D Body;
            internal readonly Collider2D[] Parts;
            internal readonly Collider2D Head;
            internal Participant Teammate;
            internal double LastShotKick;
            internal PassContact Contact;

            internal Participant(PlayerActions actions)
            {
                Actions = actions;
                Body = actions.GetComponent<Rigidbody2D>();
                Parts = actions.GetComponentsInChildren<Collider2D>(true);
                Head = actions.HeadContactCollider;
                LastShotKick = actions.LastKickTime;
            }
        }

        readonly BallScript _ball;
        readonly Dictionary<PlayerActions, Participant> _team = new(4);
        readonly List<(Collider2D part, Participant owner)> _ignored = new();
        readonly HashSet<Collider2D> _needed = new();
        readonly HashSet<Collider2D> _intended = new();
        Participant _receiver;
        Vector2 _incomingVelocity;
        bool _hasContacts, _hitBoundary;

        TeammatePassThrough(BallScript ball) => _ball = ball;

        internal static TeammatePassThrough ForMatch(BallScript ball, PlayersManager players)
        {
            // NumberOfPlayers counts human inputs. Check the actual two teams instead.
            if (players == null || players.Players.Count != 4)
                return null;

            var assist = new TeammatePassThrough(ball);
            foreach (var entity in players.Players)
            {
                if (entity == null || !entity.TryGetComponent(out PlayerActions actions))
                    return null;
                assist._team.Add(actions, new Participant(actions));
            }
            foreach (var actor in assist._team.Values)
            {
                var partner = players.GetTeammate(actor.Actions);
                if (partner == null || !assist._team.TryGetValue(partner, out actor.Teammate) ||
                    players.GetTeammate(partner) != actor.Actions)
                    return null;
            }
            return assist;
        }

        internal void OnPlayerContact(PlayerActions actions, Collider2D part, bool assistedKick,
            Collision2D collision, bool firstContact)
        {
            if (!_team.TryGetValue(actions, out var actor) || _ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
                return;

            // Unity reuses Collision2D. Copy only the evidence for this step; deciding the
            // passer in a callback would let the last limb reported overwrite a real clearance.
            ref var hit = ref actor.Contact;
            bool impact = false;
            int direction = actions.AttackingDirection;
            for (int i = 0; i < collision.contactCount; i++)
            {
                var point = collision.GetContact(i);
                if (point.normalImpulse <= _ball.Rigidbody.mass * Physics2D.linearSleepTolerance)
                    continue;
                var normal = point.normal;
                if (Vector2.Dot(normal, (Vector2)_ball.Collider.bounds.center - point.point) < 0f)
                    normal = -normal;
                float normalImpulseX = direction * normal.x * point.normalImpulse;
                float frictionX = Mathf.Abs(normal.y * point.tangentImpulse);
                hit.RecordImpulse(point.point.x * direction, normalImpulseX, frictionX);
                float surfaceSpeed = part.attachedRigidbody.GetPointVelocity(point.point).x * direction;
                hit.UpfieldSurfaceSpeed = Mathf.Max(hit.UpfieldSurfaceSpeed, surfaceSpeed);
                impact = true;
            }
            if (!assistedKick && !impact)
                return;

            if (!hit.Touched)
                hit.ContactDepth = part.bounds.center.x * direction;
            hit.Touched = true;
            _hasContacts = true;
            hit.Entered |= firstContact;
            bool intended = _intended.Contains(part) || IsReceivingStrike(actor, part, _ball.Collider.Distance(part), _incomingVelocity);
            // A rescue nudge can launch the ball off a torso. Only a receiving kick or
            // header takes priority over a teammate's clearance in the same step.
            hit.Played |= intended;
            if (assistedKick || intended)
                hit.KickTime = actions.LastKickTime;
            hit.CanLaunch |= hit.KickTime.HasValue || firstContact || _incomingVelocity.x * direction <= Physics2D.linearSleepTolerance;
        }

        internal void OnFieldContact(Collision2D collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                var point = collision.GetContact(i);
                float horizontalImpulse = Mathf.Abs(point.normal.x) * point.normalImpulse +
                    Mathf.Abs(point.normal.y * point.tangentImpulse);
                if (horizontalImpulse > Physics2D.linearSleepTolerance * _ball.Rigidbody.mass)
                    _hitBoundary = true;
            }
        }

        void ResolveContacts()
        {
            if (_hasContacts)
            {
                Participant source = null;
                bool interrupted = false;
                Participant first = null, second = null;
                int touched = 0;
                foreach (var actor in _team.Values)
                {
                    if (!actor.Contact.Touched || actor.Actions == null)
                        continue;
                    if (touched == 0) first = actor;
                    else second = actor;
                    touched++;
                    float impulseThreshold = _ball.Rigidbody.mass * Physics2D.linearSleepTolerance;
                    bool changedTravel = (actor.Contact.MinImpulse > impulseThreshold || actor.Contact.MaxImpulse < -impulseThreshold) &&
                        Mathf.Abs(_ball.Rigidbody.linearVelocityX - _incomingVelocity.x) > Physics2D.linearSleepTolerance;
                    interrupted |= _receiver != null && actor != _receiver.Teammate &&
                                   (actor.Contact.Entered || actor.Contact.KickTime.HasValue || changedTravel);
                }
                if (!_hitBoundary && touched == 2 && first.Teammate == second)
                    RecoverDoubleRebound(first, second);
                foreach (var actor in _team.Values)
                {
                    if (actor.Actions == null || !actor.Contact.CanLaunch ||
                        _ball.Rigidbody.linearVelocityX * actor.Actions.AttackingDirection <= Physics2D.linearSleepTolerance)
                        continue;
                    // A deliberate reception takes precedence. For simultaneous passive
                    // impacts the goal-side contact feeds the teammate along the outgoing path.
                    if (source == null || actor.Contact.Precedes(source.Contact,
                        (actor.Body.position.x - source.Body.position.x) * actor.Actions.AttackingDirection))
                        source = actor;
                }
                if (source != null)
                {
                    _receiver = source.Teammate;
                    if (source.Contact.KickTime is double kickTime)
                        source.LastShotKick = kickTime;
                }
                else if (interrupted)
                    _receiver = null;
                foreach (var actor in _team.Values)
                    actor.Contact = default;
            }
            _hasContacts = _hitBoundary = false;
        }

        void RecoverDoubleRebound(Participant first, Participant second)
        {
            var rear = first.Contact.ContactDepth <= second.Contact.ContactDepth ? first : second;
            int direction = rear.Actions.AttackingDirection;
            float outgoing = _ball.Rigidbody.linearVelocityX * direction;
            float corrected = rear.Contact.ClearanceSpeed(rear.Teammate.Contact, _incomingVelocity.x * direction,
                outgoing, _ball.Rigidbody.mass, _incomingVelocity.magnitude, Physics2D.linearSleepTolerance);
            if (corrected != outgoing)
                _ball.Rigidbody.linearVelocityX = direction * corrected;
        }

        internal void BeforePhysicsStep()
        {
            if (!_ball.Collider.isActiveAndEnabled || !_ball.Rigidbody.simulated ||
                _ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            {
                Reset();
                return;
            }

            ResolveContacts();
            _incomingVelocity = _ball.Rigidbody.linearVelocity;
            _needed.Clear();
            _intended.Clear();
            if (_receiver != null && _receiver.Actions != null && _receiver.Actions.isActiveAndEnabled &&
                _ball.Rigidbody.linearVelocityX * _receiver.Actions.AttackingDirection > Physics2D.linearSleepTolerance)
            {
                if (CheckReceiver(_receiver, _ball.Rigidbody.linearVelocity))
                    _receiver = null;
            }
            else
                _receiver = null;

            if (_receiver == null)
                ProtectPointBlankKick();
            ProtectCrowdedRebound();
            RestorePairs(force: false);
        }

        static bool TeammateIsAhead(Participant actor) => actor.Teammate.Actions != null &&
            (actor.Teammate.Actions.transform.position.x - actor.Actions.transform.position.x) *
            actor.Actions.AttackingDirection >= 0f;

        static bool HasUnusedKick(Participant actor)
        {
            float window = actor.Actions.Data?.KickAssist?.ContactAssistWindow ?? 0.45f;
            return actor.Actions.LastKickTime > actor.LastShotKick &&
                   Time.fixedTimeAsDouble - actor.Actions.LastKickTime <= window;
        }

        void ProtectPointBlankKick()
        {
            // Contact callbacks arrive after the solver. When both teammates crowd the ball,
            // protect the launch step too, but only with a fresh strike already at the ball.
            Participant striker = null;
            Vector2 launchVelocity = Vector2.zero;
            foreach (var actor in _team.Values)
            {
                if (actor.Actions == null || !actor.Actions.isActiveAndEnabled || !TeammateIsAhead(actor) ||
                    (!HasUnusedKick(actor) && !actor.Actions.IsOnHeaderJump))
                    continue;

                foreach (var part in actor.Parts)
                {
                    if (!IsSolid(part) || (part != actor.Head && !actor.Actions.IsKickingContact(part)))
                        continue;
                    var gap = _ball.Collider.Distance(part);
                    if (!gap.isValid || gap.distance > 2f * Physics2D.defaultContactOffset ||
                        !IsReceivingStrike(actor, part, gap, _ball.Rigidbody.linearVelocity))
                        continue;

                    Vector2 outgoing = _ball.Rigidbody.linearVelocity;
                    var strikeVelocity = part.attachedRigidbody.GetPointVelocity(gap.pointB);
                    int direction = actor.Actions.AttackingDirection;
                    if (strikeVelocity.x * direction > outgoing.x * direction)
                        outgoing = strikeVelocity;
                    bool startingFromRest = Time.fixedTimeAsDouble - actor.Actions.LastKickTime <= Time.fixedDeltaTime &&
                        Mathf.Abs(_ball.Rigidbody.linearVelocityX) <= Physics2D.linearSleepTolerance &&
                        actor.Actions.IsKickingContact(part);
                    if (outgoing.x * direction <= Physics2D.linearSleepTolerance && !startingFromRest)
                        continue;

                    // In a contested launch, let physics establish who played the ball.
                    if (striker != null)
                        return;
                    striker = actor;
                    launchVelocity = outgoing;
                    break;
                }
            }
            // Keep the second defender solid on an incoming shot until the ball actually turns.
            if (striker != null && _incomingVelocity.x * striker.Actions.AttackingDirection >= 0f)
                CheckReceiver(striker.Teammate, launchVelocity);
        }

        static bool IsSolid(Collider2D part) => part != null && part.isActiveAndEnabled && !part.isTrigger &&
            part.attachedRigidbody != null && part.attachedRigidbody.simulated;

        void ProtectCrowdedRebound()
        {
            // Before the solver, keep every potential inbound save solid. Only a limb
            // already on the exit side and separating from the incoming ball may be bypassed.
            var incoming = _ball.Rigidbody.linearVelocity;
            foreach (var rear in _team.Values)
            {
                if (rear.Actions == null || !rear.Actions.isActiveAndEnabled || !TeammateIsAhead(rear))
                    continue;
                int direction = rear.Actions.AttackingDirection;
                if (incoming.x * direction >= -Physics2D.linearSleepTolerance)
                    continue;
                foreach (var part in rear.Parts)
                {
                    if (!IsSolid(part) || Physics2D.GetIgnoreCollision(_ball.Collider, part))
                        continue;
                    var gap = _ball.Collider.Distance(part);
                    if (!gap.isValid || gap.normal.x * direction <= 0f || !CanMeetNextStep(part, gap, incoming))
                        continue;

                    var body = part.attachedRigidbody;
                    float approach = Vector2.Dot(body.GetPointVelocity(gap.pointB) - incoming, gap.normal);
                    float massShare = body.bodyType == RigidbodyType2D.Dynamic ? body.mass / (body.mass + _ball.Rigidbody.mass) : 1f;
                    float bounce = Mathf.Max(_ball.Collider.bounciness, part.bounciness);
                    Vector2 rebound = incoming + gap.normal * (Mathf.Max(0f, approach) * (1f + bounce) * massShare);
                    if (rebound.x * direction > Physics2D.linearSleepTolerance)
                        CheckReceiver(rear.Teammate, rebound, exitSideOnly: true);
                }
            }
        }

        // Returns true once the entire ball has cleared every solid part of the receiver.
        bool CheckReceiver(Participant receiver, Vector2 ballVelocity, bool exitSideOnly = false)
        {
            Bounds ballBounds = _ball.Collider.bounds;
            bool cleared = true;
            var actions = receiver.Actions;
            foreach (var part in receiver.Parts)
            {
                if (!IsSolid(part))
                    continue;

                Bounds bounds = part.bounds;
                if (actions.AttackingDirection > 0 ? ballBounds.min.x > bounds.max.x : ballBounds.max.x < bounds.min.x)
                    continue;
                cleared = false;

                var separation = _ball.Collider.Distance(part);
                if (!separation.isValid || !CanMeetNextStep(part, separation, ballVelocity))
                    continue;
                if (exitSideOnly && (separation.distance > 0f || separation.normal.x * actions.AttackingDirection >= 0f ||
                    Vector2.Dot(_ball.Rigidbody.linearVelocity - part.attachedRigidbody.GetPointVelocity(separation.pointB),
                        separation.normal) <= Physics2D.linearSleepTolerance))
                    continue;
                if (IsReceivingStrike(receiver, part, separation, ballVelocity))
                {
                    _intended.Add(part);
                    continue;
                }
                _needed.Add(part);
                if (!Physics2D.GetIgnoreCollision(_ball.Collider, part))
                {
                    Physics2D.IgnoreCollision(_ball.Collider, part);
                    var pair = (part, receiver);
                    if (!_ignored.Contains(pair))
                        _ignored.Add(pair);
                }
            }
            return cleared;
        }

        bool IsReceivingStrike(Participant actor, Collider2D part, ColliderDistance2D gap, Vector2 ballVelocity)
        {
            if (!gap.isValid)
                return false;
            Vector2 normal = gap.normal; // from the limb toward the ball
            Vector2 limbVelocity = part.attachedRigidbody.GetPointVelocity(gap.pointB);
            float closing = Vector2.Dot(limbVelocity - ballVelocity, normal);
            bool embedded = gap.distance < -Physics2D.defaultContactOffset;
            if (embedded && closing <= Physics2D.linearSleepTolerance)
                return false;

            if (part == actor.Head)
            {
                // A falling ball can meet a jumping head at its apex or on early descent.
                return actor.Actions.IsOnHeaderJump && _ball.Collider.bounds.center.y >= part.bounds.center.y &&
                       closing >= -Physics2D.linearSleepTolerance;
            }

            double age = Time.fixedTimeAsDouble - actor.Actions.LastKickTime;
            if (!actor.Actions.IsKickingContact(part) || age > ReceiveStrikeWindow)
                return false;
            Vector2 swing = limbVelocity - actor.Body.GetPointVelocity(gap.pointB);
            // The motor may not have moved yet on the input step. Inside an existing
            // overlap, require an actual swing into the ball before restoring the pair.
            return (actor.Actions.IsSwingingKickOutward && Vector2.Dot(swing, normal) > Physics2D.linearSleepTolerance) ||
                   (!embedded && age <= Time.fixedDeltaTime);
        }

        bool CanMeetNextStep(Collider2D part, ColliderDistance2D separation, Vector2 ballVelocity)
        {
            float skin = 2f * Physics2D.defaultContactOffset;
            if (separation.distance <= skin)
                return true;

            // Project relative motion onto the current separating axis. The rotational sweep
            // includes the far end of a swinging limb; translation includes a falling teammate.
            var body = part.attachedRigidbody;
            float dt = Time.fixedDeltaTime;
            Vector2 towardPart = (separation.pointB - separation.pointA).normalized;
            Vector2 relativeVelocity = ballVelocity - body.linearVelocity;
            Vector2 relativeAcceleration = Physics2D.gravity * _ball.Rigidbody.gravityScale + _ball.Rigidbody.totalForce / _ball.Rigidbody.mass;
            if (body.bodyType == RigidbodyType2D.Dynamic)
                relativeAcceleration -= Physics2D.gravity * body.gravityScale + body.totalForce / body.mass;
            float closing = Vector2.Dot(relativeVelocity + relativeAcceleration * dt, towardPart);
            Bounds bounds = part.bounds;
            float radius = Vector2.Distance(body.worldCenterOfMass, bounds.center) + ((Vector2)bounds.extents).magnitude;
            float rotationSpeed = Mathf.Abs(body.angularVelocity) * Mathf.Deg2Rad * radius;
            return separation.distance <= skin + (closing + rotationSpeed) * dt;
        }

        void RestorePairs(bool force)
        {
            for (int i = _ignored.Count - 1; i >= 0; i--)
            {
                var (part, owner) = _ignored[i];
                if (part != null && part.isActiveAndEnabled && _ball.Collider != null && _ball.Collider.isActiveAndEnabled)
                {
                    if (!force)
                    {
                        if (_needed.Contains(part) && !_intended.Contains(part))
                            continue;
                        // Direction can change while embedded. Restore untouched limbs
                        // immediately; let a passive overlap drain safely in either direction.
                        var separation = _ball.Collider.Distance(part);
                        if (separation.isValid && separation.distance < -Physics2D.defaultContactOffset &&
                            (owner.Actions == null || !IsReceivingStrike(owner, part, separation, _ball.Rigidbody.linearVelocity)))
                            continue;
                    }
                    Physics2D.IgnoreCollision(_ball.Collider, part, false);
                }
                _ignored.RemoveAt(i);
            }
        }

        internal void Reset()
        {
            _receiver = null;
            _incomingVelocity = Vector2.zero;
            _hasContacts = _hitBoundary = false;
            _needed.Clear();
            _intended.Clear();
            RestorePairs(force: true);
            foreach (var actor in _team.Values)
            {
                actor.Contact = default;
                if (actor.Actions != null)
                    actor.LastShotKick = actor.Actions.LastKickTime;
            }
        }
    }
}
