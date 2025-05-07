using System.Collections.Generic;
using UnityEngine;

namespace UI.Customization.Clothing
{
    [CreateAssetMenu(fileName = "CharacterCustomizationImages", menuName = "Customizations/CharacterCustomizationImages")]
    public class CharacterCustomizationImages : ScriptableObject
    {
        [SerializeField] List<Sprite> _shirtImages = new();
        [SerializeField] List<Sprite> _shoesImages = new();

        public Sprite GetShirtSprite(int index) => _shirtImages[index];
        public Sprite GetShoesSprite(int index) => _shoesImages[index];

        public int GetShirtSpriteCount() => _shirtImages.Count;
        public int GetShoesSpriteCount() => _shoesImages.Count;
    }
}