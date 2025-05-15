using System.Collections;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentLayoutView : UIView
    {
        [SerializeField] TournamentLayout[] _layouts;

        Tournament _tournament;
        
        public override IEnumerator Show()
        {
            foreach (TournamentLayout layout in _layouts)
            {
                layout.gameObject.SetActive(false);
            }

            _tournament = TournamentModeController.Tournament;
            _layouts[_tournament.LayoutMode].Show(_tournament);
            yield return base.Show();
        }
    }
}