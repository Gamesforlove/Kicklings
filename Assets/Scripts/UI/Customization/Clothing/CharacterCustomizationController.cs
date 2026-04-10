using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static CommonDataTypes.TeamsData;

namespace UI.Customization.Clothing
{
    public class CharacterCustomizationController : MonoBehaviour
    {
        public int ShirtIndex { get; private set; } 
        public int ShoesIndex { get; private set; }
        public int CountryIndex { get; private set; }
        
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] FieldSideType  _fieldSideType;
        [SerializeField] Image _shirtImage, _shirtPatternImage, _shoesLeftImage, _shoesRightImage, _leftSleeveImage, 
        _rightSleeveImage, _leftShortSockImage, _rightShortSockImage;

        void Start()
        {
/*            ChangeShirt(0);
            ChangeShoes(0);*/
        }
        private void OnEnable()
        {
            EventBus<OnCountryChanged>.OnEvent += OnCountyChanged;
        }
        private void OnDisable()
        {
            EventBus<OnCountryChanged>.OnEvent -= OnCountyChanged;
        }

        public void ChangeShirt(int nextIndex)
        {
            int newIndex = GetNextShirtIndex(nextIndex);
            _shirtPatternImage.sprite = _customizationImages.GetShirtSprite(newIndex);
            
            ShirtIndex = newIndex;
        }

        public void ChangeShoes(int nextIndex)
        {
            int newIndex = GetNextShoesIndex(nextIndex);
            _shoesLeftImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            _shoesRightImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            
            ShoesIndex = newIndex;
        }

        int GetNextShirtIndex(int delta)
        {
            int count = _customizationImages.GetShirtSpriteCount();
            return (ShirtIndex + delta + count) % count;
        }
        
        int GetNextShoesIndex(int delta)
        {
            int count = _customizationImages.GetShoesSpriteCount();
            return (ShoesIndex + delta + count) % count;
        }

        public void OnCountyChanged(OnCountryChanged evt)
        {
            if (!gameObject.activeSelf)
                return;
            if (evt.LastSelectedFieldSideType == FieldSideType.None)
            {
                _shirtImage.sprite = evt.TeamData.ShirtSprite;
                _leftSleeveImage.color = evt.TeamData.CountryColor;
                _rightSleeveImage.color = evt.TeamData.CountryColor;
                _leftShortSockImage.color = evt.TeamData.CountryColor;
                _rightShortSockImage.color = evt.TeamData.CountryColor;

                CountryIndex = evt.TeamData.Id;
            }
        }
    }
}
