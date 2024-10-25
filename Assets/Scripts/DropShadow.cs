using UnityEngine;
using System.Collections;

public class DropShadow : MonoBehaviour {

    public Vector3 zeropos = new Vector3(-0.1f, 0.2f, 0); 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.localPosition = zeropos;
        transform.position -= Vector3.up / 10;
	}
}
