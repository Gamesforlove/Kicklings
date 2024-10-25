using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {
    public bool isLeft;
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ground")
        {
            if (isLeft)
                transform.parent.GetComponent<Jump>().leftOnGround = true;
            else
                transform.parent.GetComponent<Jump>().rightOnGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ground")
        {
            if (isLeft)
                transform.parent.GetComponent<Jump>().leftOnGround = false;
            else
                transform.parent.GetComponent<Jump>().rightOnGround = false;
        }
    }
}
