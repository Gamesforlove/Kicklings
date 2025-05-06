using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UI;
using UI.UiSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Customization
{
    public class CountryCustomizationController : MonoBehaviour
    {
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

        void OnEnable()
        {
            EventBus<OnCountryChanged>.OnEvent += OnCountryChanged;
        }

        void OnDisable()
        {
            EventBus<OnCountryChanged>.OnEvent -= OnCountryChanged;
        }

        void OnCountryChanged(OnCountryChanged evt)
        {
            _selectedFlagButtonImage.sprite = evt.CountryImage.sprite;
            
            if (_selectedFieldSideType == FieldSideType.Left)
                MatchFlow.SetLeftCountryImage(evt.CountryImage);
            else if (_selectedFieldSideType == FieldSideType.Right)
                MatchFlow.SetRightCountryImage(evt.CountryImage);
            
            _uiViewsManager.HideView();
        }
    }
}
