using CommonDataTypes;
using Scene_Management;
using UI.Customization.Countries;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCelebrationFlag: MonoBehaviour
{
    public TeamsData.TeamData PlayerTeamData;
    [SerializeField] Image _countryImage;
    [SerializeField] CountriesImages _countriesImages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _countryImage.sprite = _countriesImages.GetCountrySprite(MatchFlow.Match.Settings.LeftCountryImageIndex);
    }

}
