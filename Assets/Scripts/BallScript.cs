using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
    public bool goal;
    public bool redGoal;
    public bool blueGoal;

    public bool OOBBlue;
    public bool OOBRed;
	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(lateEnable());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void resetBall()
    {
        goal = false;
        redGoal = false;
        blueGoal = false;
        OOBBlue = false;
        OOBRed = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(lateEnable());
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!goal)
        {
            if (coll.gameObject.tag == "GoalRed")
            {
                goal = true;
                redGoal = true;
                GameHandler.GameController.Goal(GameController.Player.Red);
            }
        }

        if (coll.gameObject.tag == "GoalBlue")
        {
            if (!goal)
            {
                goal = true;
                blueGoal = true;
                GameHandler.GameController.Goal(GameController.Player.Blue);
            }
        }

        if (coll.gameObject.tag == "OOBBlue")
        {
            GameHandler.GameController.Out(GameController.Player.Blue);
            OOBBlue = true;
        }

        if (coll.gameObject.tag == "OOBRed")
        {
            GameHandler.GameController.Out(GameController.Player.Red);
            OOBRed = true;
        }

    }

    IEnumerator lateEnable()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<SpriteRenderer>().enabled =false;
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<SpriteRenderer>().enabled =true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Collider2D>().enabled = true;
    }
}
