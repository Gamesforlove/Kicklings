using System.Collections;
using TMPro;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentLayoutView : UIView
    {
        [SerializeField] TournamentLayout[] _layouts;
        [SerializeField] TextMeshProUGUI _roundText;

        Tournament _tournament;
        
        public override IEnumerator Show()
        {
            foreach (TournamentLayout layout in _layouts)
            {
                layout.gameObject.SetActive(false);
            }

            _tournament = TournamentModeController.Tournament;
            _layouts[_tournament.LayoutMode].Show(_tournament);
            _roundText.text = "Round " + _tournament.CurrentRound.Id;
            yield return base.Show();
        }
    }
}