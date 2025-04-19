using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour
{
    public bool isLeft;
    public bool isTop;
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ground")
        {
            if (isTop)
                transform.parent.GetComponent<Jump>().topOnGround = true;
            else if (isLeft)
                transform.parent.GetComponent<Jump>().leftOnGround = true;
            else
                transform.parent.GetComponent<Jump>().rightOnGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ground")
        {
            if (isTop)
                transform.parent.GetComponent<Jump>().topOnGround = false;
            else if (isLeft)
                transform.parent.GetComponent<Jump>().leftOnGround = false;
            else
                transform.parent.GetComponent<Jump>().rightOnGround = false;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Player Out")
        {
            transform.parent.GetComponent<Jump>().canMove = false;
            GameHandler.GameController.PlayerOut(transform.parent.gameObject);
        }
    }

}