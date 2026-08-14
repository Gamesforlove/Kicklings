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
        [SerializeField] bool oneOnOneForCampaign;
        [SerializeField] bool justPlayerForCampaign;

        readonly List<GameObject> _players = new();
        readonly Dictionary<GameObject, Vector2> _playersPositions = new();
        List<InputControlScheme> _controlSchemes = new();
        List<AbilityActor> abilityActors = new();
        MatchSettings _matchSettings;
        bool campaign;

        public static PlayersManager Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            InputActionAsset actionAsset = InputSystem.actions;

            foreach (InputControlScheme scheme in actionAsset.controlSchemes)
            {
                _controlSchemes.Add(scheme);
            }
        }

        public void SpawnEntities(MatchSettings matchSettings)
        {
            _matchSettings  = matchSettings;

            if (matchSettings.IsCampaignMatch)
            {
                campaign = true;
                SpawnCampaign();
                return;
            }

            switch (matchSettings.NumberOfPlayers)
            {
                case 0:
                    SpawnCpuMode();
                    break;
                case 1:
                    SpawnOnePlayerMode(matchSettings.SplitControls);
                    break;
                case 2:
                    SpawnTwoPlayersMode();
                    break;
                case 4:
                    SpawnFourPlayersMode();
                    break;
            }
        }


        public void SpawnSinglePlayer()
        {
            GameObject player = _playersSpawner.SpawnChallengePlayer(_spawnPoints[0], _controlSchemes[0]);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
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

        void SpawnOnePlayerMode(bool twoControls = false)
        {
            int layer = LayerMask.NameToLayer(EntityLayer.Player1_GoalKeeper.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player1_Player.ToString());
            SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], twoControls ? _controlSchemes[1] : _controlSchemes[0], layer);

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
            GameObject player = _playersSpawner.SpawnPlayer(type, position, scheme, campaign);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
        }
        void SpawnPlayer(PlayersSpawner.PlayerType type, Transform position, InputControlScheme scheme, int layer)
        {
            GameObject player = _playersSpawner.SpawnPlayer(type, position, scheme, campaign);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
            SetLayerAllChildren(player.transform, layer);
        }

        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position, campaign);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
        }

        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position, int layer)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position, campaign);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
            SetLayerAllChildren(cpu.transform, layer);
        }

        public void SetDifficulty(DifficultyLevel difficulty)
        {
            _playersSpawner.SetDifficulty(difficulty);
        }


        void SpawnCampaign()
        {
            if (justPlayerForCampaign)
            {
                SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0]);
            }
            else if (oneOnOneForCampaign)
            {
                SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0]);
                SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[1]);
            }
            else
            {
                SpawnPlayer(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0]);
                SpawnPlayer(PlayersSpawner.PlayerType.Normal, _spawnPoints[1], _controlSchemes[0]);
                SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[2]);
                SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3]);
            }
        }

        public void ResetMainPlayer()
        {
            if (_players.Count > 0)
            {
                GameObject mainPlayer = _players[0];
                mainPlayer.GetComponent<IEntity>()?.Reset();
                mainPlayer.transform.SetPositionAndRotation(_playersPositions[mainPlayer], Quaternion.identity);
            }
        }

        public void ResetPlayers()
        {
            foreach (GameObject player in _players)
            {
                player.GetComponent<IEntity>()?.Reset();
                player.transform.SetPositionAndRotation(_playersPositions[player],  Quaternion.identity);
            }
        }
        public void DisablePlayers()
        {
            foreach (GameObject player in _players)
                player.GetComponent<PlayerActions>().DisableInput = true;
        }

        public List<GameObject> Players { get => _players; }

        public List<PlayerActions> GetPlayerActions()
        {
            List<PlayerActions> allPlayerActions = new List<PlayerActions>();
            foreach (var player in _players)
            {
                var actions = player.GetComponent<PlayerActions>();
                if (actions != null) allPlayerActions.Add(actions);
            }
            return allPlayerActions;
        }

        public void EnablePlayers()
        {
            foreach (GameObject player in _players)
                player.GetComponent<PlayerActions>().DisableInput = false;
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