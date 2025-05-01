using CommonDataTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersSpawner : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab, _cpuPrefab;
    
    public GameObject SpawnCpu(Transform spawnPosition)
    {
        GameObject go = Instantiate(_cpuPrefab, spawnPosition.position, spawnPosition.rotation);
        ClothesSetter clothesSetter = go.GetComponent<ClothesSetter>();
        
        if (spawnPosition.position.x > 0)
        {
            ModifyComponentsForRightSide(go);
            clothesSetter.SetClothes(FieldSideType.Right);
        } else 
            clothesSetter.SetClothes(FieldSideType.Left);
        
        return go;
    }

    public GameObject SpawnPlayer(Transform spawnPosition, InputControlScheme scheme)
    {
        GameObject go = PlayerInput.Instantiate(_playerPrefab, controlScheme: scheme.name, pairWithDevice: Keyboard.current).gameObject;
        go.transform.position = spawnPosition.position;
        ClothesSetter clothesSetter = go.GetComponent<ClothesSetter>();
        
        if (spawnPosition.position.x > 0)
        {
            ModifyComponentsForRightSide(go);
            clothesSetter.SetClothes(FieldSideType.Right);
        } else 
            clothesSetter.SetClothes(FieldSideType.Left);
        
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
}