using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using static CommonDataTypes.TeamsData;

namespace UI.Customization.Clothing
{
    public class CharacterCustomizationController : MonoBehaviour
    {
        public int ShirtIndex { get; private set; } 
        public int ShoesIndex { get; private set; }
        public int CountryIndex { get; private set; }
        [field: SerializeField] public Gradient SkinTone { get; private set; }
        [field: SerializeField, Range(0, 1)] public float SkinToneValue { get; private set; }
        
        [SerializeField] CharacterCustomizationImages _customizationImages;
        [SerializeField] FieldSideType  _fieldSideType;
        [SerializeField] Image _shirtImage, _shirtPatternImage, _shoesLeftImage, _shoesRightImage, _leftSleeveImage, 
        _rightSleeveImage, _leftShortSockImage, _rightShortSockImage, _leftArmFleshImage, _rightArmFleshImage, 
        _leftLegFleshImage, _rightLegFleshImage, _faceImage;

        void Start()
        {
/*            ChangeShirt(0);
            ChangeShoes(0);*/
        }
        private void OnValidate()
        {
            //ChangeSkinTone(SkinToneValue);
        }
        private void OnEnable()
        {
            SetUpSkinTone();
            EventBus<OnCountryChanged>.OnEvent += OnCountyChanged;
        }

        private void OnDisable()
        {
            EventBus<OnCountryChanged>.OnEvent -= OnCountyChanged;
        }

        public void ChangeShirt(int nextIndex)
        {
            int newIndex = GetNextShirtIndex(nextIndex);
            _shirtPatternImage.sprite = _customizationImages.GetShirtSprite(newIndex);
            
            ShirtIndex = newIndex;
        }

        public void ChangeShoes(int nextIndex)
        {
            int newIndex = GetNextShoesIndex(nextIndex);
            _shoesLeftImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            _shoesRightImage.sprite = _customizationImages.GetShoesSprite(newIndex);
            
            ShoesIndex = newIndex;
        }

        int GetNextShirtIndex(int delta)
        {
            int count = _customizationImages.GetShirtSpriteCount();
            return (ShirtIndex + delta + count) % count;
        }
        
        int GetNextShoesIndex(int delta)
        {
            int count = _customizationImages.GetShoesSpriteCount();
            return (ShoesIndex + delta + count) % count;
        }

        public void OnCountyChanged(OnCountryChanged evt)
        {
            if (!gameObject.activeSelf)
                return;
            if (evt.LastSelectedFieldSideType == FieldSideType.None)
            {
                SetCountryOutfit(evt.TeamData);

                CountryIndex = evt.TeamData.Id;
            }
        }

        public void SetCountryOutfit(TeamData teamData)
        {
            _shirtImage.sprite = teamData.ShirtSprite;
            _leftSleeveImage.color = teamData.CountryColor;
            _rightSleeveImage.color = teamData.CountryColor;
            _leftShortSockImage.color = teamData.CountryColor;
            _rightShortSockImage.color = teamData.CountryColor;
        }
        public void ChangeSkinTone(float skinToneValue)
        {
            _leftArmFleshImage.color = SkinTone.Evaluate(skinToneValue);
            _rightArmFleshImage.color = SkinTone.Evaluate(skinToneValue);
            _leftLegFleshImage.color = SkinTone.Evaluate(skinToneValue);
            _rightLegFleshImage.color = SkinTone.Evaluate(skinToneValue);
            _faceImage.color = SkinTone.Evaluate(skinToneValue);
        }
        private void SetUpSkinTone()
        {
            var m_Scene = SceneManager.GetActiveScene();
            string sceneName = m_Scene.name;
            if (sceneName == "Gameplay")
            {
                ChangeSkinTone(MatchFlow.Match.Settings.LeftSkinToneValue);
                return;
            }
            SkinToneValue = Random.Range(0, 1.0f);
            ChangeSkinTone(SkinToneValue);
        }
    }
}
