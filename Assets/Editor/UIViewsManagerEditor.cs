using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UI;
using UI.UiSystem;

[CustomEditor(typeof(UIViewsManager))]
public class UIViewsManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var manager = (UIViewsManager)target;
        Stack<UIView> stack = manager.GetDebugStack();

        GUILayout.Space(10);
        GUILayout.Label("Views History", EditorStyles.boldLabel);

        foreach (UIView view in stack)
        {
            GUILayout.Label(view != null ? view.name : "(null)");
        }
    }
}