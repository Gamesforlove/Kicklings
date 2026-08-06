using UnityEditor;
using UnityEngine;
using SaveSystem;

public class SaveDataViewerWindow : EditorWindow
{
    private StorageData loadedData;
    private bool showAbilities = true;
    private Vector2 scrollPosition;

    // Add menu item to open the window
    [MenuItem("Tools/Save Data Viewer")]
    public static void ShowWindow()
    {
        GetWindow<SaveDataViewerWindow>("Save Data Viewer");
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        // Button to trigger loading of the save data
        if (GUILayout.Button("Load / Refresh Data", GUILayout.Height(30)))
        {
            // Replace with your actual load method if it differs
            if (SaveLoadGame.Load())
            {
                loadedData = SaveLoadGame.LoadedData;
            }
        }

        if (loadedData == null)
        {
            EditorGUILayout.HelpBox("Data is not loaded or save file does not exist.", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Display basic fields
        EditorGUILayout.LabelField("Campaign Progress", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current Player Level", loadedData.currentPlayerLevel.ToString());
        EditorGUILayout.LabelField("Last Unlocked Level", loadedData.lastUnlockedLevel.ToString());
        EditorGUILayout.LabelField("Stage", loadedData.stage.ToString());
        EditorGUILayout.LabelField("Scene", loadedData.scene.ToString());

        EditorGUILayout.Space(10);

/*        // Display Dictionary entries
        showAbilities = EditorGUILayout.Foldout(showAbilities, $"Abilities ({loadedData.abilities?.Count ?? 0})", true);
        if (showAbilities && loadedData.abilities != null)
        {
            EditorGUI.indentLevel++;
            foreach (var ability in loadedData.abilities)
            {
                EditorGUILayout.LabelField($"ID: {ability.Key}", ability.Value);
            }
            EditorGUI.indentLevel--;
        }*/

        EditorGUILayout.EndScrollView();
    }
}