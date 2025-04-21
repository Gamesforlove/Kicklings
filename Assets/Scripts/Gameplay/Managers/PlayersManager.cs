using System;
using System.Collections.Generic;
using CommonDataTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] PlayersSpawner _playersSpawner;
    [SerializeField] Transform[] _fourEntitiesModePositions;
    
    readonly List<GameObject> _players = new();
    readonly Dictionary<GameObject, Vector2> _playersPositions = new();
    [SerializeField] List<InputControlScheme> _controlSchemes = new();

    void Start()
    {
        InputActionAsset actionAsset = InputSystem.actions;

        foreach (InputControlScheme scheme in actionAsset.controlSchemes)
        {
            _controlSchemes.Add(scheme);
        }
    }

    public void SpawnEntities(GameModeData selectedGameModeData)
    {
        switch (selectedGameModeData.NumberOfPlayers)
        {
            case 1:
                SpawnOnePlayerMode();
                break;
            case 2:
                SpawnTwoPlayersMode(selectedGameModeData);
                break;
            case 4:
                SpawnFourPlayersMode(selectedGameModeData);
                break;
        }
    }

    void SpawnOnePlayerMode()
    {
        SpawnPlayer(_fourEntitiesModePositions[0], _controlSchemes[0]);
        SpawnPlayer(_fourEntitiesModePositions[1], _controlSchemes[0]);
        SpawnCpu(_fourEntitiesModePositions[2]);
        SpawnCpu(_fourEntitiesModePositions[3]);
    }

    void SpawnTwoPlayersMode(GameModeData selectedGameModeData)
    {
        for (int i = 0; i < selectedGameModeData.TotalEntities; i++)
        {
            if (i < selectedGameModeData.NumberOfPlayers) SpawnPlayer(_fourEntitiesModePositions[i], _controlSchemes[i]);
            
            else SpawnCpu(_fourEntitiesModePositions[i]);
        }
    }

    void SpawnFourPlayersMode(GameModeData selectedGameModeData)
    {
        for (int i = 0; i < selectedGameModeData.TotalEntities; i++)
        { 
            SpawnPlayer(_fourEntitiesModePositions[i], _controlSchemes[i]);
        }
    }

    void SpawnPlayer(Transform position, InputControlScheme scheme)
    {
        GameObject player = _playersSpawner.SpawnPlayer(position, scheme);
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
