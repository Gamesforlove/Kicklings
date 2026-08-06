using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClearSavedData))]
public class ClearSavedDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ClearSavedData script = (ClearSavedData)target;

        GUILayout.Space(10); 

        if (GUILayout.Button("Clear Saved Data"))
        {
            script.ClearData();

            Debug.Log("Saved data has been cleared!");
        }
    }
}
