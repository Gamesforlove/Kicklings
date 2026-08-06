using System;
using UnityEngine;

public class ColliderEventForwarder : MonoBehaviour
{
    public event Action<Collision2D, BodyPartCollider> CollisionEntered;
    public Collider2D collider { get; private set; }

    [SerializeField] private BodyPartCollider bodyPart;
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEntered?.Invoke(collision, bodyPart);
    }

    public enum BodyPartCollider
    {
        Head,
        Body,
        KickingLeg,
        Leg,
        Foot
    }
}