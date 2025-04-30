using CommonDataTypes;
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
        string _shirtKey, _shoesKey;

        void Awake()
        {
            _shirtKey = _fieldSideType == FieldSideType.Left ? 
                CharacterCustomizationPlayerPrefsKeys.LeftShirt.ToString() : CharacterCustomizationPlayerPrefsKeys.RightShirt.ToString();
            _shoesKey = _fieldSideType == FieldSideType.Left ? 
                CharacterCustomizationPlayerPrefsKeys.LeftShoes.ToString() : CharacterCustomizationPlayerPrefsKeys.RightShoes.ToString();
        }

        void Start()
        {
            _shirtIndex = PlayerPrefs.GetInt(_shirtKey, 0);
            _shoesIndex = PlayerPrefs.GetInt(_shoesKey, 0);
            _shirtImage.sprite = _customizationImages.GetShirtSprite(_shirtIndex);
            _shoesLeftImage.sprite = _customizationImages.GetShoesSprite(_shoesIndex);
            _shoesRightImage.sprite = _customizationImages.GetShoesSprite(_shoesIndex);
        }
        
        public void ChangeShirt(int nextIndex)
        {
            int newIndex = GetNextShirtIndex(nextIndex);
            _shirtImage.sprite = _customizationImages.GetShirtSprite(newIndex);
            PlayerPrefs.SetInt(_shirtKey, newIndex);
            _shirtIndex = newIndex;
        }

        public void ChangeShoes(int nextIndex)
        {
            int newIndex = GetNextShoesIndex(nextIndex);
            _shoesLeftImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            _shoesRightImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            PlayerPrefs.SetInt(_shoesKey, newIndex);
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
