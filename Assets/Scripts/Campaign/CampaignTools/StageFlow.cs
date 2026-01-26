using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Linq;
#endif

[CreateAssetMenu(fileName = "StageFlow", menuName = "Scriptable Objects/StageFlow")]
public class StageFlow : ScriptableObject
{
    public List<SceneInfo> scenes = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (scenes == null) return;

        foreach (SceneInfo info in scenes)
        {
            if (info == null || info.sceneAsset == null) continue;

            string path = AssetDatabase.GetAssetPath(info.sceneAsset);
            info.SetPathEditorOnly(path);
            bool inBuild = EditorBuildSettings.scenes
                .Any(s => s != null && s.enabled && s.path == path);

            if (!inBuild)
                Debug.LogWarning($"Scene '{info.sceneAsset.name}' is not ENABLED in Build Settings. " +
                    $"Use the button below to add it.", this);
        }
    }
#endif
}

[System.Serializable]
public class SceneInfo
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
    public void SetPathEditorOnly(string path) => scenePath = path;
#endif

    public SceneType sceneType;
    [TextArea] public string description;

    [SerializeField] private string scenePath;
    public string Path => scenePath;
}

public enum SceneType
{
    Gameplay,
    Cutscene
}
