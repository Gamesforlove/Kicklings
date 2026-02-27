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

    private const string PackageUrl =
        "https://github.com/Gamesforlove/Kickling-Art.git?path=/KicklingsArt/Assets/Runtime-Assets#main";

    private static AddRequest _addRequest;
    private static bool _started;
    private static string _revisionBefore;

    static InvictusArtAutoUpdater()
    {
        if (!EditorPrefs.HasKey(PrefKey))
            EditorPrefs.SetBool(PrefKey, true);

        EditorApplication.delayCall += TryAutoUpdateOnce;
    }

    // Toggle
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

    // Manual update
    [MenuItem("Tools/Packages/Invictus Art/Update now")]
    private static void UpdateNow()
    {
        StartUpdate();
    }

    private static void TryAutoUpdateOnce()
    {
        if (_started) return;
        _started = true;

        if (!EditorPrefs.GetBool(PrefKey, true))
            return;

        if (Application.isBatchMode)
            return;

        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            EditorApplication.delayCall += TryAutoUpdateOnce;
            return;
        }

        _revisionBefore = GetLockedRevision();
        Debug.Log("Invictus Art: Checking for updates...");
        StartUpdate();
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

        if (_addRequest.Status == StatusCode.Success)
        {
            string after = GetLockedRevision();

            if (_revisionBefore != after)
            {
                ShowNotification(after);
                Debug.Log($"Invictus Art updated → {Short(_revisionBefore)} → {Short(after)}");
            }
            else
            {
                Debug.Log("Invictus Art already up to date.");
            }
        }
        else
        {
            Debug.LogError("Invictus Art update failed: " + _addRequest.Error.message);
        }

        _addRequest = null;
    }

    private static void ShowNotification(string revision)
    {
        EditorApplication.delayCall += () =>
        {
            var window = EditorWindow.focusedWindow;
            if (window != null)
                window.ShowNotification(new GUIContent($"Art package updated ({Short(revision)})"));

            // clicking console entry shows details
            Debug.Log($"Invictus Art revision: {revision}");
        };
    }

    private static string GetLockedRevision()
    {
        string lockPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "packages-lock.json");
        if (!File.Exists(lockPath)) return null;

        string json = File.ReadAllText(lockPath);
        string marker = $"\"{PackageName}\"";

        int start = json.IndexOf(marker);
        if (start == -1) return null;

        int revIndex = json.IndexOf("\"revision\"", start);
        if (revIndex == -1) return null;

        int quote = json.IndexOf('"', revIndex + 10);
        int end = json.IndexOf('"', quote + 1);

        return json.Substring(quote + 1, end - quote - 1);
    }

    private static string Short(string sha)
    {
        if (string.IsNullOrEmpty(sha)) return "(none)";
        return sha.Length <= 8 ? sha : sha.Substring(0, 8);
    }
}