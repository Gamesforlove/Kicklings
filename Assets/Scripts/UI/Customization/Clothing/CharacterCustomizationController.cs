using CommonDataTypes;
using Scene_Management;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Customization.Clothing
{
    public class CharacterCustomizationController : MonoBehaviour
    {
        public int ShirtIndex { get; private set; } 
        public int ShoesIndex { get; private set; }
        
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] FieldSideType  _fieldSideType;
        [SerializeField] Image _shirtImage,  _shoesLeftImage, _shoesRightImage;

        void Start()
        {
            ChangeShirt(0);
            ChangeShoes(0);

            if (_fieldSideType == FieldSideType.Left)
            {
                MatchFlow.SetLeftSideShoesIndex(ShirtIndex);
                MatchFlow.SetLeftSideShoesIndex(ShoesIndex);
            }
            else
            {
                MatchFlow.SetRightSideShoesIndex(ShirtIndex);
                MatchFlow.SetRightSideShoesIndex(ShoesIndex);
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
            
            ShirtIndex = newIndex;
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
    }
}
