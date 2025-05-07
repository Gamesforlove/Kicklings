using UI.ButtonsBehaviours;
using UnityEngine;

namespace UI.Customization.Countries
{
    public class CountrySelectionListing : MonoBehaviour
    {
        [SerializeField] CountriesImages _countriesImages;
        [SerializeField] GameObject _flagButtonPrefab;

        void Start()
        {
            for (int i = 0; i < _countriesImages.GetListCount(); i++) {
                GameObject countryFlag = Instantiate(_flagButtonPrefab, transform);
                countryFlag.GetComponent<FlagButtonBehaviour>().SetUp(i, _countriesImages.GetCountrySprite(i));
            }
        }
    
    
    }
}
