using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;


public class AdmobHandler : MonoBehaviour
{
    private InterstitialAd interstitial;
    public float AddCooldown=120;
    public bool isOnCoolDown;
    bool adfree;
    void Awake()
    {
       // Debug.Log("yayaya");
        int af = PlayerPrefs.GetInt("adfree",0);
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
        GameHandler.GameController.OnGameEnd +=GameController_OnGameEnd;
        GameHandler.GameController.OnGameStart += GameController_OnGameStart;
        GameHandler.GameController.OnNextRaund += GameController_OnNextRaund;
        GameHandler.GameController.OnFiveGoal += GameController_OnFiveGoal;

        StartCoroutine(videoad());
    }

    void OnDisable()
    {

        GameHandler.GameController.OnGameEnd -= GameController_OnGameEnd;
        GameHandler.GameController.OnGameStart -= GameController_OnGameStart;
        GameHandler.GameController.OnNextRaund -= GameController_OnNextRaund;
        GameHandler.GameController.OnFiveGoal -= GameController_OnFiveGoal;
    }

    void GameController_OnNextRaund()
    {
        if (GameHandler.GameController.isEndless)
        {
            if (GameHandler.GameController.totalGoal % 7 == 0 && GameHandler.GameController.totalGoal> 0)
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

    // Update is called once per frame
    void Update()
    {

    }

    void GameController_OnGameEnd()
    {
        if(!adfree)
        ShowInterstitial();
    }

    void GameController_OnGameStart()
    {
        RequestInterstitial();
    }

    IEnumerator videoad()
    {
        yield return new WaitForSeconds(5);
        if (!adfree)
        RequestInterstitial();
        
    }

    IEnumerator AddCoolDown()
    {
        yield return new WaitForSeconds(AddCooldown);
        isOnCoolDown = false;

    }


    private void RequestInterstitial()
    {
#if UNITY_EDITOR
        string adUnitId = "ca-app-pub-7105645608079871/9927301740";
#elif UNITY_ANDROID
string adUnitId = "ca-app-pub-7105645608079871/9927301740";
#elif UNITY_IPHONE
string adUnitId = "ca-app-pub-7105645608079871/9927301740";
#else
string adUnitId = "ca-app-pub-7105645608079871/9927301740";
#endif
        // Create an interstitial.
        interstitial = new InterstitialAd(adUnitId);
        // Register for ad events.
        // Load an interstitial ad.
        interstitial.LoadAd(createAdRequest());
    }

    private AdRequest createAdRequest()
    {
        return new AdRequest.Builder()
        .AddTestDevice("93C50CAB222EEF08284780A51053B2FF") // Benim g2
        .Build();
        
        /* 
        .AddTestDevice("11DB3D498B8F8E64AF96109BE3FC52B0") // Halis ace
        .AddTestDevice("7B3568CFFB653F653E293EC941F71A73") // Halisin peder
        */
    }

    private void ShowInterstitial()
    {
        if (interstitial.IsLoaded() && !isOnCoolDown)
        {
            interstitial.Show();
            isOnCoolDown = true;
            StartCoroutine(AddCoolDown());
        }
        else
        {
          //  print("Interstitial is not ready yet.");
        }
    }
}
