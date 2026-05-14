using UI.Customization.Countries;
using UnityEngine;

public class FreemodeIdenticalCountryBlocker : MonoBehaviour
{
    [SerializeField] private CountryCustomizationController _leftCountry;
    [SerializeField] private CountryCustomizationController _rightCountry;
    [SerializeField] private CountrySelectionListing _countrySelectionListing;

    private void OnEnable()
    {
        _countrySelectionListing.EnableAlButtons();
        if (_leftCountry.IsSelected)
        {
            _countrySelectionListing.DisableButton(_rightCountry.TeamDataIndex);
        }
        if (_rightCountry.IsSelected)
        {
            _countrySelectionListing.DisableButton(_leftCountry.TeamDataIndex);
        }
    }
}
