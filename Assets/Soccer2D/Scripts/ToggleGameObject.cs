using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObject : MonoBehaviour {

    public GameObject target;

	public void Toggle()
    {
        target.SetActive(!target.activeSelf);
    }

    public void ShowGameObject()
    {
        target.SetActive(true);
    }

    public void HideGameObject()
    {
        target.SetActive(false);
    }
}
