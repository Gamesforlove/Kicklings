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

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Load / Refresh Data", GUILayout.Height(30)))
        {
            loadedData = SaveLoadGame.GetLastSavedData();
        }

        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("Clear Saved Data", GUILayout.Height(30)))
        {
            StorageData emptyData = new StorageData();
            SaveLoadGame.Save(emptyData);

            loadedData = emptyData;
            Debug.Log("Saved data cleared successfully!");
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (loadedData == null)
        {
            EditorGUILayout.HelpBox("Data is not loaded or save file does not exist.", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Display basic fields
        EditorGUILayout.LabelField("Campaign Progress", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current Player Level", loadedData.PlayerLevel.ToString());
        EditorGUILayout.LabelField("Last Unlocked Level", loadedData.lastUnlockedLevel.ToString());
        EditorGUILayout.LabelField("Stage", loadedData.stage.ToString());
        EditorGUILayout.LabelField("Scene", loadedData.scene.ToString());

        EditorGUILayout.Space(10);

        EditorGUILayout.EndScrollView();
    }
}