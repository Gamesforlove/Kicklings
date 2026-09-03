using UnityEditor;

[CustomEditor(typeof(CampaignLevelData))]
public class CampaignLevelDataEditor : Editor
{
    private SerializedProperty customBehaviourAfterLevelProp;
    private SerializedProperty insertStageProp;
    private SerializedProperty player1Prop;
    private SerializedProperty player2Prop;
    private SerializedProperty opponent1Prop;
    private SerializedProperty opponent2Prop;
    private SerializedProperty preMatchCutSceneProp;
    private SerializedProperty afterMatchCutSceneProp;
    private SerializedProperty afterMatchDefeatCutSceneProp;
    private SerializedProperty atutorialMatchProp;
    private SerializedProperty levelGameplaySceneProp;

    private void OnEnable()
    {
        customBehaviourAfterLevelProp = serializedObject.FindProperty("<CustomBehaviourAfterLevel>k__BackingField");
        insertStageProp = serializedObject.FindProperty("insertStage");

        player1Prop = serializedObject.FindProperty("<Player1>k__BackingField");
        player2Prop = serializedObject.FindProperty("<Player2>k__BackingField");
        opponent1Prop = serializedObject.FindProperty("<Opponent1>k__BackingField");
        opponent2Prop = serializedObject.FindProperty("<Opponent2>k__BackingField");
        preMatchCutSceneProp = serializedObject.FindProperty("<PreMatchCutScene>k__BackingField");
        afterMatchCutSceneProp = serializedObject.FindProperty("<AfterMatchCutScene>k__BackingField");
        afterMatchDefeatCutSceneProp = serializedObject.FindProperty("<AfterMatchDefeatCutScene>k__BackingField");
        atutorialMatchProp = serializedObject.FindProperty("<TutorialMatch>k__BackingField");
        levelGameplaySceneProp = serializedObject.FindProperty("<LevelGameplayScene>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(player1Prop);
        EditorGUILayout.PropertyField(player2Prop);
        EditorGUILayout.PropertyField(opponent1Prop);
        EditorGUILayout.PropertyField(opponent2Prop);
        EditorGUILayout.PropertyField(preMatchCutSceneProp);
        EditorGUILayout.PropertyField(afterMatchCutSceneProp);
        EditorGUILayout.PropertyField(afterMatchDefeatCutSceneProp);
        EditorGUILayout.PropertyField(atutorialMatchProp);
        EditorGUILayout.PropertyField(levelGameplaySceneProp);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(customBehaviourAfterLevelProp);

        if (customBehaviourAfterLevelProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(insertStageProp);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}