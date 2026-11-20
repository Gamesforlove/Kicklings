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
            internal readonly Collider2D[] Parts;
            internal readonly Collider2D Head;
            internal Participant Teammate;
            internal double LastShotKick;

            internal Participant(PlayerActions actions)
            {
                Actions = actions;
                Parts = actions.GetComponentsInChildren<Collider2D>(true);
                Head = actions.HeadContactCollider;
                LastShotKick = actions.LastKickTime;
            }
        }

        readonly BallScript _ball;
        readonly Dictionary<PlayerActions, Participant> _team = new(4);
        readonly List<(Collider2D part, int teamDirection)> _ignored = new();
        readonly HashSet<Collider2D> _needed = new();
        Participant _receiver;
        double _launchTime;
        float _incomingVelocityX;

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
            if (!_team.TryGetValue(actions, out var actor))
                return;

            // All contacts in one solver step see the same outgoing velocity. A trailing
            // callback from the other team must not cancel a clearance confirmed this step.
            if (_receiver != null && actor != _receiver.Teammate && _launchTime != Time.fixedTimeAsDouble)
                _receiver = null;

            if (_ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic ||
                _ball.Rigidbody.linearVelocityX * actions.AttackingDirection <= Physics2D.linearSleepTolerance)
                return;

            bool kicked = HasUnusedKick(actor) && (assistedKick || part == actor.Head || actions.IsKickingContact(part));
            if (kicked)
                actor.LastShotKick = actions.LastKickTime;
            else
            {
                // A new impact can send the ball upfield off any body part. A continuing
                // contact can also turn it upfield, but must not restart the receiver's timing window.
                bool wasMovingUpfield = _incomingVelocityX * actions.AttackingDirection > Physics2D.linearSleepTolerance;
                if ((wasMovingUpfield && (!firstContact || _receiver == actor.Teammate)) || !HasImpact(collision))
                    return;
            }
            _receiver = actor.Teammate;
            _launchTime = Time.fixedTimeAsDouble;
        }

        static bool HasImpact(Collision2D collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
                if (collision.GetContact(i).normalImpulse > 0f)
                    return true;
            return false;
        }

        internal void BeforePhysicsStep()
        {
            if (!_ball.Collider.isActiveAndEnabled || !_ball.Rigidbody.simulated ||
                _ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            {
                Reset();
                return;
            }

            _incomingVelocityX = _ball.Rigidbody.linearVelocityX;
            _needed.Clear();
            if (_receiver != null && _receiver.Actions != null && _receiver.Actions.isActiveAndEnabled &&
                _ball.Rigidbody.linearVelocityX * _receiver.Actions.AttackingDirection > Physics2D.linearSleepTolerance)
            {
                if (CheckReceiver(_receiver, _launchTime, _ball.Rigidbody.linearVelocity))
                    _receiver = null;
            }
            else
                _receiver = null;

            if (_receiver == null)
                ProtectPointBlankKick();
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
                if (actor.Actions == null || !actor.Actions.isActiveAndEnabled || !HasUnusedKick(actor) ||
                    Time.fixedTimeAsDouble - actor.Actions.LastKickTime > ReceiveStrikeWindow || !TeammateIsAhead(actor))
                    continue;

                foreach (var part in actor.Parts)
                {
                    if (!IsSolid(part) || (part != actor.Head && !actor.Actions.IsKickingContact(part)))
                        continue;
                    var gap = _ball.Collider.Distance(part);
                    if (!gap.isValid || gap.distance > 2f * Physics2D.defaultContactOffset)
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
            if (striker != null && _incomingVelocityX * striker.Actions.AttackingDirection >= 0f)
                CheckReceiver(striker.Teammate, striker.Actions.LastKickTime, launchVelocity);
        }

        static bool IsSolid(Collider2D part) => part != null && part.isActiveAndEnabled && !part.isTrigger &&
            part.attachedRigidbody != null && part.attachedRigidbody.simulated;

        // Returns true once the entire ball has cleared every solid part of the receiver.
        bool CheckReceiver(Participant receiver, double launchTime, Vector2 ballVelocity)
        {
            Bounds ballBounds = _ball.Collider.bounds;
            bool cleared = true;
            var actions = receiver.Actions;
            bool freshStrike = actions.LastKickTime >= launchTime - Time.fixedDeltaTime &&
                Time.fixedTimeAsDouble - actions.LastKickTime <= ReceiveStrikeWindow;
            foreach (var part in receiver.Parts)
            {
                if (!IsSolid(part))
                    continue;

                Bounds bounds = part.bounds;
                if (actions.AttackingDirection > 0 ? ballBounds.min.x > bounds.max.x : ballBounds.max.x < bounds.min.x)
                    continue;
                cleared = false;

                if (freshStrike && actions.IsKickingContact(part))
                    continue;
                // A jump already rising into an overhead ball can be a header. Merely landing
                // on the shot from an earlier jump is a passive obstruction.
                if (part == receiver.Head)
                {
                    bool aboveHead = ballBounds.center.y >= bounds.center.y;
                    bool risingHeader = aboveHead && actions.IsWithinActionContactWindow && part.attachedRigidbody.linearVelocityY > 0f;
                    if (risingHeader || (freshStrike && (aboveHead || part.attachedRigidbody.linearVelocityY >= 0f)))
                        continue;
                }

                var separation = _ball.Collider.Distance(part);
                if (separation.isValid && CanMeetNextStep(part, separation, ballVelocity))
                {
                    _needed.Add(part);
                    if (!Physics2D.GetIgnoreCollision(_ball.Collider, part))
                    {
                        Physics2D.IgnoreCollision(_ball.Collider, part);
                        var pair = (part, actions.AttackingDirection);
                        if (!_ignored.Contains(pair))
                            _ignored.Add(pair);
                    }
                }
            }
            return cleared;
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
                var (part, teamDirection) = _ignored[i];
                if (part != null && part.isActiveAndEnabled && _ball.Collider != null && _ball.Collider.isActiveAndEnabled)
                {
                    if (!force && _ball.Rigidbody.linearVelocityX * teamDirection >= 0f)
                    {
                        if (_needed.Contains(part))
                            continue;
                        // Allow an outward ball to separate cleanly. A reversal restores
                        // defensive contact immediately, even with the ball still inside the player.
                        var separation = _ball.Collider.Distance(part);
                        if (separation.isValid && separation.distance < -Physics2D.defaultContactOffset)
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
            _incomingVelocityX = 0f;
            _needed.Clear();
            RestorePairs(force: true);
            foreach (var actor in _team.Values)
                if (actor.Actions != null)
                    actor.LastShotKick = actor.Actions.LastKickTime;
        }
    }
}
