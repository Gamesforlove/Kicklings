using System;
using CommonDataTypes;
using EventBusSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayersSpawner : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    
    public GameObject SpawnCpu(Transform spawnPosition)
    {
        GameObject go = Instantiate(_playerPrefab, spawnPosition.position, spawnPosition.rotation);
        go.transform.localScale = new Vector3(-1, 1, 1);
        go.AddComponent<Cpu>();
        HingeJoint2D joint = go.transform.Find("Visuals/Legs/RightLeg").gameObject.GetComponent<HingeJoint2D>();
        JointAngleLimits2D limits = new()
        {
            min = 90f,
        };
        joint.limits = limits;
        go.GetComponent<PlayerActions>().KickingLegSpeed *= -1;
        return go;
    }

    public GameObject SpawnPlayer(Transform spawnPosition)
    {
        GameObject go = Instantiate(_playerPrefab, spawnPosition.position, spawnPosition.rotation);
        go.AddComponent<Player>();
        return go;
    }
}
