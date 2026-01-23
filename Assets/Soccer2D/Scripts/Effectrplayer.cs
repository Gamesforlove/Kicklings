using UnityEngine;
using System.Collections;

public class Effectrplayer : MonoBehaviour
{
    public static Effectrplayer instance;

    public AudioClip Jump;
    public AudioClip Button;
    public AudioClip Goal;
    public AudioClip RedWins;
    public AudioClip BlueWins;
    public AudioClip GoalExtend;
    public AudioClip RuneSpawn;
    public AudioClip crowdCheer;
    public AudioClip buyEffect;
    public AudioClip poppingBubbleSound;
    public AudioClip pressingPrizeMachine;
    // Use this for initialization
    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayJump()
    {
        GetComponent<AudioSource>().PlayOneShot(Jump);
    }
    public void PlayButton()
    {
        GetComponent<AudioSource>().PlayOneShot(Button);
    }

    public void PlayGoal()
    {
        GetComponent<AudioSource>().PlayOneShot(Goal);
        GetComponent<AudioSource>().PlayOneShot(crowdCheer);
    }
    public void PlayRedWins()
    {
        GetComponent<AudioSource>().PlayOneShot(RedWins);
    }

    public void PlayBlueWins()
    {
        GetComponent<AudioSource>().PlayOneShot(BlueWins);
    }

    public void PlayRunespawn()
    {
        GetComponent<AudioSource>().PlayOneShot(RuneSpawn);
    }

    public void PlayBlueGoalExtend()
    {
        GetComponent<AudioSource>().PlayOneShot(GoalExtend);
    }

    public void Toggle(bool state)
    {
        GetComponent<AudioSource>().enabled = state;
    }

    public void PlayBuyEffect()
    {
        GetComponent<AudioSource>().PlayOneShot(buyEffect);
    }

    public void PoppingbubbleEffect()
    {
        GetComponent<AudioSource>().PlayOneShot(poppingBubbleSound);
    }

    public void PlayPrizeMachineSound()
    {
        GetComponent<AudioSource>().PlayOneShot(pressingPrizeMachine, 2);
    }
}
