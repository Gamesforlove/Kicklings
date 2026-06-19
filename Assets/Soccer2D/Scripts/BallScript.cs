using System;
using UnityEngine;
using System.Collections;
using CommonDataTypes;

public class BallScript : MonoBehaviour {

    public static event Action TouchedPlayer;
    [field:SerializeField] public Rigidbody2D Rigidbody { get; private set; }
    [field:SerializeField] public Collider2D Collider { get; private set; }
    [field:SerializeField] public SpriteRenderer Renderer { get; private set; }

    [SerializeField] private LayerMask PlayersLayers;
    [SerializeField] private float _resetTorgueForce = 50f;
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
        Renderer.enabled =false;
        yield return new WaitForSeconds(0.2f);
        Renderer.enabled =true;
        Rigidbody.simulated = true;
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Rigidbody.AddTorque(_resetTorgueForce * sideMultiplier);
        Collider.enabled = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & PlayersLayers) != 0)
        {
            TouchedPlayer?.Invoke();
        }
    }
}
