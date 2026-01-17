using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageFlow", menuName = "Scriptable Objects/StageFlow")]
public class StageFlow : ScriptableObject
{
    public List<SceneInfo> scenes;
}

[System.Serializable]
public class SceneInfo
{
    public string sceneName;
    public SceneType sceneType;
    public string Description;
}

[System.Serializable]
public enum SceneType
{
    Gameplay,
    Cutscene
}