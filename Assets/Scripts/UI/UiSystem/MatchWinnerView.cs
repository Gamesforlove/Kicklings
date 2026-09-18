using CommonDataTypes;
using TMPro;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.UiSystem
{
    public class MatchWinnerView : UIViewWithData<FieldSideData>
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] GameObject _redWinText;
        [SerializeField] GameObject _blueWinText;
        
        protected override void OnDataReceived(FieldSideData sideData)
        {
            base.OnDataReceived(sideData);
            /*            _text.text = sideData.SideType switch
                        {
                            FieldSideType.Left => "RED WINS!",
                            FieldSideType.Right => "BLUE WINS!",
                        };
            _text.color = sideData.Color;
            */
            switch (sideData.SideType)
            {
                case FieldSideType.Left: _redWinText.SetActive(true); break;
                case FieldSideType.Right: _blueWinText.SetActive(true); break;
                default: break;
            }

        }
    }
}