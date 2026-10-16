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
    Vector2? _tutorialKickArrival;

    public Vector2 GetTutorialFeedPosition(float playerX)
    {
        float direction = Mathf.Sign(playerX - transform.position.x);
        return (Vector2)transform.position + new Vector2(direction * 1.25f, 0.25f);
    }

    public void RecordTutorialKickArrival(Vector2 position) => _tutorialKickArrival = position;

    public bool TryTakeTutorialKickArrival(out Vector2 position)
    {
        bool hasArrival = _tutorialKickArrival.HasValue;
        position = _tutorialKickArrival.GetValueOrDefault();
        _tutorialKickArrival = null;
        return hasArrival;
    }

    public void DoScriptedKick() => DoScriptedKick(1f);

    public void DoScriptedKick(float jumpForceMultiplier)
    {
        if (playerActions == null)
            playerActions = GetComponent<PlayerActions>();
        playerActions?.ScriptedKick(jumpForceMultiplier);
    }
}
