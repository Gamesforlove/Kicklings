using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using appnext;

public class AppnextHandler : MonoBehaviour {

	private int gamesPlayed = 0;

	Interstitial interstitial;
	RewardedVideo rewVideo;


	private bool rewardedVideo = false;

	bool adfree;
	void Awake()
	{
		// Debug.Log("yayaya");
		adfree = PlayerPrefs.GetInt("adfree",0)==1;
	}

	// Use this for initialization
	void Start()
	{
		GameHandler.GameController.OnGameEnd +=GameController_OnGameEnd;
		GameHandler.GameController.OnFiveGoal += GameController_OnFiveGoal;

		interstitial = new Interstitial ("38d4ca73-d53a-4cc7-a079-968fd33c1339");
		rewVideo = new RewardedVideo ("38d4ca73-d53a-4cc7-a079-968fd33c1339");

		rewVideo.onAdClosedDelegate += OnAdDone;

	}

	void OnDisable()
	{
		GameHandler.GameController.OnGameEnd -= GameController_OnGameEnd;
		GameHandler.GameController.OnFiveGoal -= GameController_OnFiveGoal;
		rewVideo.onAdClosedDelegate -= OnAdDone;
	}

	void GameController_OnFiveGoal()
	{
		if (GameHandler.GameController.isEndless)
		{
			gamesPlayed++;
			if (!adfree)
				ShowInterstitial();
		}
	}

	void GameController_OnGameEnd()
	{
		if(!adfree && !GameHandler.GameController.isOnMenu) {
			gamesPlayed++;
			ShowInterstitial();
		}
	}


	void OnAdDone(Ad ad){
		StartCoroutine(Timer());
	}


	private void ShowInterstitial()
	{
		if (gamesPlayed % 3 != 0)
			return;

		if (rewVideo.isAdLoaded ())
			rewVideo.showAd ();
		else if (interstitial.isAdLoaded ())
			interstitial.showAd ();
	}

	public void ShowRewardedVideo()
	{
		if (rewVideo.isAdLoaded())
		{
			rewardedVideo = true;
			rewVideo.showAd ();
		}
	}

	//Rewarded video functions


	IEnumerator Timer()
	{
		yield return new WaitForSeconds(0.5f);
		if (rewardedVideo)
		{
			rewardedVideo = false;
			PlayerPurchaseManager.Instance.AddCoins(50);
		}
	}
}
