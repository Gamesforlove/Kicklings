using Gameplay.CharacterComponents;
using UnityEngine;

public class GrandpaStage0 : MonoBehaviour
{
    public static GrandpaStage0 Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }


    PlayerActions playerActions;
    public void DoScriptedKick()
    {
        if (playerActions == null)
            playerActions = GetComponent<PlayerActions>();
        playerActions?.ScriptedKick();
    }
}
