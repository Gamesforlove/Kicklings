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
    private const string PackageName = "com.invictus.art";

    // Must match your manifest entry
    private const string PackageUrl =
        "https://github.com/Gamesforlove/Kickling-Art.git?path=/KicklingsArt/Assets/Runtime-Assets#main";

    private static AddRequest _addRequest;
    private static bool _started;

    private static string _revBefore;
    private static HashSet<string> _filesBefore;

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
        CaptureBeforeState();
        StartUpdate();
    }

    private static void TryAutoUpdateOnce()
    {
        if (_started) return;
        _started = true;

        if (!EditorPrefs.GetBool(PrefKey, true)) return;
        if (Application.isBatchMode) return;

        // Avoid updating during compile/playmode; retry next tick.
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            EditorApplication.delayCall += TryAutoUpdateOnce;
            return;
        }

        CaptureBeforeState();
        Debug.Log($"Invictus Art: Checking for updates... (before rev: {Short(_revBefore)})");
        StartUpdate();
    }

    private static void CaptureBeforeState()
    {
        _revBefore = GetLockedRevisionSafe(PackageName);
        _filesBefore = SnapshotPackageFiles(PackageName);
    }

    private static void StartUpdate()
    {
        _addRequest = Client.Add(PackageUrl);
        EditorApplication.update += PollAdd;
    }

    private static void PollAdd()
    {
        if (_addRequest == null || !_addRequest.IsCompleted) return;

        EditorApplication.update -= PollAdd;

        if (_addRequest.Status != StatusCode.Success)
        {
            Debug.LogError("Invictus Art update failed: " + _addRequest.Error.message);
            ShowNotification("Art package update FAILED (see Console)");
            _addRequest = null;
            return;
        }

        // Give Unity a moment to write packages-lock.json / refresh PackageCache
        EditorApplication.delayCall += OnUpdateFinished;
        _addRequest = null;
    }

    private static void OnUpdateFinished()
    {
        string revAfter = GetLockedRevisionSafe(PackageName);
        var filesAfter = SnapshotPackageFiles(PackageName);

        Debug.Log($"Invictus Art: resolved rev {Short(_revBefore)} → {Short(revAfter)}");

        var added = new List<string>();
        var removed = new List<string>();

        if (_filesBefore != null && filesAfter != null)
        {
            foreach (var f in filesAfter)
                if (!_filesBefore.Contains(f)) added.Add(f);

            foreach (var f in _filesBefore)
                if (!filesAfter.Contains(f)) removed.Add(f);
        }

        bool changed = !StringEquals(_revBefore, revAfter) || added.Count > 0 || removed.Count > 0;

        if (!changed)
        {
            Debug.Log("Invictus Art already up to date.");
            return;
        }

        Debug.Log($"Invictus Art updated. Added: {added.Count}, Removed: {removed.Count}");

        foreach (var a in Limit(added, 25)) Debug.Log("  + " + a);
        foreach (var r in Limit(removed, 25)) Debug.Log("  - " + r);

        if (added.Count > 25 || removed.Count > 25)
            Debug.Log($"(Showing up to 25 of each. Total Added={added.Count}, Removed={removed.Count})");

        ShowNotification($"Art package updated ({Short(revAfter)})");
    }

    private static HashSet<string> SnapshotPackageFiles(string packageName)
    {
        try
        {
            string rev = GetLockedRevisionSafe(packageName);
            if (string.IsNullOrEmpty(rev)) return null;

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
                if (t > best) { best = t; pkgDir = dirs[i]; }
            }

            var set = new HashSet<string>();
            foreach (var file in Directory.GetFiles(pkgDir, "*", SearchOption.AllDirectories))
            {
                var rel = file.Substring(pkgDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
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

        // Try revision first
        string value = ExtractValue(json, pkgIndex, "revision");
        if (!string.IsNullOrEmpty(value))
            return value;

        // Fallback to hash (Git packages)
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
        for (int i = 0; i < n; i++) yield return list[i];
    }

    private static bool StringEquals(string a, string b) =>
        string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    private static string Short(string sha) =>
        string.IsNullOrEmpty(sha) ? "(none)" : (sha.Length <= 8 ? sha : sha.Substring(0, 8));
}