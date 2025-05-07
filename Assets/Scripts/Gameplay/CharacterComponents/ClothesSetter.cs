using UI.Customization.Clothing;
using UI.MainMenu.FreeMode;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class ClothesSetter : MonoBehaviour
    {
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] SpriteRenderer _shirtSpriteRenderer, _shoesLeftSpriteRenderer, _shoesRightSpriteRenderer;

        public void SetClothes(int shirtIndex, int shoesIndex)
        {
            _shirtSpriteRenderer.sprite = _customizationImages.GetShirtSprite(shirtIndex);
            _shoesLeftSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
            _shoesRightSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
        }
    }
}
