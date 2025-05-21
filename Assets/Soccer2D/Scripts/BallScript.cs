using System;
using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
    
    [field:SerializeField] public Rigidbody2D Rigidbody { get; private set; }
    [field:SerializeField] public Collider2D Collider { get; private set; }
    [field:SerializeField] public SpriteRenderer Renderer { get; private set; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Renderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
	void Start () {
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Collider.enabled = false;
        StartCoroutine(LateEnable());
	}

    public void Reset()
    {
        Rigidbody.linearVelocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
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
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Collider.enabled = true;
    }
}
