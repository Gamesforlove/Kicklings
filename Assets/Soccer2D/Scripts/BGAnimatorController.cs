using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAnimatorController : MonoBehaviour {

    public static BGAnimatorController Instance;

    private Animator animator;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start () 
    {
        animator = GetComponent<Animator>();
    }

    public void Goal()
    {
        animator.SetTrigger("Flash");
    }
}
