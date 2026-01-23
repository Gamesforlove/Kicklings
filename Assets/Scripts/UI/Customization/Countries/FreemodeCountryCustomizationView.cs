using CommonDataTypes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Customization.Countries
{
    public class FreemodeCountryCustomizationView : CountryCustomizationView
    {
        [SerializeField] Image _image;
        
        public override void ChangeViewElements(TeamsData.TeamData teamData)
        {
            _image.sprite = teamData.Icon;
        }
    }
}