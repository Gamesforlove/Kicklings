using CommonDataTypes;
using Scene_Management;
using UI.Customization.Clothing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.FreeMode
{
    public class CharacterCustomizationController : MonoBehaviour
    {
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] FieldSideType  _fieldSideType;
        [SerializeField] Image _shirtImage,  _shoesLeftImage, _shoesRightImage;
        int _shirtIndex, _shoesIndex;

        void Start()
        {
            _shirtImage.sprite = _customizationImages.GetShirtSprite(_shirtIndex);
            _shoesLeftImage.sprite = _customizationImages.GetShoesSprite(_shoesIndex);
            _shoesRightImage.sprite = _customizationImages.GetShoesSprite(_shoesIndex);

            if (_fieldSideType == FieldSideType.Left)
            {
                MatchFlow.SetLeftSideShoesIndex(_shirtIndex);
                MatchFlow.SetLeftSideShoesIndex(_shoesIndex);
            }
            else
            {
                MatchFlow.SetRightSideShoesIndex(_shirtIndex);
                MatchFlow.SetRightSideShoesIndex(_shoesIndex);
            }
                
        }
        
        public void ChangeShirt(int nextIndex)
        {
            int newIndex = GetNextShirtIndex(nextIndex);
            _shirtImage.sprite = _customizationImages.GetShirtSprite(newIndex);
            
            if (_fieldSideType == FieldSideType.Left)
                MatchFlow.SetLeftSideShirtIndex(newIndex);
            else
                MatchFlow.SetRightSideShirtIndex(newIndex);
            
            _shirtIndex = newIndex;
        }

        public void ChangeShoes(int nextIndex)
        {
            int newIndex = GetNextShoesIndex(nextIndex);
            _shoesLeftImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            _shoesRightImage.sprite = _customizationImages.GetShoesSprite(newIndex);

            if (_fieldSideType == FieldSideType.Left)
                MatchFlow.SetLeftSideShoesIndex(newIndex);
            else
                MatchFlow.SetRightSideShoesIndex(newIndex);
            
            _shoesIndex = newIndex;
        }

        int GetNextShirtIndex(int delta)
        {
            int count = _customizationImages.GetShirtSpriteCount();
            return (_shirtIndex + delta + count) % count;
        }
        
        int GetNextShoesIndex(int delta)
        {
            int count = _customizationImages.GetShoesSpriteCount();
            return (_shoesIndex + delta + count) % count;
        }
    }
}
