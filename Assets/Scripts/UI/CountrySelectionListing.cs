using System.Collections.Generic;
using TMPro;
using UI.ButtonsBehaviours;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountrySelectionListing : MonoBehaviour
{
    [SerializeField] GameObject flagButtonPrefab;
    [SerializeField] List<Sprite> flagSprites = new List<Sprite>();

    void Start()
    {
        for (int i = 0; i < flagSprites.Count; i++) {
            GameObject countryFlag = Instantiate(flagButtonPrefab, transform);
            countryFlag.GetComponent<FlagButtonBehaviour>().SetUp(i, flagSprites[i]);
        }
    }
    
    
}
