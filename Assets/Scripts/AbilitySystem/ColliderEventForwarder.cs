using System;
using UnityEngine;

public class ColliderEventForwarder : MonoBehaviour
{
    public event Action<Collision2D> CollisionEntered;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEntered?.Invoke(collision);
    }
}