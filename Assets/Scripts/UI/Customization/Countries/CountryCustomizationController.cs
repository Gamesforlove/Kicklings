using CommonDataTypes;
using EventBusSystem;
using TMPro;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Customization.Countries
{
    public class CountryCustomizationController : MonoBehaviour
    {
        public int TeamDataIndex { get; private set; }
        public bool IsSelected { get; private set; }

        [SerializeField] FieldSideType _fieldSideType;
        [SerializeField] TeamsData _teamsData;
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] CountryCustomizationView _countryCustomizationView;
        [SerializeField] UIView _countrySelectionView;
        [SerializeField] CountryFacts _countryFacts;
        [SerializeField] TextMeshProUGUI _countryFactTextBox;
        
        
        public void Select() => IsSelected = true;

        void Start()
        {
/*            ChangeCountryImage(_teamsData.GetTeamById(randomIndex));
            ChangeCountryFact(_teamsData.GetTeamById(randomIndex));*/
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
            if (!IsSelected) return;

            ChangeCountryImage(payload.TeamData);

            ChangeCountryFact(payload.TeamData);

            _uiViewsManager.HideView(_countrySelectionView);
        }

        public void ChangeCountryFact(TeamsData.TeamData teamData)
        {
            if (_countryFactTextBox && _countryFacts)
                _countryFactTextBox.text = _countryFacts.GetRandomCountryFactByName(teamData.Name);
        }

        public void ChangeCountryImage(TeamsData.TeamData teamData)
        {
            _countryCustomizationView.ChangeViewElements(teamData);
            TeamDataIndex = teamData.Id;
            IsSelected = false;
        }
    }
}
