using Scene_Management;
using UnityEngine;

namespace UI.ButtonsBehaviours
{
    public class CreateMatchButton : MonoBehaviour
    {
        [SerializeField] int _numberOfPlayers;
        
        public void OnClick()
        {
            MatchFlow.SetNumberOfPlayers(_numberOfPlayers);
            MatchFlow.CreateMatch();
        }
    }
}