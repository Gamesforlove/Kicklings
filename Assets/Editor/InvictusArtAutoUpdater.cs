using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
public static class InvictusArtAutoUpdater
{
    private const string PrefKey = "InvictusArt.AutoUpdateOnLaunch";

    private static readonly PackageTarget[] Targets =
    {
        new PackageTarget(
            "com.invictus.art.common",
            "https://github.com/Gamesforlove/Kickling-Art.git?path=/KicklingsArt/Assets/Runtime-Assets/Common#main"
        ),
        new PackageTarget(
            "com.invictus.art.campaign",
            "https://github.com/Gamesforlove/Kickling-Art.git?path=/KicklingsArt/Assets/Runtime-Assets/Campaign#main"
        )
    };

    private static bool _started;
    private static int _currentTargetIndex = -1;
    private static AddRequest _addRequest;

    private static readonly Dictionary<string, string> RevBefore = new();
    private static readonly Dictionary<string, HashSet<string>> FilesBefore = new();

    private class PackageTarget
    {
        public string Name;
        public string Url;

        public PackageTarget(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }

    static InvictusArtAutoUpdater()
    {
        if (!EditorPrefs.HasKey(PrefKey))
            EditorPrefs.SetBool(PrefKey, true);

        EditorApplication.delayCall += TryAutoUpdateOnce;
    }

    [MenuItem("Tools/Packages/Invictus Art/Auto-update on launch")]
    private static void Toggle()
    {
        bool enabled = EditorPrefs.GetBool(PrefKey, true);
        EditorPrefs.SetBool(PrefKey, !enabled);
    }

    [MenuItem("Tools/Packages/Invictus Art/Auto-update on launch", true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked("Tools/Packages/Invictus Art/Auto-update on launch",
            EditorPrefs.GetBool(PrefKey, true));
        return true;
    }

    [MenuItem("Tools/Packages/Invictus Art/Update now")]
    private static void UpdateNow()
    {
        BeginUpdateSequence();
    }

    private static void TryAutoUpdateOnce()
    {
        if (_started) return;
        _started = true;

        if (!EditorPrefs.GetBool(PrefKey, true)) return;
        if (Application.isBatchMode) return;

        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            EditorApplication.delayCall += TryAutoUpdateOnce;
            return;
        }

        BeginUpdateSequence();
    }

    private static void BeginUpdateSequence()
    {
        RevBefore.Clear();
        FilesBefore.Clear();

        foreach (var target in Targets)
        {
            RevBefore[target.Name] = GetLockedRevisionSafe(target.Name);
            FilesBefore[target.Name] = SnapshotPackageFiles(target.Name);
            Debug.Log($"Invictus Art: Checking {target.Name}... (before rev: {Short(RevBefore[target.Name])})");
        }

        _currentTargetIndex = -1;
        UpdateNextTarget();
    }

    private static void UpdateNextTarget()
    {
        _currentTargetIndex++;

        if (_currentTargetIndex >= Targets.Length)
        {
            Debug.Log("Invictus Art: All package updates complete.");
            return;
        }

        var target = Targets[_currentTargetIndex];
        _addRequest = Client.Add(target.Url);
        EditorApplication.update += PollAdd;
    }

    private static void PollAdd()
    {
        if (_addRequest == null || !_addRequest.IsCompleted) return;

        EditorApplication.update -= PollAdd;

        var target = Targets[_currentTargetIndex];

        if (_addRequest.Status != StatusCode.Success)
        {
            Debug.LogError($"Invictus Art update failed for {target.Name}: " + _addRequest.Error.message);
            ShowNotification($"Update FAILED: {target.Name}");
            _addRequest = null;
            UpdateNextTarget();
            return;
        }

        _addRequest = null;
        EditorApplication.delayCall += () =>
        {
            OnUpdateFinished(target);
            UpdateNextTarget();
        };
    }

    private static void OnUpdateFinished(PackageTarget target)
    {
        string revBefore = RevBefore.TryGetValue(target.Name, out var rb) ? rb : null;
        var filesBefore = FilesBefore.TryGetValue(target.Name, out var fb) ? fb : null;

        string revAfter = GetLockedRevisionSafe(target.Name);
        var filesAfter = SnapshotPackageFiles(target.Name);

        Debug.Log($"{target.Name}: resolved rev {Short(revBefore)} → {Short(revAfter)}");

        var added = new List<string>();
        var removed = new List<string>();

        if (filesBefore != null && filesAfter != null)
        {
            foreach (var f in filesAfter)
                if (!filesBefore.Contains(f)) added.Add(f);

            foreach (var f in filesBefore)
                if (!filesAfter.Contains(f)) removed.Add(f);
        }

        bool changed = !StringEquals(revBefore, revAfter) || added.Count > 0 || removed.Count > 0;

        if (!changed)
        {
            Debug.Log($"{target.Name} already up to date.");
            return;
        }

        Debug.Log($"{target.Name} updated. Added: {added.Count}, Removed: {removed.Count}");

        foreach (var a in Limit(added, 25)) Debug.Log($"  [{target.Name}] + " + a);
        foreach (var r in Limit(removed, 25)) Debug.Log($"  [{target.Name}] - " + r);

        if (added.Count > 25 || removed.Count > 25)
            Debug.Log($"[{target.Name}] (Showing up to 25 of each. Total Added={added.Count}, Removed={removed.Count})");

        ShowNotification($"{target.Name} updated ({Short(revAfter)})");
    }

    private static HashSet<string> SnapshotPackageFiles(string packageName)
    {
        try
        {
            string cacheRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "PackageCache");
            if (!Directory.Exists(cacheRoot)) return null;

            string prefix = packageName + "@";
            var dirs = Directory.GetDirectories(cacheRoot, prefix + "*");
            if (dirs.Length == 0) return null;

            string pkgDir = dirs[0];
            DateTime best = Directory.GetLastWriteTimeUtc(pkgDir);
            for (int i = 1; i < dirs.Length; i++)
            {
                var t = Directory.GetLastWriteTimeUtc(dirs[i]);
                if (t > best)
                {
                    best = t;
                    pkgDir = dirs[i];
                }
            }

            var set = new HashSet<string>();
            foreach (var file in Directory.GetFiles(pkgDir, "*", SearchOption.AllDirectories))
            {
                var rel = file.Substring(pkgDir.Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                set.Add(rel.Replace('\\', '/'));
            }

            return set;
        }
        catch
        {
            return null;
        }
    }

    private static string GetLockedRevisionSafe(string packageName)
    {
        string lockPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "packages-lock.json");
        if (!File.Exists(lockPath))
            return null;

        string json = File.ReadAllText(lockPath);

        int pkgIndex = json.IndexOf($"\"{packageName}\"", StringComparison.Ordinal);
        if (pkgIndex < 0) return null;

        string value = ExtractValue(json, pkgIndex, "revision");
        if (!string.IsNullOrEmpty(value))
            return value;

        value = ExtractValue(json, pkgIndex, "hash");
        if (!string.IsNullOrEmpty(value))
            return value;

        return null;
    }

    private static string ExtractValue(string json, int startIndex, string key)
    {
        int keyIndex = json.IndexOf($"\"{key}\"", startIndex, StringComparison.Ordinal);
        if (keyIndex < 0) return null;

        int colon = json.IndexOf(':', keyIndex);
        if (colon < 0) return null;

        int q1 = json.IndexOf('"', colon + 1);
        if (q1 < 0) return null;

        int q2 = json.IndexOf('"', q1 + 1);
        if (q2 < 0) return null;

        return json.Substring(q1 + 1, q2 - q1 - 1);
    }

    private static void ShowNotification(string text)
    {
        EditorApplication.delayCall += () =>
        {
            var window = EditorWindow.focusedWindow;
            if (window != null)
                window.ShowNotification(new GUIContent(text));
        };
    }

    private static IEnumerable<string> Limit(List<string> list, int max)
    {
        int n = Mathf.Min(max, list.Count);
        for (int i = 0; i < n; i++)
            yield return list[i];
    }

    private static bool StringEquals(string a, string b) =>
        string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    private static string Short(string sha) =>
        string.IsNullOrEmpty(sha) ? "(none)" : (sha.Length <= 8 ? sha : sha.Substring(0, 8));
}