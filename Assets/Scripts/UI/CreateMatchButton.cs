using CommonDataTypes;
using Scene_Management;
using UnityEngine;

namespace UI
{
    public class CreateMatchButton : MonoBehaviour
    {
        [SerializeField] GameModeData _gameModeData;
        
        public void OnClick()
        {
            MatchFlow.CreateMatch(_gameModeData);
        }
    }
}