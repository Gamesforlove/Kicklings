using Scene_Management;
using UnityEngine;

public class GroundLevelSetter : MonoBehaviour
{
    [SerializeField] private Transform _goal1;
    [SerializeField] private Transform _goal2;
    [SerializeField] private Transform _ground;
    [SerializeField] private Transform _ballSpawnPoint;

    public void SetGroundLevel()
    {
        float offset = MatchFlow.Match.Settings.LevelData.GroundLevelOffset;
        _goal1.position = new Vector3(_goal1.position.x, _goal1.position.y + offset, _goal1.position.z);
        _goal2.position = new Vector3(_goal2.position.x, _goal2.position.y + offset, _goal2.position.z);
        _ground.position = new Vector3(_ground.position.x, _ground.position.y + offset, _ground.position.z);
        _ballSpawnPoint.position = new Vector3(_ballSpawnPoint.position.x, _ballSpawnPoint.position.y + offset, _ballSpawnPoint.position.z);
    }
}
