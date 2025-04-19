using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsDsiplayer : MonoBehaviour {


    private Text label;
	// Use this for initialization
	void Start () 
    {
        label = GetComponent<Text>();
        label.text = PlayerPurchaseManager.Instance.coinsAmount.ToString();

        PlayerPurchaseManager.Instance.onCoinsAmountChange += OnCoinsAmountChange;
    }

    private void OnDestroy()
    {
        PlayerPurchaseManager.Instance.onCoinsAmountChange -= OnCoinsAmountChange;
    }

    void OnCoinsAmountChange()
    {
        label.text = PlayerPurchaseManager.Instance.coinsAmount.ToString();
    }
}
