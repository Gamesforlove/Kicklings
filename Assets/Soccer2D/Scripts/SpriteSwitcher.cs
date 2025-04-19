using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpriteSwitcher : MonoBehaviour {

    private float lastSwitcTime;
    public float switchTime;
    public Sprite[] sprites;
    private int spriteIndex = 0;
    private Sprite sprite;
    private Button button;

	// Use this for initialization
	void Start () {
        lastSwitcTime = Time.unscaledTime;
        sprite = GetComponent<Sprite>();
        button = GetComponent<Button>();
    }
	
	void Update () {
		if(Time.unscaledTime - lastSwitcTime >= switchTime)
        {
            spriteIndex++;
            if(spriteIndex >= sprites.Length)
                spriteIndex = 0;

            lastSwitcTime = Time.unscaledTime;
            sprite = sprites[spriteIndex];
            //button.image = sprites[spriteIndex];
        }
	}
}
