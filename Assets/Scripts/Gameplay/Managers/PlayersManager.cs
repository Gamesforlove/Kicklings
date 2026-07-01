using System.Collections.Generic;
using CommonDataTypes;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Spawners;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Managers
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField] PlayersSpawner _playersSpawner;
        [SerializeField] Transform[] _spawnPoints;

        public static PlayersManager Instance { get; private set; }
        void Awake() => Instance = this;

        readonly List<GameObject> _players = new();
        readonly Dictionary<GameObject, Vector2> _playersPositions = new();
        List<InputControlScheme> _controlSchemes = new();
        MatchSettings _matchSettings;
        private List<AbilityActor> abilityActors = new();


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
            int layer = LayerMask.NameToLayer(EntityLayer.Player1_GoalKeeper.ToString());
            SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player1_Player.ToString());
            SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_Player.ToString());
            SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[2], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_GoalKeeper.ToString());
            SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], layer);
        }

        void SpawnOnePlayerMode()
        {
            int layer = LayerMask.NameToLayer(EntityLayer.Player1_GoalKeeper.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player1_Player.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_Player.ToString());
            SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[2], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_GoalKeeper.ToString());
            SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], layer);
        }

        void SpawnTwoPlayersMode()
        {
            int layer = LayerMask.NameToLayer(EntityLayer.Player1_GoalKeeper.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player1_Player.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_Player.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[2], _controlSchemes[1], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_GoalKeeper.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], _controlSchemes[1], layer);
        }

        void SpawnFourPlayersMode()
        {
            int layer = LayerMask.NameToLayer(EntityLayer.Player1_GoalKeeper.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player1_Player.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[1], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_Player.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[2], _controlSchemes[2], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_GoalKeeper.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], _controlSchemes[3], layer);

            #if UNITY_EDITOR
                foreach (var player in _players)
                {
                    var abilityActor = player.GetComponent<AbilityActor>();
                    abilityActor.SetUpPlayersList(_players);
                    abilityActors.Add(abilityActor);
                }
            #endif

        }

        void SpawnPlayer(PlayersSpawner.PlayerType type,Transform position, InputControlScheme scheme)
        {
            GameObject player = _playersSpawner.SpawnPlayer(type, position, scheme);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
        }
        void SpawnPlayer(PlayersSpawner.PlayerType type, Transform position, InputControlScheme scheme, int layer)
        {
            GameObject player = _playersSpawner.SpawnPlayer(type, position, scheme);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
            SetLayerAllChildren(player.transform, layer);
        }

        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
        }
        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position, int layer)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
            SetLayerAllChildren(cpu.transform, layer);
        }

        public void SetDifficulty(DifficultyLevel difficulty)
        {
            _playersSpawner.SetDifficulty(difficulty);
        }

        public void ResetPlayers()
        {
            foreach (GameObject player in _players)
            {
                player.GetComponent<IEntity>().Reset();
                player.transform.SetPositionAndRotation(_playersPositions[player],  Quaternion.identity);
            }
        }
        public void DisablePlayers()
        {
            foreach (GameObject player in _players)
            {
                player.GetComponent<PlayerActions>().DisableInput = true;
            }
        }
        public IReadOnlyList<AbilityActor> GetAbilityActors()
        {
            return abilityActors.AsReadOnly();
        }
        void SetLayerAllChildren(Transform root, int layer)
        {
            var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = layer;
            }
        }
        public enum EntityLayer
        {
            Player1_Player,
            Player2_Player,
            Player1_GoalKeeper,
            Player2_GoalKeeper
        }
    }
}
