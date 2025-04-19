using System.Collections;
using UnityEngine;

public class Cpu : MonoBehaviour
{
    [SerializeField] float _timeToPerformAction = 2f;
    
    PlayerActions _playerActions;

    void Awake()
    {
        _playerActions = GetComponent<PlayerActions>();
    }

    void Start()
    {
        StartCoroutine(DoAction(_timeToPerformAction));
    }
    
    IEnumerator DoAction(float time)
    {
        _playerActions.OnActionPerformed();
        yield return new WaitForSeconds(0.2f);
        _playerActions.OnActionCancelled();
        yield return new WaitForSeconds(time);
        StartCoroutine(DoAction(time));
    }
}
