using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlagDisplay : MonoBehaviour {

    public string prefsName;
    private Sprite sprite;

    private void Start()
    {
        sprite = GetComponent<Sprite>();
    }

    void Update () {
        sprite.name = PlayerPrefs.GetString(prefsName, transform.parent.name == "Player1 Country Button" ? "USA" : "RUS");
    }
}
