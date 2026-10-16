using CommonDataTypes;
using TMPro;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.UiSystem
{
    public class MatchWinnerView : UIViewWithData<FieldSideData>
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] TextMeshProUGUI _nextLevelText;
        [SerializeField] GameObject _leftWinText;
        [SerializeField] GameObject _rightWinText;

        [Header("Heading Override")]
        [SerializeField] bool _useCustomHeading;
        [SerializeField] string _customHeading;
        [SerializeField] Color _customHeadingColor = Color.white;
        [SerializeField] Material _customHeadingMaterial;
        
        protected override void OnDataReceived(FieldSideData sideData)
        {
            base.OnDataReceived(sideData);
            bool useCustomHeading = _useCustomHeading && _text != null;
            if (useCustomHeading)
            {
                if (_leftWinText != null)
                    _leftWinText.SetActive(false);
                if (_rightWinText != null)
                    _rightWinText.SetActive(false);
                _text.gameObject.SetActive(true);
                _text.text = _customHeading;
                _text.color = _customHeadingColor;
                if (_customHeadingMaterial != null)
                    _text.fontSharedMaterial = _customHeadingMaterial;
            }

            switch (sideData.SideType)
            {
                case FieldSideType.Left: 
                    if (!useCustomHeading)
                        _leftWinText.SetActive(true);
                    if (_nextLevelText) _nextLevelText.text = "Next Level";
                    break;
                case FieldSideType.Right:
                    if (!useCustomHeading)
                        _rightWinText.SetActive(true);
                    if (_nextLevelText) _nextLevelText.text = "Restart";
                    break;
                default: break;
            }

        }
    }
}
