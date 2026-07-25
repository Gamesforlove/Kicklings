#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(StageFlowSO))]
public class StageFlowEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var flow = (StageFlowSO)target;
        if (flow == null || flow.scenes == null) return;

        var referencedPaths = new HashSet<string>();
        foreach (var info in flow.scenes)
        {
            if (info == null) continue;

            // Prefer the serialized runtime path
            var p = info.Path;
            if (!string.IsNullOrEmpty(p))
                referencedPaths.Add(p);
        }

        if (referencedPaths.Count == 0) return;

        // Analyze Build Settings issues
        var buildScenes = EditorBuildSettings.scenes.ToList();

        var buildPathsAll = buildScenes
            .Where(s => s != null && !string.IsNullOrEmpty(s.path))
            .Select(s => s.path)
            .ToList();

        var buildPathCounts = buildPathsAll
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());

        bool hasDuplicates = buildPathCounts.Values.Any(c => c > 1);

        bool hasMissing = referencedPaths.Any(path => !buildPathCounts.ContainsKey(path));
        bool hasDisabled = referencedPaths.Any(path =>
        {
            // disabled means: it exists, but at least one entry is disabled AND none enabled
            var matches = buildScenes.Where(s => s != null && s.path == path).ToList();
            if (matches.Count == 0) return false;
            return !matches.Any(s => s.enabled);
        });

        bool needsFix = hasDuplicates || hasMissing || hasDisabled;

        EditorGUILayout.Space(10);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);

            if (!needsFix)
            {
                EditorGUILayout.HelpBox("✅ All referenced scenes are present, enabled, and unique.", MessageType.Info);
            }
            else
            {
                var problems = new List<string>();
                if (hasMissing) problems.Add("missing");
                if (hasDisabled) problems.Add("disabled");
                if (hasDuplicates) problems.Add("duplicates");
                EditorGUILayout.HelpBox($"Build Settings has issues: {string.Join(", ", problems)}.", MessageType.Warning);

                if (GUILayout.Button("Fix Build Settings (Add/Enable/Duplicate)"))
                {
                    FixBuildSettings(referencedPaths);
                }
            }
        }
    }

    private static void FixBuildSettings(HashSet<string> referencedPaths)
    {
        var scenes = EditorBuildSettings.scenes.ToList();

        // 1) Remove duplicates (keep first occurrence)
        var seen = new HashSet<string>();
        var deduped = new List<EditorBuildSettingsScene>();

        foreach (var s in scenes)
        {
            if (s == null || string.IsNullOrEmpty(s.path)) continue;
            if (seen.Add(s.path))
                deduped.Add(s);
        }

        scenes = deduped;

        // Rebuild lookup
        var indexByPath = scenes
            .Select((s, i) => new { s.path, i })
            .ToDictionary(x => x.path, x => x.i);

        int added = 0;
        int enabled = 0;

        // 2) Add missing + enable referenced
        foreach (var path in referencedPaths)
        {
            if (!indexByPath.TryGetValue(path, out int idx))
            {
                scenes.Add(new EditorBuildSettingsScene(path, true));
                indexByPath[path] = scenes.Count - 1;
                added++;
            }
            else
            {
                if (!scenes[idx].enabled)
                {
                    scenes[idx] = new EditorBuildSettingsScene(path, true);
                    enabled++;
                }
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();

        Debug.Log($"[StageFlowEditor] Fixed Build Settings. Added={added}, Enabled={enabled}, Deduped=true");
    }
}
#endif
