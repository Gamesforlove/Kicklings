using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private Scene scene;
    public void LoadScene() => SceneHandler.Instance.LoadScene(scene);
}
