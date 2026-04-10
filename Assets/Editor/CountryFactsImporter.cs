using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CountryFactsImporter : EditorWindow
{
    private string csvPath = "Assets/Editor/CountryFacts.csv";
    private string outputPath = "Assets/Scripts/CommonDataTypes/CountryFacts.asset";

    [MenuItem("Tools/Import Country Facts from CSV")]
    public static void ShowWindow() => GetWindow<CountryFactsImporter>("Country Facts Importer");

    private void OnGUI()
    {
        GUILayout.Label("CSV Import Settings", EditorStyles.boldLabel);
        csvPath = EditorGUILayout.TextField("CSV Path", csvPath);
        outputPath = EditorGUILayout.TextField("Output Asset Path", outputPath);

        if (GUILayout.Button("Import"))
            ImportCSV();
    }

    private void ImportCSV()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSV not found at: {csvPath}");
            return;
        }

        var lines = File.ReadAllLines(csvPath);

        // Пропускаем заголовок, группируем факты по стране
        var grouped = lines
            .Skip(1)
            .Select(line => ParseCSVLine(line))
            .Where(cols => cols.Count >= 2 && !string.IsNullOrWhiteSpace(cols[0]))
            .GroupBy(cols => cols[0].Trim())
            .ToDictionary(
                g => g.Key,
                g => g.Select(cols => cols[1].Trim()).ToList()
            );

        var db = AssetDatabase.LoadAssetAtPath<CountryFacts>(outputPath);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<CountryFacts>();
            AssetDatabase.CreateAsset(db, outputPath);
        }

        var so = new SerializedObject(db);
        var countriesProp = so.FindProperty("Countries");
        countriesProp.ClearArray();

        int i = 0;
        foreach (var kvp in grouped)
        {
            countriesProp.InsertArrayElementAtIndex(i);
            var element = countriesProp.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("countryName").stringValue = kvp.Key;

            var factsProp = element.FindPropertyRelative("facts");
            factsProp.ClearArray();
            for (int j = 0; j < kvp.Value.Count; j++)
            {
                factsProp.InsertArrayElementAtIndex(j);
                factsProp.GetArrayElementAtIndex(j).stringValue = kvp.Value[j];
            }
            i++;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {grouped.Count} cointries, {grouped.Values.Sum(f => f.Count)} facts -> {outputPath}");
    }

    private List<string> ParseCSVLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ';' && !inQuotes) { result.Add(current.ToString()); current.Clear(); }
            else current.Append(c);
        }
        result.Add(current.ToString());
        return result;
    }
}
