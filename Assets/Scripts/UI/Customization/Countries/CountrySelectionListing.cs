using System.Collections.Generic;
using UI.ButtonsBehaviours;
using UnityEngine;

public class CountrySelectionListing : MonoBehaviour
{
    [SerializeField] GameObject _flagButtonPrefab;
    [SerializeField] List<Sprite> _flagSprites = new();

    void Start()
    {
        for (int i = 0; i < _flagSprites.Count; i++) {
            GameObject countryFlag = Instantiate(_flagButtonPrefab, transform);
            countryFlag.GetComponent<FlagButtonBehaviour>().SetUp(i, _flagSprites[i]);
        }
    }
    
    
}
