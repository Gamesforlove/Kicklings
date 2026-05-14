using CommonDataTypes;
using System.Collections.Generic;
using UI.ButtonsBehaviours;
using UnityEngine;

namespace UI.Customization.Countries
{
    public class CountrySelectionListing : MonoBehaviour
    {
        [SerializeField] TeamsData _teamsData;
        [SerializeField] GameObject _flagButtonPrefab;
        private Dictionary<int, FlagButtonBehaviour> _buttons = new Dictionary<int, FlagButtonBehaviour>();

        void Awake()
        {
            foreach (TeamsData.TeamData team in _teamsData.Teams)
            {
                GameObject countryFlag = Instantiate(_flagButtonPrefab, transform);
                FlagButtonBehaviour flagButton = countryFlag.GetComponent<FlagButtonBehaviour>();
                flagButton.SetUp(team);
                _buttons.Add(team.Id, flagButton);
            }
        }
        public void DisableButton(int id)
        {
            if(_buttons.TryGetValue(id, out FlagButtonBehaviour flagButton))
            {
                flagButton.Deactivate();
            }
        }
        public void EnableButton(int id)
        {
            if (_buttons.TryGetValue(id, out FlagButtonBehaviour flagButton))
            {
                flagButton.Activate();
            }
        }
        public void EnableAlButtons()
        {
            foreach (FlagButtonBehaviour flagButton in _buttons.Values)
            {
                flagButton.Activate();
            }
        }
    }
}
