using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer instance;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private AudioSource audioSource;

    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    void Start () {
        GameHandler.GameController.OnGameEnd += GameController_OnGameEnd;
        GameHandler.GameController.OnGameStart += GameController_OnGameStart;

        audioSource = GetComponent<AudioSource>();
    }

    void GameController_OnGameStart()
    {
        audioSource.clip = gameMusic;
        audioSource.Play();
        audioSource.volume = 0.1f;
    }

    void GameController_OnGameEnd()
    {
        if (GameHandler.GameController.isOnMenu)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
            audioSource.volume = 0.6f;
        }

    }


    public void Toggle(bool state)
    {
        GetComponent<AudioSource>().enabled = state;
    }
}
