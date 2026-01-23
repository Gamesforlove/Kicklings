using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDisable : MonoBehaviour
{
    [SerializeField] private float delayTime = 7f;

    private void Start()
    {
        GameController.Instance.OnGameStart += EnableObject;
    }

    private void OnDestroy()
    {
        GameController.Instance.OnGameStart -= EnableObject;
    }

    private void EnableObject()
    {
        gameObject.SetActive(true);
        Invoke(nameof(DisableObject), delayTime);
    }

    private void DisableObject() => gameObject.SetActive(false);

}
