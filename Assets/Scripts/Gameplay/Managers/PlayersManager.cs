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
            SpawnCpu(_spawnPoints[1]);
            SpawnCpu(_spawnPoints[2]);
        }

        void SpawnOnePlayerMode()
        {
            SpawnPlayer(_spawnPoints[1], _controlSchemes[0]);
            SpawnCpu(_spawnPoints[2]);
        }

        void SpawnTwoPlayersMode()
        {
            for (int i = 0; i < _matchSettings.MaxNumberOfEntities; i++)
            {
                if (i < _matchSettings.NumberOfPlayers) SpawnPlayer(_spawnPoints[i], _controlSchemes[i]);
            
                else SpawnCpu(_spawnPoints[i]);
            }
        }

        void SpawnFourPlayersMode()
        {
            for (int i = 0; i < _matchSettings.MaxNumberOfEntities; i++)
            { 
                SpawnPlayer(_spawnPoints[i], _controlSchemes[i]);
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
}
