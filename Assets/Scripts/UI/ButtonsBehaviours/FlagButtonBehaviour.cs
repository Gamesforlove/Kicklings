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

        int _index;
    
        public void SetUp(int index, Sprite sprite)
        {
            _index = index;
            _flagButtonImage.sprite = sprite;
            _flagButtonText.text = sprite.name;
        }

        public void OnClick()
        {
            EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_index));
        }
    }
}
