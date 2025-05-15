using CommonDataTypes;
using EventBusSystem;
using UI.UiSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Customization.Countries
{
    public class CountryCustomizationController : MonoBehaviour
    {
        public int TeamDataIndex { get; private set; }
        
        [SerializeField] FieldSideType _fieldSideType;
        [SerializeField] TeamsData _teamsData;
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] Image _image;

        void Start()
        {

            TeamsData.TeamData defaultTeamData = _teamsData.GetTeamById(0);
            ChangeCountryImage(defaultTeamData);
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
            _image.sprite = teamData.Icon;
            TeamDataIndex = teamData.Id;
        }
    }
}
