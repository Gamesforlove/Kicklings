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
        
        [SerializeField] GameObject _fielderPrefab, _goalkeeperPrefab;
        [SerializeField] EntityData _fielderData, _goalkeeperData;
        
        public GameObject SpawnPlayer(PlayerType playerType, Transform spawnPosition, InputControlScheme scheme)
        {
            GameObject go = PlayerInput.Instantiate(
                playerType == PlayerType.Normal ? _fielderPrefab : _goalkeeperPrefab,
                controlScheme: scheme.name,
                pairWithDevice: Keyboard.current
                ).gameObject;
            
            go.transform.SetPositionAndRotation(spawnPosition.position, Quaternion.identity);
            
            go.AddComponent<Player>().SetUp(playerType == PlayerType.Normal ? _fielderData : _goalkeeperData);

            return go;
        }

        public GameObject SpawnCpu(PlayerType playerType, Transform spawnPosition)
        {
            GameObject go = Instantiate(
                playerType == PlayerType.Normal ? _fielderPrefab : _goalkeeperPrefab,
                spawnPosition.position, 
                Quaternion.identity
                );
 
            go.AddComponent<Cpu>().SetUp(playerType == PlayerType.Normal ? _fielderData : _goalkeeperData);

            return go;
        }
    }
}