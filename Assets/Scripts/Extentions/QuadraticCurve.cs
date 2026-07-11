using UnityEngine;

public class QuadraticCurve
{
    public Vector3 A { get; private set; }
    public Vector3 B { get; private set; }
    public Vector3 Control { get; private set; }
    public QuadraticCurve(Vector3 A, Vector3 B, Vector3 Control)
    {
        this.A = A;
        this.B = B;
        this.Control = Control;
    }
    public Vector3 Evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(A, Control, t);
        Vector3 cb = Vector3.Lerp(Control, B, t);
        return Vector3.Lerp(ac, cb, t);
    }
    public static Vector3 Evaluate(Vector2 A, Vector2 Control, Vector2 B, float t)
    {
        Vector3 ac = Vector3.Lerp(A, Control, t);
        Vector3 cb = Vector3.Lerp(Control, B, t);
        return Vector3.Lerp(ac, cb, t);
    }
    public void DrawPathGizmos()
    {
        if (A == B && B == Control)
        {
            return;
        }

        for (int i = 0; i < 20; i++)
        {
            Gizmos.DrawWireSphere(Evaluate(i / 20f), 0.1f);
        }
    }
}
