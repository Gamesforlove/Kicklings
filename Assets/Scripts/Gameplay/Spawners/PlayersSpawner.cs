using Gameplay.CharacterComponents.Cpu;
using Gameplay.CharacterComponents.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Spawners
{
    public class PlayersSpawner : MonoBehaviour
    {
        public enum PlayerType { Normal, Goalkeeper }
        
        [SerializeField] GameObject _playerPrefab, _goalkeeperPrefab;
        [SerializeField] InputActionAsset _inputActions;
        
        public GameObject SpawnPlayer(PlayerType playerType, Transform spawnPosition, InputControlScheme scheme)
        {
            GameObject go = PlayerInput.Instantiate(
                DecidePrefab(playerType),
                controlScheme: scheme.name,
                pairWithDevice: Keyboard.current
                ).gameObject;
            
            go.transform.SetPositionAndRotation(spawnPosition.position, Quaternion.identity);
            
            go.AddComponent<Player>().SetUp();

            return go;
        }

        public GameObject SpawnCpu(PlayerType playerType, Transform spawnPosition)
        {
            GameObject go = Instantiate(DecidePrefab(playerType), spawnPosition.position, Quaternion.identity);
 
            go.AddComponent<Cpu>().SetUp();

            return go;
        }

        GameObject DecidePrefab(PlayerType playerType)
        {
            return playerType switch
            {
                PlayerType.Normal => _playerPrefab,
                PlayerType.Goalkeeper => _goalkeeperPrefab
            };
        }
    }
}