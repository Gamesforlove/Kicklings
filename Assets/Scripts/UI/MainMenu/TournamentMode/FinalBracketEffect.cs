using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class FinalBracketEffect : BracketVisualEffect
    {
        [SerializeField] Color _color;

        public override void SetActive(bool active)
        {
            Background.color = active ? _color : Color.white;
            Glow.gameObject.SetActive(active);
        }
    }
}