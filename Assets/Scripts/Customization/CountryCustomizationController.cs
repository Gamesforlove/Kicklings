using CommonDataTypes;
using EventBusSystem;
using UI;
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
            _uiViewsManager.HideView();
        }
    }
}
