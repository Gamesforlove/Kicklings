using CommonDataTypes;
using EventBusSystem;
using TMPro;
using UI.Customization.Clothing;
using UnityEngine;
using UnityEngine.UI;
using static CommonDataTypes.TeamsData;

namespace UI.ButtonsBehaviours
{
    public class FlagButtonBehaviour : MonoBehaviour
    {
        [SerializeField] Image _flagButtonImage;
        [SerializeField] TextMeshProUGUI _flagButtonText;
        [SerializeField] Color _disabledFlagColor;
        [SerializeField] Color _disabledTextColor;
        
        TeamData _teamData;
        Button _button;
        
    
        public void SetUp(TeamData teamData)
        {
            _button = GetComponent<Button>();
            _teamData = teamData;
            _flagButtonImage.sprite = teamData.Icon;
            _flagButtonText.text = teamData.Name;
        }

        public void OnClick()
        {
            transform.parent.parent.TryGetComponent(out LastSelectedCountryController lastSelectedCountryController);
            if (!lastSelectedCountryController)
            {
                Debug.Log("lastSelectedFieldSideType Null");
                return;
            }
            EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_teamData, lastSelectedCountryController.lastSelectedFieldSideType));
        }
        public void Activate()
        {
            _button.interactable = true;
            _flagButtonImage.color = Color.white;
            _flagButtonText.color = Color.white;
        }
        public void Deactivate()
        {
            _button.interactable = false;
            _flagButtonImage.color = _disabledFlagColor;
            _flagButtonText.color = _disabledTextColor;
        }
    }
}
