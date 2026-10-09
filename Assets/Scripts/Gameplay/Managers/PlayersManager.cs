using CommonDataTypes;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Spawners;
using System.Collections.Generic;
using UnityEditor;
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
        [SerializeField, Range(0.8f, 1.2f)] float _entityScaleMultiplier = 1f;

        readonly List<GameObject> _players = new();
        readonly Dictionary<GameObject, Vector2> _playersPositions = new();
        readonly Dictionary<string, PlayerActions> _activePlayerActions = new();
        List<InputControlScheme> _controlSchemes = new();
        List<AbilityActor> abilityActors = new();
        MatchSettings _matchSettings;

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
            _activePlayerActions.Clear();
            if (matchSettings.IsCampaignMatch)
            {
                switch (matchSettings.LevelData.TutorialMatch)
                {
                    case TutorialType.None:
                        SpawnOnePlayerCampaignMode(matchSettings);
                        break;
                    case TutorialType.BasicTutorial:
                        SpawnCampaign();
                        break;
                    case TutorialType.PassToturial:
                        break;
                    default:
                        break;
                }
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
        void SpawnOnePlayerCampaignMode(MatchSettings matchSettings)
        {
            CampaignLevelData levelData = matchSettings.LevelData;
            int layer = LayerMask.NameToLayer(EntityLayer.Player1_GoalKeeper.ToString());
            SpawnPlayer(levelData.Player1, PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[0], _controlSchemes[0], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player1_Player.ToString());
            SpawnPlayer(
                levelData.Player2,
                PlayersSpawner.PlayerType.Normal,
                _spawnPoints[1],
                _controlSchemes[1],
                layer
            );

            layer = LayerMask.NameToLayer(EntityLayer.Player2_Player.ToString());
            SpawnCpu(levelData.Opponent1, PlayersSpawner.PlayerType.Normal, _spawnPoints[2], layer);

            layer = LayerMask.NameToLayer(EntityLayer.Player2_GoalKeeper.ToString());
            SpawnCpu(levelData.Opponent2, PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3], layer);
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
            ApplyEntityScale(player);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
            ConfigurePlayerActions(player);
        }
        void SpawnPlayer(GameObject prefab, PlayersSpawner.PlayerType type,Transform position, InputControlScheme scheme, int layer)
        {
            GameObject player = _playersSpawner.SpawnPlayer(prefab ,type, position, scheme);
            ApplyEntityScale(player);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
            ConfigurePlayerActions(player);
            SetLayerAllChildren(player.transform, layer);
        }
        void SpawnPlayer(PlayersSpawner.PlayerType type, Transform position, InputControlScheme scheme, int layer)
        {
            GameObject player = _playersSpawner.SpawnPlayer(type, position, scheme);
            ApplyEntityScale(player);
            _players.Add(player);
            _playersPositions.Add(player, player.transform.position);
            ConfigurePlayerActions(player);
            SetLayerAllChildren(player.transform, layer);
        }

        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position);
            ApplyEntityScale(cpu);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
        }

        void SpawnCpu(PlayersSpawner.PlayerType type, Transform position, int layer)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(type, position);
            ApplyEntityScale(cpu);
            _players.Add(cpu);
            _playersPositions.Add(cpu, cpu.transform.position);
            SetLayerAllChildren(cpu.transform, layer);
        }
        void SpawnCpu(GameObject prefab, PlayersSpawner.PlayerType type, Transform position, int layer)
        {
            GameObject cpu = _playersSpawner.SpawnCpu(prefab, type, position);
            ApplyEntityScale(cpu);
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
                SpawnPlayer(
                    PlayersSpawner.PlayerType.Normal,
                    _spawnPoints[1],
                    _controlSchemes[1]
                );
                SpawnCpu(PlayersSpawner.PlayerType.Normal, _spawnPoints[2]);
                SpawnCpu(PlayersSpawner.PlayerType.Goalkeeper, _spawnPoints[3]);
            }
        }

        public void ResetMainPlayer()
        {
            _activePlayerActions.Clear();
            if (_players.Count > 0)
            {
                GameObject mainPlayer = _players[0];
                mainPlayer.GetComponent<PlayerActions>()?.ResetActionState();
                mainPlayer.GetComponent<IEntity>()?.Reset();
                mainPlayer.transform.SetPositionAndRotation(_playersPositions[mainPlayer], Quaternion.identity);
            }
        }

        public void ResetPlayers()
        {
            _activePlayerActions.Clear();
            foreach (GameObject player in _players)
            {
                player.GetComponent<PlayerActions>()?.ResetActionState();
                player.GetComponent<IEntity>()?.Reset();
                player.transform.SetPositionAndRotation(_playersPositions[player],  Quaternion.identity);
            }
        }
        public void DisablePlayers()
        {
            _activePlayerActions.Clear();
            foreach (GameObject player in _players)
            {
                player.GetComponent<PlayerActions>()?.ResetActionState();
                player.GetComponent<PlayerActions>().DisableInput = true;
            }
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

        public PlayerActions GetTeammate(PlayerActions requester)
        {
            if (requester == null)
                return null;

            foreach (GameObject player in _players)
            {
                if (player == null || !player.TryGetComponent(out PlayerActions candidate) ||
                    candidate == requester ||
                    candidate.AttackingDirection != requester.AttackingDirection)
                    continue;

                return candidate;
            }

            return null;
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

        void ConfigurePlayerActions(GameObject player)
        {
            PlayerActions actions = player.GetComponent<PlayerActions>();
            if (actions == null)
                return;

            actions.LockMovementToFacingDirection =
                _matchSettings != null &&
                _matchSettings.IsCampaignMatch &&
                _matchSettings.LevelData != null &&
                _matchSettings.LevelData.TutorialMatch == TutorialType.BasicTutorial;
        }

        void ApplyEntityScale(GameObject entity)
        {
            if (entity == null)
                return;

            float multiplier = _entityScaleMultiplier > 0f ? _entityScaleMultiplier : 1f;
            Vector3 scale = entity.transform.localScale;
            entity.transform.localScale = new Vector3(
                scale.x * multiplier,
                scale.y * multiplier,
                scale.z
            );
        }

        public bool TryBeginPlayerAction(PlayerActions requester, string controlScheme)
        {
            if (requester == null || string.IsNullOrEmpty(controlScheme))
                return true;

            if (_activePlayerActions.TryGetValue(controlScheme, out PlayerActions currentOwner) &&
                currentOwner != null && currentOwner.CanReceivePlayerAction)
                return currentOwner == requester;

            PlayerActions preferredPlayer = GetPreferredPlayerAction(controlScheme);
            if (preferredPlayer == null)
                preferredPlayer = requester;

            _activePlayerActions[controlScheme] = preferredPlayer;
            return preferredPlayer == requester;
        }

        public void EndPlayerAction(PlayerActions requester, string controlScheme)
        {
            if (requester == null || string.IsNullOrEmpty(controlScheme))
                return;

            if (_activePlayerActions.TryGetValue(controlScheme, out PlayerActions currentOwner) &&
                currentOwner == requester)
                _activePlayerActions.Remove(controlScheme);
        }

        public bool IsPreferredPlayerAction(PlayerActions candidate, string controlScheme)
        {
            if (candidate == null || !candidate.CanReceivePlayerAction)
                return false;
            if (string.IsNullOrEmpty(controlScheme))
                return true;

            if (_activePlayerActions.TryGetValue(controlScheme, out PlayerActions currentOwner) &&
                currentOwner != null)
                return currentOwner == candidate;

            PlayerActions preferredPlayer = GetPreferredPlayerAction(controlScheme);
            return preferredPlayer == null || preferredPlayer == candidate;
        }

        PlayerActions GetPreferredPlayerAction(string controlScheme)
        {
            Rigidbody2D ballRigidbody = BallManager.Instance?.Ball?.Rigidbody;
            PlayerActions preferredPlayer = null;
            float bestScore = float.PositiveInfinity;

            foreach (GameObject player in _players)
            {
                if (player == null || !player.TryGetComponent(out PlayerInput playerInput) ||
                    playerInput.currentControlScheme != controlScheme ||
                    !player.TryGetComponent(out PlayerActions actions) ||
                    !actions.CanReceivePlayerAction)
                    continue;

                float score = actions.GetActionSelectionScore(ballRigidbody);
                if (score >= bestScore)
                    continue;

                bestScore = score;
                preferredPlayer = actions;
            }

            return preferredPlayer;
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
