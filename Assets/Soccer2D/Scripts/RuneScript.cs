using UnityEngine;
using System.Collections;

public class RuneScript : MonoBehaviour {

    public enum RuneType
    {
        RedRune,
    }

    public RuneType runeType;

    public bool redTake;
    public bool blueTake;

	// Use this for initialization
	void Start () {
	
	}

    void OnEnable()
    {
        reset();
    }
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 10)
        {
            redTake = true;
            gameObject.SetActive(false);
            GameHandler.GameController.BlueGoalGetBig();
        }
        if (other.gameObject.layer == 9 || other.gameObject.layer == 11)
        {
            blueTake = true;
            gameObject.SetActive(false);
            GameHandler.GameController.RedGoalGetBig();
        }

    }

    void reset()
    {
        redTake = false;
        blueTake = false;
    }
}
