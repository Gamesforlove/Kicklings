using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UI.UiSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Customization.Countries
{
    public class CountryCustomizationController : MonoBehaviour
    {
        [SerializeField] CountriesImages _countriesImages;
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] CountrySelectionListing _countrySelectionListing;
        [SerializeField] Image _leftFlagButtonImage,  _rightFlagButtonImage;

        Image _selectedFlagButtonImage;
        FieldSideType _selectedFieldSideType;

        public void SelectImage(string fieldSide)
        {
            switch (fieldSide)
            {
                case "Left":
                    _selectedFlagButtonImage = _leftFlagButtonImage;
                    _selectedFieldSideType = FieldSideType.Left;
                    break;
                case "Right":
                    _selectedFlagButtonImage = _rightFlagButtonImage;
                    _selectedFieldSideType = FieldSideType.Right;
                    break;
            }
        }

        void Start()
        {
            _leftFlagButtonImage.sprite = _countriesImages.GetCountrySprite(0);
            _rightFlagButtonImage.sprite = _countriesImages.GetCountrySprite(0);
        }

        void OnEnable()
        {
            EventBus<OnCountryChanged>.OnEvent += OnCountryChanged;
        }

        void OnDisable()
        {
            EventBus<OnCountryChanged>.OnEvent -= OnCountryChanged;
        }

        void OnCountryChanged(OnCountryChanged payload)
        {
            ChangeCountryImage(payload.CountryID);
            _uiViewsManager.HideView();
        }

        void ChangeCountryImage(int index)
        {
            _selectedFlagButtonImage.sprite = _countriesImages.GetCountrySprite(index);

            if (_selectedFieldSideType == FieldSideType.Left)
                MatchFlow.SetLeftCountryImage(index);
            else if (_selectedFieldSideType == FieldSideType.Right)
                MatchFlow.SetRightCountryImage(index);
        }
    }
}
