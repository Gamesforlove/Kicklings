using CommonDataTypes;
using EventBusSystem;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Customization.Countries
{
    public class CountryCustomizationController : MonoBehaviour
    {
        public int TeamDataIndex { get; private set; }
        
        [SerializeField] FieldSideType _fieldSideType;
        [SerializeField] TeamsData _teamsData;
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] CountryCustomizationView _countryCustomizationView;

        void Start()
        {
            ChangeCountryImage(_teamsData.GetTeamById(0));
        }

        void OnEnable()
        {
            EventBus<OnCountryChanged>.OnEvent += OnCountryChanged;
        }

        void OnDisable()
        {
            EventBus<OnCountryChanged>.OnEvent -= OnCountryChanged;
        }

        void OnCountryChanged(OnCountryChanged payload)
        {
            ChangeCountryImage(payload.TeamData);
            _uiViewsManager.HideCurrentView();
        }

        void ChangeCountryImage(TeamsData.TeamData teamData)
        {
            _countryCustomizationView.ChangeViewElements(teamData);
            TeamDataIndex = teamData.Id;
        }
    }
}
