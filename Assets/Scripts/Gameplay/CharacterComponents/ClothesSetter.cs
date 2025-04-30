using CommonDataTypes;
using UI.MainMenu.FreeMode;
using UnityEngine;

public class ClothesSetter : MonoBehaviour
{
    [SerializeField] CharacterCustomizationImages _customizationImages;
    [SerializeField] SpriteRenderer _shirtSpriteRenderer, _shoesLeftSpriteRenderer, _shoesRightSpriteRenderer;

    public void SetClothes(FieldSideType fieldSideType)
    {
        string shirtKey = fieldSideType == FieldSideType.Left ? 
            CharacterCustomizationPlayerPrefsKeys.LeftShirt.ToString() : CharacterCustomizationPlayerPrefsKeys.RightShirt.ToString();
        string shoesKey = fieldSideType == FieldSideType.Left ? 
            CharacterCustomizationPlayerPrefsKeys.LeftShoes.ToString() : CharacterCustomizationPlayerPrefsKeys.RightShoes.ToString();
        
        int shirtIndex = PlayerPrefs.GetInt(shirtKey, 0);
        int shoesIndex = PlayerPrefs.GetInt(shoesKey, 0);
        
        _shirtSpriteRenderer.sprite = _customizationImages.GetShirtSprite(shirtIndex);
        _shoesLeftSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
        _shoesRightSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
    }
}
