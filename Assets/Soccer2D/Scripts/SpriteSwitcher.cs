using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwitcher : MonoBehaviour {

    private float lastSwitcTime;
    public float switchTime;
    public Sprite[] sprites;
    private int spriteIndex = 0;
    private UI2DSprite sprite;
    private UIButton button;

	// Use this for initialization
	void Start () {
        lastSwitcTime = Time.unscaledTime;
        sprite = GetComponent<UI2DSprite>();
        button = GetComponent<UIButton>();
    }
	
	void Update () {
		if(Time.unscaledTime - lastSwitcTime >= switchTime)
        {
            spriteIndex++;
            if(spriteIndex >= sprites.Length)
                spriteIndex = 0;

            lastSwitcTime = Time.unscaledTime;
            sprite.sprite2D = sprites[spriteIndex];
            button.normalSprite2D = sprites[spriteIndex];
        }
	}
}
