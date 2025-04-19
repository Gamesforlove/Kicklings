using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountrySelectionListing : MonoBehaviour
{
    [SerializeField] private GameObject flagButtonPrefab;
    [SerializeField] private List<Sprite> flagSprites = new List<Sprite>();

    private void Start()
    {
        InitializeFlagButtons();
    }

    private void InitializeFlagButtons()
    {
        foreach (Sprite sprite in flagSprites) {
            GameObject countryFlag = Instantiate(flagButtonPrefab, transform);
            countryFlag.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            countryFlag.transform.GetChild(1).GetComponent<TMP_Text>().text = sprite.name;
        }
    }

}
