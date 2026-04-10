using CommonDataTypes;
using UI.Customization.Clothing;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class ClothesSetter : MonoBehaviour
    {
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] TeamsData _teamsData;
        [SerializeField] SpriteRenderer _shirtSpriteRenderer, _shirtPatternSpriteRenderer, _shoesLeftSpriteRenderer, _shoesRightSpriteRenderer,
        _leftSleeveSpriteRenderer, _rightSleeveSpriteRenderer, _leftShortSockSpriteRenderer, _rightShortSockSpriteRenderer;

        public void SetClothes(int shirtIndex, int shoesIndex)
        {
            _shirtPatternSpriteRenderer.gameObject.SetActive(true);
            _shirtSpriteRenderer.color = _leftSleeveSpriteRenderer.color;
            _shirtPatternSpriteRenderer.sprite = _customizationImages.GetShirtSprite(shirtIndex);
            _shoesLeftSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
            _shoesRightSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
        }
        public void SetClothes(int countryIndex)
        {
            _shirtSpriteRenderer.sprite = _teamsData.GetTeamById(countryIndex).ShirtSprite;
            _leftSleeveSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
            _rightSleeveSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
            _leftShortSockSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
            _rightShortSockSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
        }
    }
}
