using CommonDataTypes;
using EventBusSystem;
using TMPro;
using UI.Customization.Clothing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ButtonsBehaviours
{
    public class FlagButtonBehaviour : MonoBehaviour
    {
        [SerializeField] Image _flagButtonImage;
        [SerializeField] TextMeshProUGUI _flagButtonText;
        [SerializeField] CharacterCustomizationController customizationController;
        
        TeamsData.TeamData _teamData;
    
        public void SetUp(TeamsData.TeamData teamData)
        {
            _teamData = teamData;
            _flagButtonImage.sprite = teamData.Icon;
            _flagButtonText.text = teamData.Name;
        }

        public void OnClick()
        {
            transform.parent.parent.TryGetComponent<LastSelectedCountryController>(out LastSelectedCountryController lastSelectedCountryController);
            if (!lastSelectedCountryController)
            {
                Debug.Log("lastSelectedFieldSideType Null");
                return;
            }
            EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_teamData, lastSelectedCountryController.lastSelectedFieldSideType));
        }
    }
}
