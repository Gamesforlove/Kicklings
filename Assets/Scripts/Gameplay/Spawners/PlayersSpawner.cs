using Gameplay.CharacterComponents;
using Scene_Management;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Spawners
{
    public class PlayersSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _playerPrefab, _cpuPrefab;
    
        public GameObject SpawnCpu(Transform spawnPosition)
        {
            GameObject go = Instantiate(_cpuPrefab, spawnPosition.position, spawnPosition.rotation);
            bool isRightSide = spawnPosition.position.x > 0;

            if (isRightSide)
                ModifyComponentsForRightSide(go);

            SetCharacterClothes(go, isRightSide);

            return go;
        }

        public GameObject SpawnPlayer(Transform spawnPosition, InputControlScheme scheme)
        {
            GameObject go = PlayerInput.Instantiate(_playerPrefab, controlScheme: scheme.name, pairWithDevice: Keyboard.current).gameObject;
            go.transform.position = spawnPosition.position;
            bool isRightSide = spawnPosition.position.x > 0;

            if (isRightSide)
                ModifyComponentsForRightSide(go);

            SetCharacterClothes(go, isRightSide);

            return go;
        }

        void ModifyComponentsForRightSide(GameObject go)
        {
            go.transform.localScale = new Vector3(-1, 1, 1);
            HingeJoint2D joint = go.transform.Find("Visuals/Legs/RightLeg").gameObject.GetComponent<HingeJoint2D>();
            JointAngleLimits2D limits = new()
            {
                min = 90f,
            };
            joint.limits = limits;
            go.GetComponent<PlayerActions>().KickingLegSpeed *= -1;
        }
        
        void SetCharacterClothes(GameObject go, bool isRightSide)
        {
            ClothesSetter clothesSetter = go.GetComponent<ClothesSetter>();
            if (clothesSetter == null) return;

            if (isRightSide)
            {
                clothesSetter.SetClothes(
                    MatchFlow.MatchSettings.RightSideShirtIndex,
                    MatchFlow.MatchSettings.RightSideShoesIndex
                );
            }
            else
            {
                clothesSetter.SetClothes(
                    MatchFlow.MatchSettings.LeftSideShirtIndex,
                    MatchFlow.MatchSettings.LeftSideShoesIndex
                );
            }
        }
    }
}