using UnityEngine;
using System.Collections;

public class rateUsAndroid : MonoBehaviour
{


    bool popupFlag = false;
    void Start()
    {

        StartCoroutine(RatePopup());
    }


    void Update()
    {

      

    }

    

    public void Rate()
    {
        string urlString = "market://details?id=" + "com.Nebugu.FootballPyhsics2D";
        Application.OpenURL(urlString);
        PlayerPrefs.SetInt("canRate", 1);
    
    
    }
    public void RemindLater()
    {
        PlayerPrefs.SetInt("acilisSayisi", 0);
    }

    public void NaverShowAgain()
    {
        PlayerPrefs.SetInt("canRate", 1);
    }

    IEnumerator RatePopup()
    {
        yield return new WaitForSeconds(1);

        int firstRun = PlayerPrefs.GetInt("First", 0);

        if (firstRun == 0) { GameObject.Find("EnableReplays").GetComponent<TweenAlpha>().PlayForward(); PlayerPrefs.SetInt("First", 1); }
        else
        {
            DestroyImmediate(GameObject.Find("EnableReplays"));
        }

        popupFlag = false;

        if (PlayerPrefs.GetInt("canRate") == 0 && PlayerPrefs.GetInt("acilisSayisi") < 5)
        {
            PlayerPrefs.SetInt("acilisSayisi", PlayerPrefs.GetInt("acilisSayisi") + 1);

        }

        if (PlayerPrefs.GetInt("canRate") == 0 && PlayerPrefs.GetInt("acilisSayisi") >= 5)
        {
            // Rate popup fonksiyon çağır panpa
            GameObject.Find("RateUsMenu").GetComponent<TweenAlpha>().PlayForward();
            popupFlag = true;

        }
    }
}
