using UnityEngine;
using System.Collections;

public class Effectrplayer : MonoBehaviour {
   public AudioClip Jump;
   public AudioClip Button;
   public AudioClip Goal;
   public AudioClip RedWins;
   public AudioClip BlueWins;
   public AudioClip GoalExtend;
   public AudioClip RuneSpawn;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
