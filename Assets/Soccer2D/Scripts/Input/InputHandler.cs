using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [SerializeField] private KeyCode player1Jump1; // Z
    [SerializeField] private KeyCode player1Jump2; // X

    [Space(10)]
    [SerializeField] private KeyCode player2Jump1; // M
    [SerializeField] private KeyCode player2Jump2; // N

    private GameController controller;

    public static Action OnPlayer1Jump1Press;
    public static Action OnPlayer1Jump1Release;
    public static Action OnPlayer1Jump2Press;
    public static Action OnPlayer1Jump2Release;

    public static Action OnPlayer2Jump1Press;
    public static Action OnPlayer2Jump1Release;
    public static Action OnPlayer2Jump2Press;
    public static Action OnPlayer2Jump2Release;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        controller = GameController.Instance;
    }

    private void Update()
    {
        if (!controller.twoButtonControl) {
            if (Input.GetKeyDown(player1Jump1) || Input.GetKeyDown(player1Jump2))
                OnPlayer1Jump1Press?.Invoke();

            if (Input.GetKeyUp(player1Jump1) || Input.GetKeyUp(player1Jump2))
                OnPlayer1Jump1Release?.Invoke();

            if (Input.GetKeyDown(player2Jump1) || Input.GetKeyDown(player2Jump2))
                OnPlayer2Jump1Press?.Invoke();

            if (Input.GetKeyUp(player2Jump1) || Input.GetKeyUp(player2Jump2))
                OnPlayer2Jump1Release?.Invoke();
        }else {
            if (Input.GetKeyDown(player1Jump1)) OnPlayer1Jump1Press?.Invoke();
            if (Input.GetKeyUp(player1Jump1)) OnPlayer1Jump1Release?.Invoke();

            if (Input.GetKeyDown(player1Jump2)) OnPlayer1Jump2Press?.Invoke();
            if (Input.GetKeyUp(player1Jump2)) OnPlayer1Jump2Release?.Invoke();

            if (Input.GetKeyDown(player2Jump1)) OnPlayer2Jump1Press?.Invoke();
            if (Input.GetKeyUp(player2Jump1)) OnPlayer2Jump1Release?.Invoke();

            if (Input.GetKeyDown(player2Jump2)) OnPlayer2Jump2Press?.Invoke();
            if (Input.GetKeyUp(player2Jump2)) OnPlayer2Jump2Release?.Invoke();
        }
        
    }
}
