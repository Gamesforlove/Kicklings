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
        
        [SerializeField] FieldSideType _fieldSideType;
        [SerializeField] TeamsData _teamsData;
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] CountryCustomizationView _countryCustomizationView;
        [SerializeField] UIView _countrySelectionView;
        [SerializeField] CountryFacts _countryFacts;
        [SerializeField] TextMeshProUGUI _countryFactTextBox;
        
        bool _isSelected;
        
        public void Select() => _isSelected = true;

        void Start()
        {
            ChangeCountryImage(_teamsData.GetTeamById(0));
            ChangeCountryFact(_teamsData.GetTeamById(0));
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
            if (!_isSelected) return;

            ChangeCountryImage(payload.TeamData);

            ChangeCountryFact(payload.TeamData);

            _uiViewsManager.HideView(_countrySelectionView);
        }

        private void ChangeCountryFact(TeamsData.TeamData teamData)
        {
            if (_countryFactTextBox && _countryFacts)
                _countryFactTextBox.text = _countryFacts.GetRandomCountryFactByName(teamData.Name);
        }

        void ChangeCountryImage(TeamsData.TeamData teamData)
        {
            _countryCustomizationView.ChangeViewElements(teamData);
            TeamDataIndex = teamData.Id;
            _isSelected = false;
        }
    }
}
