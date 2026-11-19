using System;
using UnityEngine;
using System.Collections;
using CommonDataTypes;
using Gameplay.CharacterComponents;

public class BallScript : MonoBehaviour {

    public static event Action TouchedPlayer;
    public event Action<PlayerActions, Collider2D> ContactedPlayer;
    public event Action<Collision2D> CollisionEntered;
    [field:SerializeField] public Rigidbody2D Rigidbody { get; private set; }
    [field:SerializeField] public Collider2D Collider { get; private set; }
    [field:SerializeField] public SpriteRenderer Renderer { get; private set; }

    [SerializeField] private LayerMask PlayersLayers;
    [SerializeField] private float _resetTorgueForce = 50f;
    [Header("Player rebounds")]
    [Tooltip("Maximum rise above a player contact, in world units under this ball's gravity. Zero disables the limit.")]
    [SerializeField, Min(0f)] private float _maxPlayerHitRise;
    [Tooltip("Lower rise limit for a rebound off the top of the head. Zero uses the general player hit limit.")]
    [SerializeField, Min(0f)] private float _maxHeaderRise;
    sbyte sideMultiplier = 0;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Renderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
/*	void Start () {
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Collider.enabled = false;
        StartCoroutine(LateEnable());
	}*/

    public void Reset()
    {
        Rigidbody.linearVelocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
        sideMultiplier = 0;
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Collider.enabled = false;
        StartCoroutine(LateEnable());
    }
    public void ResetWithSpin(FieldSideType fieldSideType)
    {
        Rigidbody.linearVelocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
        sideMultiplier = (sbyte)(fieldSideType == FieldSideType.Right ? -1 : 1);
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Collider.enabled = false;
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        Renderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        Renderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        Renderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        Renderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        Renderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        Renderer.enabled = true;
        Rigidbody.simulated = true;
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Rigidbody.AddTorque(_resetTorgueForce * sideMultiplier);
        Collider.enabled = true;
    }

    public void ResetWithoutBlink() // for special cases
    {
        Rigidbody.linearVelocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEntered?.Invoke(collision);
        HandlePlayerContact(collision, firstContact: true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // A player can jump under a ball that is already resting on their head.
        if (_maxPlayerHitRise > 0f && Rigidbody.linearVelocityY > 0f)
            HandlePlayerContact(collision, firstContact: false);
    }

    private void HandlePlayerContact(Collision2D collision, bool firstContact)
    {
        if (((1 << collision.gameObject.layer) & PlayersLayers) == 0)
            return;

        if (firstContact)
            TouchedPlayer?.Invoke();

        Collider2D bodyPart = collision.collider;
        PlayerActions player = bodyPart != null ? bodyPart.GetComponentInParent<PlayerActions>() : null;
        if (player == null && collision.otherCollider != null)
        {
            bodyPart = collision.otherCollider;
            player = bodyPart.GetComponentInParent<PlayerActions>();
        }
        if (player == null)
            return;

        if (firstContact)
            player.TryApplyBadKickRescue(Rigidbody, bodyPart);

        LimitPlayerHitRise(collision, player, bodyPart);
        if (firstContact)
            ContactedPlayer?.Invoke(player, bodyPart);
    }

    private void LimitPlayerHitRise(Collision2D collision, PlayerActions player, Collider2D bodyPart)
    {
        float speedY = Rigidbody.linearVelocityY;
        float gravity = -Physics2D.gravity.y * Rigidbody.gravityScale;
        if (_maxPlayerHitRise <= 0f || speedY <= 0f || gravity <= 0f || Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        // v² = 2gh. Measuring from the contact keeps the same lob height on fields at different elevations.
        float rise = speedY * speedY / (2f * gravity);
        float limit = _maxPlayerHitRise;
        if (_maxHeaderRise > 0f && _maxHeaderRise < limit && rise > _maxHeaderRise &&
            bodyPart == player.HeadContactCollider && IsBallAboveContact(collision))
            limit = _maxHeaderRise;

        if (rise > limit)
            Rigidbody.linearVelocityY = Mathf.Sqrt(2f * gravity * limit);
    }

    private bool IsBallAboveContact(Collision2D collision)
    {
        Vector2 center = Collider.bounds.center;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 toCenter = center - collision.GetContact(i).point;
            if (toCenter.y > Mathf.Abs(toCenter.x))
                return true;
        }
        return false;
    }
}
