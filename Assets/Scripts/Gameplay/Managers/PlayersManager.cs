using System.Collections.Generic;
using CommonDataTypes;
using Gameplay.Spawners;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Managers
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField] PlayersSpawner _playersSpawner;
        [SerializeField] Transform[] _spawnPoints;
    
        readonly List<GameObject> _players = new();
        readonly Dictionary<GameObject, Vector2> _playersPositions = new();
        List<InputControlScheme> _controlSchemes = new();
        MatchSettings _matchSettings;

        void Start()
        {
            InputActionAsset actionAsset = InputSystem.actions;

            foreach (InputControlScheme scheme in actionAsset.controlSchemes)
            {
                _controlSchemes.Add(scheme);
            }
        }

        public void SpawnEntities(MatchSettings matchSettings)
        {
            _matchSettings  = matchSettings;
            switch (matchSettings.NumberOfPlayers)
            {
                case 0:
                    SpawnCpuMode();
                    break;
                case 1:
                    SpawnOnePlayerMode();
                    break;
                case 2:
                    SpawnTwoPlayersMode();
                    break;
                case 4:
                    SpawnFourPlayersMode();
                    break;
            }
        }

        void SpawnCpuMode()
        {
            SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0]);
            SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[1]);
            SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[2]);
            SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3]);
        }

        void SpawnOnePlayerMode()
        {
			SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0]);
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[0]);
            SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[2]);
            SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3]);
        }

        void SpawnTwoPlayersMode()
        {
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0]);
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[0]);
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[2], _controlSchemes[1]);
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], _controlSchemes[1]);
        }

        void SpawnFourPlayersMode()
        {
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0]);
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[1]);
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[2], _controlSchemes[2]);
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], _controlSchemes[3]);
        }

        void SpawnPlayer(PlayersSpawner.PlayerType type,Transform position, InputControlScheme scheme)
        {
            GameObject player = _playersSpawner.SpawnPlayer(type, position, scheme);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
        }

        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
        }

        public void ResetPlayers()
        {
            foreach (GameObject player in _players)
            {
                player.transform.SetPositionAndRotation(_playersPositions[player],  Quaternion.identity);
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
                player.GetComponent<Rigidbody2D>().angularVelocity = 0;
                player.transform.eulerAngles = Vector3.zero;
            }
        }

    
    }
}
