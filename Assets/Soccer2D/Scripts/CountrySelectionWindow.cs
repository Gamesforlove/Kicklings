using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountrySelectionWindow : MonoBehaviour {

    public const string COUNTRY_PLAYER = "currentCountryIOC";

    [SerializeField]
    private Transform buttonsContainer;

    [SerializeField]
    private ToggleGameObject player1Toggle;
    [SerializeField]
    private ToggleGameObject player2Toggle;

    private int currentPlayer = 1;

    private void Awake()
    {
        foreach (Transform child in buttonsContainer)
        {
            child.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => { chooseCountry(child.GetComponentInChildren<UILabel>().text); }));
        }
    }

    public void chooseCountry(string koko)
    {
        PlayerPrefs.SetString(COUNTRY_PLAYER + currentPlayer, koko);
        Debug.Log(koko);
        TweenAlpha.Begin(gameObject, 0.2f, 0f);
        player1Toggle.Toggle();
        player2Toggle.Toggle();
    }

    public void ShowWindow(string buttonName)
    {
        currentPlayer = 1;
        if (buttonName == "Player2 Country Button")
            currentPlayer = 2;

        TweenAlpha.Begin(gameObject, 0.2f, 1f);
    }
}
