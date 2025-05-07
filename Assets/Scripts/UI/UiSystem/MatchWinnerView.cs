using CommonDataTypes;
using TMPro;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.UiSystem
{
    public class MatchWinnerView : UIViewWithData<FieldSideData>
    {
        [SerializeField] TextMeshProUGUI _text;
        
        protected override void OnDataReceived(FieldSideData sideData)
        {
            base.OnDataReceived(sideData);
            _text.text = sideData.SideType switch
            {
                FieldSideType.Left => "RED WINS!",
                FieldSideType.Right => "BLUE WINS!",
            };

            _text.color = sideData.Color;
        }
    }
}