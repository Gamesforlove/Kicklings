using System.Collections.Generic;
using CommonDataTypes;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] PlayersSpawner _playersSpawner;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Transform[] _twoEntitiesModePositions;
    [SerializeField] Transform[] _fourEntitiesModePositions;

    readonly List<GameObject> _players = new();
    readonly Dictionary<GameObject, Vector2> _playersPositions = new();
    
    public void SpawnEntities(GameModeData selectedGameModeData)
    {
        Transform[] spawnPoints = selectedGameModeData.TotalEntities == 2 ? _twoEntitiesModePositions : _fourEntitiesModePositions;
        
        for (int i = 0; i < selectedGameModeData.TotalEntities; i++)
        {
            if (i < selectedGameModeData.NumberOfPlayers) SpawnPlayer(spawnPoints[i]);
            
            else SpawnCpu(spawnPoints[i]);
        }
    }

    void SpawnPlayer(Transform position)
    {
        GameObject player = _playersSpawner.SpawnPlayer(position);
        _players.Add(player);
        _playersPositions.Add(player, player.transform.position);
    }

    void SpawnCpu(Transform position)
    {
        GameObject cpu = _playersSpawner.SpawnCpu(position);
        _players.Add(cpu);
        _playersPositions.Add(cpu, cpu.transform.position);
    }

    public void ResetPlayers()
    {
        foreach (GameObject player in _players)
        {
            player.transform.SetPositionAndRotation(_playersPositions[player],  Quaternion.identity);
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        }
    }

    
}
