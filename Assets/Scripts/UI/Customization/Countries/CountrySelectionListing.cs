using CommonDataTypes;
using UI.ButtonsBehaviours;
using UnityEngine;

namespace UI.Customization.Countries
{
    public class CountrySelectionListing : MonoBehaviour
    {
        [SerializeField] TeamsData _teamsData;
        [SerializeField] GameObject _flagButtonPrefab;

        void Start()
        {
            foreach (TeamsData.TeamData team in _teamsData.Teams)
            {
                GameObject countryFlag = Instantiate(_flagButtonPrefab, transform);
                countryFlag.GetComponent<FlagButtonBehaviour>().SetUp(team);
            }
        }
    
    
    }
}
