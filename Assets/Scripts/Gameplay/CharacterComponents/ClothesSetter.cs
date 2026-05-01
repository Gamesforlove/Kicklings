using CommonDataTypes;
using UI.Customization.Clothing;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class ClothesSetter : MonoBehaviour
    {
        [SerializeField] Gradient _skinToneGradient;
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] TeamsData _teamsData;
        [SerializeField] SpriteRenderer _shirtSpriteRenderer, _shirtPatternSpriteRenderer, _shoesLeftSpriteRenderer, _shoesRightSpriteRenderer,
        _leftSleeveSpriteRenderer, _rightSleeveSpriteRenderer, _leftShortSockSpriteRenderer, _rightShortSockSpriteRenderer, _leftArmFleshSpriteRenderer, 
        _rightArmFleshSpriteRenderer, _leftLegFleshSpriteRenderer, _rightLegFleshSpriteRenderer, _faceSpriteRenderer;

        public void SetClothes(int shirtIndex, int shoesIndex, float skinToneValue)
        {
            _shirtPatternSpriteRenderer.gameObject.SetActive(true);
            _shirtSpriteRenderer.color = _leftSleeveSpriteRenderer.color;
            _shirtPatternSpriteRenderer.sprite = _customizationImages.GetShirtSprite(shirtIndex);
            _shoesLeftSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);
            _shoesRightSpriteRenderer.sprite = _customizationImages.GetShoesSprite(shoesIndex);

            SetSkinColor(skinToneValue);
        }
        public void SetClothes(int countryIndex, float skinToneValue)
        {
            _shirtSpriteRenderer.sprite = _teamsData.GetTeamById(countryIndex).ShirtSprite;
            _leftSleeveSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
            _rightSleeveSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
            _leftShortSockSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;
            _rightShortSockSpriteRenderer.color = _teamsData.GetTeamById(countryIndex).CountryColor;

            SetSkinColor(skinToneValue);
        }

        private void SetSkinColor(float skinToneValue)
        {
            _leftArmFleshSpriteRenderer.color = _skinToneGradient.Evaluate(skinToneValue);
            _rightArmFleshSpriteRenderer.color = _skinToneGradient.Evaluate(skinToneValue);
            _leftLegFleshSpriteRenderer.color = _skinToneGradient.Evaluate(skinToneValue);
            _rightLegFleshSpriteRenderer.color = _skinToneGradient.Evaluate(skinToneValue);
            _faceSpriteRenderer.color = _skinToneGradient.Evaluate(skinToneValue);
        }

    }
}
