using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlagDisplay : MonoBehaviour {

    public string prefsName;
    private UISprite sprite;

    private void Start()
    {
        sprite = GetComponent<UISprite>();
    }

    void Update () {
        sprite.spriteName = PlayerPrefs.GetString(prefsName, transform.parent.name == "Player1 Country Button" ? "USA" : "RUS");
    }
}
