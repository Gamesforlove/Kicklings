using CommonDataTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Customization.Countries
{
    public class TournamentCountryCustomizationView : CountryCustomizationView
    {
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _description;
        
        public override void ChangeViewElements(TeamsData.TeamData teamData)
        {
            _image.sprite = teamData.Icon;
            _name.text = teamData.Name;
        }
    }
}