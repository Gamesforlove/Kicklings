using UnityEngine;
using System.Collections;
using LitJson;

public class adsStatusParser : MonoBehaviour {

	
	void Start () 
    {
        WWW www = new WWW("http://nebugu.com/export.php?adsSetting&game=soccer_physics_2d");
        StartCoroutine(getAdsStatus(www));	

	}
	
	
	void Update () {
	
	}

    IEnumerator getAdsStatus(WWW status)
	{
		yield return status;

         GetComponent<AdmobHandler>().enabled = true;
                
        if (status.error == null)
        {
            if (status.text.Length < 20)
            {
                JsonData data = JsonMapper.ToObject(status.text);

               // Debug.Log(data[0].ToString()); // reklam id değeri

                if (data[0].ToString() == "1")
                {
                    GetComponent<AdmobHandler>().enabled = true;
                }

                else if (data[0].ToString() == "2")
                {
                    GetComponent<HeyzapHandler>().enabled = true;
                    GetComponent<AdmobHandler>().enabled = false;
                }

                else if (data[0].ToString() == "3")
                {
                    GetComponent<AddHandler>().enabled = true;
                    GetComponent<AdmobHandler>().enabled = false;
                }
                else
                {

                    GetComponent<AdmobHandler>().enabled = false;
                }
            }
            else
            {

                GetComponent<AdmobHandler>().enabled = false;
            }
        }
        else
        {

            GetComponent<AdmobHandler>().enabled = false;
        }
	}
}



			
		
			
			
