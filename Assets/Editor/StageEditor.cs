using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stage))]
public class StageEditor : UnityEditor.Editor
{
    private SerializedProperty stageTypeProp;
    private SerializedProperty levelsProp;

    private void OnEnable()
    {
        stageTypeProp = serializedObject.FindProperty("<StageType>k__BackingField");
        levelsProp = serializedObject.FindProperty("levels");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(stageTypeProp);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            Stage stage = (Stage)target;
            stage.UpdateBehavior();
            EditorUtility.SetDirty(stage);
        }

        EditorGUILayout.PropertyField(levelsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
