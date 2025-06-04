using System.Globalization;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    public class Option : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] TMP_InputField _inputField;
        
        private FieldInfo _fieldInfo;
        private object _target;
        private FieldWrapper _fieldWrapper;
        private PropertyWrapper _propertyWrapper;



         void Awake()
        {
            _inputField.onValueChanged.AddListener(OnValueChanged);
        }

        void OnDestroy()
        {
            _inputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void SetUpContent(string text, float value, FieldInfo fieldInfo, object target)
        {
            SetupCommon(text, value);
            _fieldInfo = fieldInfo;
            _target = target;
            _fieldWrapper = null;
        }

        public void SetUpContent(string text, float value, FieldWrapper fieldWrapper)
        {
            SetupCommon(text, value);
            _fieldWrapper = fieldWrapper;
            _fieldInfo = null;
            _target = null;
        }

        public void SetUpContent(string text, float value, PropertyWrapper propertyWrapper)
        {
            SetupCommon(text, value);
            _propertyWrapper = propertyWrapper;
            _fieldWrapper = null;
            _fieldInfo = null;
            _target = null;
        }
        
        private void SetupCommon(string text, float value)
        {
            string formattedText = string.Concat(text.Select((c, i) => 
                i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        
            _text.text = formattedText;
            _inputField.text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void OnValueChanged(string newValue)
        {
            if (!float.TryParse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
                return;

            if (_propertyWrapper != null)
            {
                _propertyWrapper.SetValue(parsedValue);
            }
            else if (_fieldWrapper != null)
            {
                _fieldWrapper.SetValue(parsedValue);
            }
            else if (_fieldInfo != null && _target != null)
            {
                _fieldInfo.SetValue(_target, parsedValue);
            }
        }

    }
}