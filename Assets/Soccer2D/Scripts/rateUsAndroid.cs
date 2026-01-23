using UnityEngine;
using System.Collections;

public class rateUsAndroid : MonoBehaviour
{


    //public TweenAlpha rateUsWindow;

    bool popupFlag = false;
    void Start()
    {

        StartCoroutine(RatePopup());
    }

    public void Rate()
    {
        //rateUsWindow.PlayForward();
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

        if (firstRun == 0) 
        { 
            PlayerPrefs.SetInt("First", 1); 
        }
        else
        {

        }

        popupFlag = false;

        if (PlayerPrefs.GetInt("canRate") == 0 && PlayerPrefs.GetInt("acilisSayisi") < 5)
        {
            PlayerPrefs.SetInt("acilisSayisi", PlayerPrefs.GetInt("acilisSayisi") + 1);

        }

        if (PlayerPrefs.GetInt("canRate") == 0 && PlayerPrefs.GetInt("acilisSayisi") >= 5)
        {
            // Rate popup fonksiyon çağır panpa
            //GameObject.Find("RateUsMenu").GetComponent<TweenAlpha>().PlayForward();
            popupFlag = true;

        }
    }
}
