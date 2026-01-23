using System;
using System.Reflection;
using CommonDataTypes;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using UnityEngine;

namespace UI.Gameplay
{
    public class Configuration : MonoBehaviour
    {
        [SerializeField] Transform _content;
        [SerializeField] Option _optionPrefab;

        public void SetUpContent(object data)
        {
            ClearContent();
            
            switch (data)
            {
                case EntityData entityData:
                    SetUpEntityData(entityData);
                    break;
                case CpuDifficultyPreset.DifficultySettings difficultySettings:
                    SetUpDifficultySettings(difficultySettings);
                    break;
                case MatchSettings matchSettings:
                    SetUpMatchSettings(matchSettings);
                    break;
            }

        }

        private void ClearContent()
        {
            foreach (Transform child in _content)
            {
                Destroy(child.gameObject);
            }
        }

        private void SetUpEntityData(EntityData data)
        {
            var fields = typeof(EntityData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(float)) continue;
                
                var option = Instantiate(_optionPrefab, _content);
                float value = (float)field.GetValue(data);
                option.SetUpContent(field.Name, value, field, data);
            }
        }

        private void SetUpDifficultySettings(CpuDifficultyPreset.DifficultySettings data)
        {
            var fields = typeof(CpuDifficultyPreset.DifficultySettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
        
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(CpuDifficultyPreset.FloatRange))
                {
                    var floatRange = (CpuDifficultyPreset.FloatRange)field.GetValue(data);
                    
                    // Create option for Min value
                    var minOption = Instantiate(_optionPrefab, _content);
                    minOption.SetUpContent($"{field.Name} Min", floatRange.Min, 
                        new FieldWrapper(field, data, true));
                    
                    // Create option for Max value
                    var maxOption = Instantiate(_optionPrefab, _content);
                    maxOption.SetUpContent($"{field.Name} Max", floatRange.Max, 
                        new FieldWrapper(field, data, false));
                }
            }
        }
        
        private void SetUpMatchSettings(MatchSettings data)
        {
            var properties = typeof(MatchSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Skip properties that can't be set or are marked private set
                if (!property.CanWrite || property.SetMethod?.IsPrivate == true) continue;
        
                // Only handle integer properties
                if (property.PropertyType != typeof(int)) continue;
        
                var option = Instantiate(_optionPrefab, _content);
                var value = Convert.ToSingle(property.GetValue(data));
                option.SetUpContent(property.Name, value, new PropertyWrapper(property, data));
            }
        }

    }
    
    public class FieldWrapper
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _target;
        private readonly bool _isMin;

        public FieldWrapper(FieldInfo fieldInfo, object target, bool isMin)
        {
            _fieldInfo = fieldInfo;
            _target = target;
            _isMin = isMin;
        }

        public void SetValue(float newValue)
        {
            var currentRange = (CpuDifficultyPreset.FloatRange)_fieldInfo.GetValue(_target);
        
            if (_isMin)
            {
                currentRange.Min = newValue;
            }
            else
            {
                currentRange.Max = newValue;
            }
        
            _fieldInfo.SetValue(_target, currentRange);
        }
    }
    
    public class PropertyWrapper
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _target;

        public PropertyWrapper(PropertyInfo propertyInfo, object target)
        {
            _propertyInfo = propertyInfo;
            _target = target;
        }

        public void SetValue(float newValue)
        {
            _propertyInfo.SetValue(_target, (int)newValue);
        }
    }
}