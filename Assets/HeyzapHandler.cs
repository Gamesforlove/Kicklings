using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HeyzapHandler : MonoBehaviour {
    public float AddCooldown = 60;
    public bool isOnCoolDown;
    bool adfree;
    void Awake()
    {
      //  Debug.Log("yayaya");
        int af = PlayerPrefs.GetInt("adfree", 0);
        if (af == 1)
        {
            adfree = true;
        }

        else
        {
            adfree = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        GameHandler.GameController.OnGameEnd += GameController_OnGameEnd;
        GameHandler.GameController.OnGameStart += GameController_OnGameStart;
        GameHandler.GameController.OnNextRaund += GameController_OnNextRaund;
        GameHandler.GameController.OnFiveGoal += GameController_OnFiveGoal;
         HeyzapAds.start("676c289f7334b307b822a86d4dacb629", HeyzapAds.FLAG_NO_OPTIONS);
        // As early as possible, and after showing a video, call fetch
        //StartCoroutine(videoad());
    }


    void GameController_OnNextRaund()
    {
        if (GameHandler.GameController.isEndless)
        {
            if (GameHandler.GameController.totalGoal % 7 == 0 && GameHandler.GameController.totalGoal > 0)
            {
                RequestInterstitial();
            }

        }
    }

    void GameController_OnFiveGoal()
    {
        if (GameHandler.GameController.isEndless)
        {
            if (!adfree)
                ShowInterstitial();
        }
    }

    void GameController_OnGameEnd()
    {
        if (!adfree)
            ShowInterstitial();
    }

    void GameController_OnGameStart()
    {
        if (!adfree)
            RequestInterstitial();  
    }

    void ShowInterstitial()
    {

        if (HZVideoAd.isAvailable())
        {
            HZVideoAd.show("");
        }
        else { HZInterstitialAd.show(); } 
    }

    void RequestInterstitial()
    {
        HZVideoAd.fetch();
    }



    IEnumerator videoad()
    {
        yield return new WaitForSeconds(5);

        HZInterstitialAd.fetch();
    }
}
