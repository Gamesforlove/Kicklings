using CommonDataTypes;
using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ButtonsBehaviours
{
    public class FlagButtonBehaviour : MonoBehaviour
    {
        [SerializeField] Image _flagButtonImage;
        [SerializeField] TextMeshProUGUI _flagButtonText;
        
        TeamsData.TeamData _teamData;
    
        public void SetUp(TeamsData.TeamData teamData)
        {
            _teamData = teamData;
            _flagButtonImage.sprite = teamData.Icon;
            _flagButtonText.text = teamData.Name;
        }

        public void OnClick()
        {
            EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_teamData));
        }
    }
}
