using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersSpawner : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab, _cpuPrefab;
    
    public GameObject SpawnCpu(Transform spawnPosition)
    {
        GameObject go = Instantiate(_cpuPrefab, spawnPosition.position, spawnPosition.rotation);
        go.transform.localScale = new Vector3(-1, 1, 1);
        HingeJoint2D joint = go.transform.Find("Visuals/Legs/RightLeg").gameObject.GetComponent<HingeJoint2D>();
        JointAngleLimits2D limits = new()
        {
            min = 90f,
        };
        joint.limits = limits;
        go.GetComponent<PlayerActions>().KickingLegSpeed *= -1;
        return go;
    }

    public GameObject SpawnPlayer(Transform spawnPosition, InputControlScheme scheme)
    {
        //GameObject go = Instantiate(_playerPrefab, spawnPosition.position, spawnPosition.rotation);
        GameObject go = PlayerInput.Instantiate(_playerPrefab, controlScheme: scheme.name, pairWithDevice: Keyboard.current).gameObject;
        go.transform.position = spawnPosition.position;
        return go;
    }
}
