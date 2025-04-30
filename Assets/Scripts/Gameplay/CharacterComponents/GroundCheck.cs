using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour
{
    public bool IsGrounded { get; private set; }
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
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