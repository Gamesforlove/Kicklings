using UnityEngine;

#if UNITY_EDITOR
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor")]
#endif

namespace Gameplay
{
    // One player's contacts during a solver step, projected along their attacking direction.
    internal struct PassContact
    {
        internal bool Touched, Entered, CanLaunch, Played;
        internal double KickTime;
        internal float ContactDepth, MinImpulse, MaxImpulse, UpfieldSurfaceSpeed;

        internal void RecordImpulse(float depth, float normalImpulse, float frictionBound)
        {
            if (!Touched || depth < ContactDepth)
                ContactDepth = depth;
            Touched = true;
            MinImpulse += normalImpulse - frictionBound;
            MaxImpulse += normalImpulse + frictionBound;
        }

        internal bool Precedes(PassContact other, float relativeBodyDepth)
        {
            if (Played != other.Played)
                return Played;
            if (ContactDepth != other.ContactDepth)
                return ContactDepth < other.ContactDepth;
            return relativeBodyDepth < 0f;
        }

        internal float ClearanceSpeed(PassContact front, float incoming, float outgoing, float mass,
            float incomingSpeed, float tolerance)
        {
            if (outgoing > tolerance || !front.Touched || front.Played ||
                ContactDepth >= front.ContactDepth || front.MaxImpulse >= 0f || MinImpulse <= 0f)
                return outgoing;

            // Lower bound on the rear save's upfield speed with the passive front block
            // removed. Bound it by the measured shot or striking surface speed so that
            // compression alone cannot launch a stationary ball.
            float clearance = Mathf.Min(incoming + MinImpulse / mass, outgoing - front.MaxImpulse / mass);
            clearance = Mathf.Min(clearance, Mathf.Max(incomingSpeed, UpfieldSurfaceSpeed));
            return clearance > tolerance ? clearance : outgoing;
        }
    }
}
