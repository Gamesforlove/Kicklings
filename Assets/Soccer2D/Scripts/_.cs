using UnityEngine;
using System.Collections;

public static class _
{
    static public void Assert(bool expression)
    {
        if (!expression)
        {
#if UNITY_EDITOR
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            System.Diagnostics.StackFrame stackFrame = stackTrace.GetFrame(1);
            
            string fileName = stackFrame.GetFileName();
            int lineNumber = stackFrame.GetFileLineNumber();
            
            string message = "Assertion failed: file " + fileName + ", line " + lineNumber;
            
            Debug.LogError(message);
            
            if (UnityEditor.EditorUtility.DisplayDialog("Assertion failed", message, "Break", "Ignore"))
            {
                Debug.Break();
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fileName, lineNumber);
            }
#else
            Debug.LogError("Assertion failed");
#endif
        }
    }

    static public float fmod(float a, float b)
    {
        return a - Mathf.Floor(a / b) * b;
    }

    static public float MultipleCeil(float x, float m)
    {
        return Mathf.Ceil(x/m)*m;
    }

    static public double MultipleCeil(double x, double m)
    {
        return System.Math.Ceiling(x/m)*m;
    }

    static public float SafeClamp01(float value)
    {
        if (float.IsNaN(value))
        {
            return 0.0f;
        }

        return Mathf.Clamp01(value);
    }

    static public float Sqr(float x)
    {
        return x*x;
    }

    static public Vector2 Clamp(Vector2 v, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
    }

    static public Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
    }

    static public float WrapToRange(float x, float min, float max)
    {
        float t = (x - min)/(max - min);
        t = t - Mathf.Floor(t);
        return min + t*(max - min);
    }

    static public Vector2 ToVector2XZ(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    static public Vector3 CartesianToSpherical(Vector3 xyz)
    {
        float r = xyz.magnitude;
        float theta = Mathf.Acos(xyz.z/r)*Mathf.Rad2Deg;
        float phi = Mathf.Atan2(xyz.y, xyz.x)*Mathf.Rad2Deg;
        
        return new Vector3(r, theta, phi);
    }
    
    static public Vector3 SphericalToCartesian(Vector3 rThetaPhi)
    {
        float r = rThetaPhi.x;
        float sinTheta = Mathf.Sin(rThetaPhi.y*Mathf.Deg2Rad);
        float cosTheta = Mathf.Cos(rThetaPhi.y*Mathf.Deg2Rad);
        float sinPhi = Mathf.Sin(rThetaPhi.z*Mathf.Deg2Rad);
        float cosPhi = Mathf.Cos(rThetaPhi.z*Mathf.Deg2Rad);
        
        return new Vector3(r*sinTheta*cosPhi, r*sinTheta*sinPhi, r*cosTheta);
    }

    static public Vector3 CartesianToSphericalXZY(Vector3 xyz)
    {
        float r = xyz.magnitude;
        float theta = Mathf.Acos(xyz.y/r)*Mathf.Rad2Deg;
        float phi = Mathf.Atan2(xyz.z, xyz.x)*Mathf.Rad2Deg;

        return new Vector3(r, theta, phi);
    }

    static public Vector3 SphericalXZYToCartesian(Vector3 rThetaPhi)
    {
        float r = rThetaPhi.x;
        float sinTheta = Mathf.Sin(rThetaPhi.y*Mathf.Deg2Rad);
        float cosTheta = Mathf.Cos(rThetaPhi.y*Mathf.Deg2Rad);
        float sinPhi = Mathf.Sin(rThetaPhi.z*Mathf.Deg2Rad);
        float cosPhi = Mathf.Cos(rThetaPhi.z*Mathf.Deg2Rad);

        return new Vector3(r*sinTheta*cosPhi, r*cosTheta, r*sinTheta*sinPhi);
    }

    static public Vector3 LerpInSphericalXZY(Vector3 from, Vector3 to, float t)
    {
        Vector3 sphericalFrom = _.CartesianToSphericalXZY(from);
        Vector3 sphericalTo = _.CartesianToSphericalXZY(to);

        Vector3 sphericalResult = new Vector3(Mathf.Lerp(sphericalFrom.x, sphericalTo.x, t),
                                              Mathf.LerpAngle(sphericalFrom.y, sphericalTo.y, t),
                                              Mathf.LerpAngle(sphericalFrom.z, sphericalTo.z, t));

        return _.SphericalXZYToCartesian(sphericalResult);
    }

    static public T Find<T>(Transform transform, string name) where T : Component
    {
        T[] components = transform.GetComponentsInChildren<T>();
        foreach (T component in components)
        {
            if (component.gameObject.name == name)
            {
                return component;
            }
        }

        return null;
    }

    static public T Instantiate<T>(GameObject prefab, string name, Transform parent = null) where T : Component
    {
        GameObject gameObject = Object.Instantiate(prefab) as GameObject;
        gameObject.name = name;

        Transform transform = gameObject.transform;

        Vector3 position = transform.localPosition;
        Quaternion rotation = transform.localRotation;
        Vector3 scale = transform.localScale;

        transform.SetParent(parent);

        transform.localPosition = position;
        transform.localRotation = rotation;
        transform.localScale = scale;

        return gameObject.GetComponent<T>();
    }

    static public T Instantiate<T>(string resourcePath, string name, Transform parent = null) where T : Component
    {
        GameObject prefab = (GameObject)Resources.Load(resourcePath);
        return Instantiate<T>(prefab, name, parent);
    }

    static public void SetLayerRecursively(GameObject gameObject, int layer)
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    static public void DestroyAllChildren(Transform t)
    {
        Transform[] children = t.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child == t)
            {
                continue;
            }

            child.SetParent(null);
            Object.Destroy(child.gameObject);
        }
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
    {
        parent.layer = layer;
        if (includeChildren)
        {
            foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
    }
}
