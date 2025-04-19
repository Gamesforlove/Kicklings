using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsDsiplayer : MonoBehaviour {


    private UILabel label;
	// Use this for initialization
	void Start () 
    {
        label = GetComponent<UILabel>();
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
