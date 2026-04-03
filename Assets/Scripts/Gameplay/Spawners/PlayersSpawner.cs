using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.CharacterComponents.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Spawners
{
    public class PlayersSpawner : MonoBehaviour
    {
        public static PlayersSpawner Instance { get; private set; }
        void Awake() => Instance = this;

        public enum PlayerType { Normal, Goalkeeper }
        
        [SerializeField] GameObject _playerFielderPrefab, _playerGoalkeeperPrefab, _cpuFielderPrefab, _cpuGoalkeeperPrefab;
        [field: SerializeField] public EntityData FielderData { get; private set; }
        [field: SerializeField] public EntityData GoalkeeperData { get; private set; }
        [field: SerializeField] public CpuDifficultyPreset CpuDifficultyPreset { get; private set; }
        [field: SerializeField] public DifficultyLevel CurrentDifficulty { get; private set; } = DifficultyLevel.Default;

        
        public GameObject SpawnPlayer(PlayerType playerType, Transform spawnPosition, InputControlScheme scheme, bool campaign)
        {
            GameObject go = PlayerInput.Instantiate(
                playerType == PlayerType.Normal ? _playerFielderPrefab : _playerGoalkeeperPrefab,
                controlScheme: scheme.name,
                pairWithDevice: Keyboard.current
                ).gameObject;
            
            go.transform.SetPositionAndRotation(spawnPosition.position, Quaternion.identity);
            
            go.GetComponent<Player>()?.SetUp(playerType == PlayerType.Normal ? FielderData : GoalkeeperData, campaign);

            return go;
        }

        public GameObject SpawnCpu(PlayerType playerType, Transform spawnPosition, bool campaign)
        {
            GameObject go = Instantiate(
                playerType == PlayerType.Normal ? _cpuFielderPrefab : _cpuGoalkeeperPrefab,
                spawnPosition.position, 
                Quaternion.identity
                );
 
            CpuDifficultyPreset.DifficultySettings settings = CpuDifficultyPreset.GetSettingsForDifficulty(CurrentDifficulty);
            go.GetComponent<Cpu>()?.SetUp(new CpuConfiguration(
                playerType == PlayerType.Normal ? FielderData : GoalkeeperData,
                settings
            ), campaign);
            
            return go;
        }

        public void SetDifficulty(DifficultyLevel newDifficulty)
        {
            CurrentDifficulty = newDifficulty;
        }
    }
}