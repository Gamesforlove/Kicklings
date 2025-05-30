using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.CharacterComponents.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Spawners
{
    public class PlayersSpawner : MonoBehaviour
    {
        public enum PlayerType { Normal, Goalkeeper }
        
        [SerializeField] GameObject _playerFielderPrefab, _playerGoalkeeperPrefab, _cpuFielderPrefab, _cpuGoalkeeperPrefab;
        [SerializeField] EntityData _fielderData, _goalkeeperData;
        [SerializeField] CpuDifficultyPreset _cpuDifficultyPreset;
        
        [SerializeField] DifficultyLevel _currentDifficulty = DifficultyLevel.Default;

        
        public GameObject SpawnPlayer(PlayerType playerType, Transform spawnPosition, InputControlScheme scheme)
        {
            GameObject go = PlayerInput.Instantiate(
                playerType == PlayerType.Normal ? _playerFielderPrefab : _playerGoalkeeperPrefab,
                controlScheme: scheme.name,
                pairWithDevice: Keyboard.current
                ).gameObject;
            
            go.transform.SetPositionAndRotation(spawnPosition.position, Quaternion.identity);
            
            go.GetComponent<Player>().SetUp(playerType == PlayerType.Normal ? _fielderData : _goalkeeperData);

            return go;
        }

        public GameObject SpawnCpu(PlayerType playerType, Transform spawnPosition)
        {
            GameObject go = Instantiate(
                playerType == PlayerType.Normal ? _cpuFielderPrefab : _cpuGoalkeeperPrefab,
                spawnPosition.position, 
                Quaternion.identity
                );
 
            CpuDifficultyPreset.DifficultySettings settings = _cpuDifficultyPreset.GetSettingsForDifficulty(_currentDifficulty);
            go.GetComponent<Cpu>().SetUp(new CpuConfiguration(
                playerType == PlayerType.Normal ? _fielderData : _goalkeeperData,
                settings
            ));
            
            return go;
        }
        
        public void SetDifficulty(DifficultyLevel newDifficulty)
        {
            _currentDifficulty = newDifficulty;
        }
    }
}