using System.Collections.Generic;
using UnityEngine;

namespace UI.Customization.Countries
{
    [CreateAssetMenu(fileName = "CountriesImages", menuName = "Customizations/Countries Images")]
    public class CountriesImages : ScriptableObject
    {
        [SerializeField] List<Sprite> _countriesImages = new();

        public Sprite GetCountrySprite(int index) => _countriesImages[index];

        public int GetListCount() => _countriesImages.Count;
    }
}
