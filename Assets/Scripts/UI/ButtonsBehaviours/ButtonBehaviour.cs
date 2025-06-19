using UnityEngine;
using UnityEngine.UI;

namespace UI.ButtonsBehaviours
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBehaviour : MonoBehaviour
    {
        Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
        }

        protected virtual void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();
    }
}