using System;
using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
    
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] Collider2D _collider;
    [SerializeField] SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
	void Start () {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _collider.enabled = false;
        StartCoroutine(LateEnable());
	}

    public void Reset()
    {
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _collider.enabled = false;
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.enabled =false;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.enabled =true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _collider.enabled = true;
    }
}
