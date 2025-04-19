using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreColisionTailHelper : MonoBehaviour
{

    public LayerMask layersToIgnore;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (layersToIgnore == (layersToIgnore | (1 << collision.gameObject.layer)) && collision.gameObject.layer != gameObject.layer)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
